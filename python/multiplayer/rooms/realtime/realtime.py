"""Realtime client for game rooms.

Two channels are available at the same time:

- a reliable WebSocket (TCP) connection for control traffic, and
- one bound UDP endpoint for high-frequency unreliable traffic
  (movement state, inputs) — see :meth:`Realtime.connect_udp` and
  :meth:`Realtime.send_unreliable`.

Requires the optional ``websockets`` package::

    pip install websockets

Usage::

    realtime = Realtime()
    token = await realtime.get_token(client, player_token)
    await realtime.connect(token.token)
    await realtime.connect_udp()          # optional fast lane

    realtime.on_receive = lambda command, data, sender: print(command, data)
    await realtime.send(RoomTargetPlayer.ALL, "move", {"x": 1, "y": 2})
    await realtime.send_unreliable(RoomTargetPlayer.ALL, "pos", {"x": 1})
"""

from __future__ import annotations

import asyncio
import inspect
import json
import struct
import time
import uuid as _uuid
from typing import Any, Callable, List, Optional
from urllib.parse import urlparse

import requests

from ..._serialization import from_dict
from ...client import Client
from .enums import RoomTargetPlayer
from .models import RealtimeMessage, SenderInfo, TokenResponse

REALTIME_TOKEN = "realtime.php/token"
DEFAULT_REALTIME_WS_URL = "wss://realtime.michitai.com"
DEFAULT_REALTIME_HTTP_URL = "https://realtime.michitai.com/"

HEARTBEAT_INTERVAL = 20  # seconds

# MUR1 datagram layout — shared with realtime/src/protocol.js.
_UDP_MAGIC = 0x4D555231
_UDP_HEADER = 21
_UDP_BIND, _UDP_BIND_ACK, _UDP_MSG, _UDP_PING, _UDP_PONG = 1, 2, 3, 4, 5

OnReceiveCallback = Callable[[str, Any, Optional[SenderInfo]], Any]
OnConnectedCallback = Callable[[], Any]


def _udp_encode(packet_type: int, connection_id: str, body: Optional[bytes]) -> bytes:
    payload = body or b""
    return (
        struct.pack(">IB", _UDP_MAGIC, packet_type)
        + _uuid.UUID(connection_id).bytes
        + payload
    )


def _udp_decode(datagram: bytes):
    """Returns ``(packet_type, connection_id, json_str)`` or None."""
    if len(datagram) < _UDP_HEADER or datagram[:4] != b"MUR1":
        return None
    packet_type = datagram[4]
    connection_id = str(_uuid.UUID(bytes=datagram[5:21]))
    json_str = (
        datagram[_UDP_HEADER:].decode("utf-8") if len(datagram) > _UDP_HEADER else None
    )
    return packet_type, connection_id, json_str


def get_token(client: Client, player_token: str) -> TokenResponse:
    """Retrieve a realtime authentication token for WebSocket connections.

    :param client: The API client instance.
    :param player_token: The player's private authentication token.
    """
    return client.send(
        "POST",
        client.url(REALTIME_TOKEN),
        None,
        TokenResponse,
        player_token=player_token,
    )


class _UdpProtocol(asyncio.DatagramProtocol):
    """Forwards received datagrams to the owning Realtime client."""

    def __init__(self, owner: "Realtime") -> None:
        self._owner = owner

    def datagram_received(self, data: bytes, addr) -> None:  # noqa: D102
        self._owner._on_udp_datagram(data)


class Realtime:
    """Manages the realtime channels for room communication.

    Async API built on ``asyncio`` + ``websockets`` + ``asyncio`` datagram
    endpoints. Set :attr:`on_receive` and :attr:`on_connected` to subscribe.
    """

    def __init__(self, realtime_websocket_url: str = DEFAULT_REALTIME_WS_URL) -> None:
        self._url = realtime_websocket_url
        self._ws: Any = None
        self._token: Optional[str] = None
        self._listen_task: Optional[asyncio.Task] = None
        self._heartbeat_task: Optional[asyncio.Task] = None

        # UDP companion channel state
        self._udp_transport: Optional[asyncio.DatagramTransport] = None
        self._udp_bound = False
        self._connection_id: Optional[str] = None
        self._server_udp_port = 0
        self._udp_seq = 0
        self.last_udp_rtt_ms = 0

        #: Called with ``(command, data, sender)`` for each received message.
        self.on_receive: Optional[OnReceiveCallback] = None
        #: Called once the WebSocket connection is established.
        self.on_connected: Optional[OnConnectedCallback] = None

    @staticmethod
    def get_token(client: Client, player_token: str) -> TokenResponse:
        """Retrieve a realtime authentication token (see :func:`get_token`)."""
        return get_token(client, player_token)

    @property
    def is_connected(self) -> bool:
        return self._ws is not None and self._ws.open

    @property
    def connection_id(self) -> Optional[str]:
        """The connection id issued by the server (set after connect)."""
        return self._connection_id

    @property
    def is_udp_connected(self) -> bool:
        """True while a UDP address is bound to this session."""
        return self._udp_bound and self._udp_transport is not None

    async def connect(self, realtime_token: str) -> bool:
        """Connect to the realtime WebSocket server.

        :param realtime_token: Token from :func:`get_token`.
        :returns: True if the connection succeeded.
        """
        try:
            import websockets
        except ImportError as exc:
            raise ImportError(
                "The 'websockets' package is required for realtime support. "
                "Install it with: pip install michitai-multiplayer[realtime]"
            ) from exc

        try:
            self._token = realtime_token

            # Wake up the server before connecting
            await asyncio.get_running_loop().run_in_executor(
                None, self._wake_up_server
            )

            # Prefer the header so the token stays out of URLs/logs.
            # websockets renamed extra_headers -> additional_headers in v12.
            try:
                self._ws = await websockets.connect(
                    f"{self._url}?client=json",
                    additional_headers={"X-Realtime-Token": self._token},
                )
            except TypeError:
                try:
                    self._ws = await websockets.connect(
                        f"{self._url}?client=json",
                        extra_headers={"X-Realtime-Token": self._token},
                    )
                except TypeError:
                    self._ws = await websockets.connect(
                        f"{self._url}?client=json&token={self._token}"
                    )

            self._listen_task = asyncio.ensure_future(self._listen())
            self._heartbeat_task = asyncio.ensure_future(self._heartbeat_loop())

            if self.on_connected:
                result = self.on_connected()
                if inspect.isawaitable(result):
                    await result

            return True
        except Exception as exc:
            print(f"Connection failed: {exc}")
            await self.disconnect()
            return False

    async def connect_udp(self, udp_port: Optional[int] = None) -> bool:
        """Bind a UDP endpoint to this session (call after :meth:`connect`).

        The server accepts one UDP binding per connection. Datagrams are
        authenticated with the connection id issued on the socket.

        :param udp_port: Override port — defaults to the ``udp_port``
            announced in the server's connected payload.
        :returns: True when the server acknowledged the binding.
        """
        if not self.is_connected or not self._connection_id:
            return False
        if self._udp_bound:
            return True

        try:
            host = urlparse(self._url).hostname or "realtime.michitai.com"
            # The WS port is a proxy port for wss:// — UDP needs the service
            # port announced by the server (3000 by default).
            port = udp_port or self._server_udp_port or 3000

            loop = asyncio.get_running_loop()
            transport, _ = await loop.create_datagram_endpoint(
                lambda: _UdpProtocol(self),
                remote_addr=(host, port),
            )
            self._udp_transport = transport  # type: ignore[assignment]

            bind_packet = _udp_encode(_UDP_BIND, self._connection_id, b'{"v":1}')
            # BIND is unreliable — retry once before giving up.
            for _ in range(2):
                transport.sendto(bind_packet)
                deadline = loop.time() + 1.5
                while not self._udp_bound and loop.time() < deadline:
                    await asyncio.sleep(0.025)
                if self._udp_bound:
                    break
            return self._udp_bound
        except Exception as exc:
            print(f"UDP connect failed: {exc}")
            return False

    async def send(
        self,
        target: RoomTargetPlayer,
        command: str,
        data: Any = None,
        target_ids: Optional[List[int]] = None,
    ) -> None:
        """Send a message to the specified players over the WebSocket.

        :param target: Target group (ALL, HOST, OTHERS, SPECIFIC).
        :param command: The command/type of the message.
        :param data: Optional payload.
        :param target_ids: Player IDs when ``target`` is SPECIFIC.
        """
        if not self.is_connected:
            return

        message = {
            "type": "send",
            "command": command,
            "data": data,
            "target_ids": target_ids or [],
            "target": target.value,
        }
        await self._ws.send(json.dumps(message))

    async def send_unreliable(
        self,
        target: RoomTargetPlayer,
        command: str,
        data: Any = None,
        target_ids: Optional[List[int]] = None,
    ) -> None:
        """Send over the bound UDP channel — unordered, unacknowledged,
        latest-wins via ``seq``. Best for movement/state streams.

        Falls back to the WebSocket when UDP is not bound.
        """
        if not self.is_udp_connected or not self._connection_id:
            await self.send(target, command, data, target_ids)
            return

        self._udp_seq += 1
        message = {
            "type": "send",
            "command": command,
            "data": data,
            "seq": self._udp_seq,
            "target_ids": target_ids or [],
            "target": target.value,
        }
        packet = _udp_encode(_UDP_MSG, self._connection_id,
                             json.dumps(message).encode("utf-8"))
        self._udp_transport.sendto(packet)  # type: ignore[union-attr]

    async def disconnect(self) -> None:
        """Disconnect from the server and clean up both channels."""
        for task in (self._listen_task, self._heartbeat_task):
            if task is not None:
                task.cancel()
        self._listen_task = None
        self._heartbeat_task = None
        self._udp_bound = False

        if self._udp_transport is not None:
            self._udp_transport.close()
            self._udp_transport = None

        try:
            if self._ws is not None and self._ws.open:
                await self._ws.close()
        except Exception as exc:
            print(f"Disconnect error: {exc}")
        finally:
            self._ws = None

    async def _listen(self) -> None:
        try:
            async for raw in self._ws:
                try:
                    message = from_dict(RealtimeMessage, json.loads(raw))
                except (json.JSONDecodeError, TypeError):
                    continue
                if message.type == "connected":
                    self._handle_connected(message.data)
                elif message.type == "receive" and self.on_receive:
                    result = self.on_receive(
                        message.command or "", message.data, message.sender
                    )
                    if inspect.isawaitable(result):
                        await result
        except asyncio.CancelledError:
            pass
        except Exception:
            pass

    def _handle_connected(self, data: Any) -> None:
        if isinstance(data, dict):
            self._connection_id = data.get("connection_id")
            port = data.get("udp_port")
            if isinstance(port, int):
                self._server_udp_port = port

    def _on_udp_datagram(self, data: bytes) -> None:
        decoded = _udp_decode(data)
        if decoded is None:
            return
        packet_type, connection_id, json_str = decoded
        if connection_id != self._connection_id:
            return

        if packet_type == _UDP_BIND_ACK:
            try:
                if json.loads(json_str or "{}").get("ok"):
                    self._udp_bound = True
            except ValueError:
                pass
        elif packet_type == _UDP_MSG:
            try:
                message = from_dict(RealtimeMessage, json.loads(json_str or "{}"))
            except (json.JSONDecodeError, TypeError):
                return
            if message.type == "receive" and self.on_receive:
                result = self.on_receive(
                    message.command or "", message.data, message.sender
                )
                if inspect.isawaitable(result):
                    asyncio.ensure_future(result)
        elif packet_type == _UDP_PONG:
            try:
                sent_ms = int(json.loads(json_str or "{}").get("t", 0))
                self.last_udp_rtt_ms = max(0, int(time.time() * 1000) - sent_ms)
            except (ValueError, TypeError):
                pass

    async def _heartbeat_loop(self) -> None:
        try:
            while self.is_connected:
                await asyncio.sleep(HEARTBEAT_INTERVAL)
                if self.is_connected:
                    await self._ws.send(json.dumps({"type": "heartbeat"}))
                    # Keep the NAT mapping warm on the UDP channel too.
                    if self.is_udp_connected and self._connection_id:
                        ping = _udp_encode(
                            _UDP_PING,
                            self._connection_id,
                            json.dumps({"t": int(time.time() * 1000)}).encode("utf-8"),
                        )
                        self._udp_transport.sendto(ping)  # type: ignore[union-attr]
        except asyncio.CancelledError:
            pass
        except Exception as exc:
            print(f"Heartbeat error: {exc}")

    @staticmethod
    def _wake_up_server() -> None:
        try:
            requests.get(DEFAULT_REALTIME_HTTP_URL, timeout=10)
        except requests.RequestException:
            pass
