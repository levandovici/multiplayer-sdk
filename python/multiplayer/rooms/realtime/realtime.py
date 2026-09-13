"""Realtime WebSocket client for game rooms.

Requires the optional ``websockets`` package::

    pip install websockets

Usage::

    realtime = Realtime()
    token = await realtime.get_token(client, player_token)
    await realtime.connect(token.token)

    realtime.on_receive = lambda command, data, sender: print(command, data)
    await realtime.send(RoomTargetPlayer.ALL, "move", {"x": 1, "y": 2})
"""

from __future__ import annotations

import asyncio
import inspect
import json
from typing import Any, Callable, List, Optional

import requests

from ..._serialization import from_dict
from ...client import Client
from .enums import RoomTargetPlayer
from .models import RealtimeMessage, SenderInfo, TokenResponse

REALTIME_TOKEN = "realtime.php/token"
DEFAULT_REALTIME_WS_URL = "wss://realtime.michitai.com"
DEFAULT_REALTIME_HTTP_URL = "https://realtime.michitai.com/"

HEARTBEAT_INTERVAL = 20  # seconds

OnReceiveCallback = Callable[[str, Any, Optional[SenderInfo]], Any]
OnConnectedCallback = Callable[[], Any]


def get_token(client: Client, player_token: str) -> TokenResponse:
    """Retrieve a realtime authentication token for WebSocket connections.

    :param client: The API client instance.
    :param player_token: The player's private authentication token.
    """
    return client.send(
        "POST",
        client.player_url(REALTIME_TOKEN, player_token),
        None,
        TokenResponse,
    )


class Realtime:
    """Manages a WebSocket connection for realtime room communication.

    Async API built on ``asyncio`` + ``websockets``. Set :attr:`on_receive`
    and :attr:`on_connected` to subscribe to events.
    """

    def __init__(self, realtime_websocket_url: str = DEFAULT_REALTIME_WS_URL) -> None:
        self._url = realtime_websocket_url
        self._ws: Any = None
        self._token: Optional[str] = None
        self._listen_task: Optional[asyncio.Task] = None
        self._heartbeat_task: Optional[asyncio.Task] = None

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

            self._ws = await websockets.connect(
                f"{self._url}?token={self._token}&client=json"
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

    async def send(
        self,
        target: RoomTargetPlayer,
        command: str,
        data: Any = None,
        target_ids: Optional[List[int]] = None,
    ) -> None:
        """Send a message to the specified players.

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

    async def disconnect(self) -> None:
        """Disconnect from the server and clean up tasks."""
        for task in (self._listen_task, self._heartbeat_task):
            if task is not None:
                task.cancel()
        self._listen_task = None
        self._heartbeat_task = None

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
                if message.type == "receive" and self.on_receive:
                    result = self.on_receive(
                        message.command or "", message.data, message.sender
                    )
                    if inspect.isawaitable(result):
                        await result
        except asyncio.CancelledError:
            pass
        except Exception:
            pass

    async def _heartbeat_loop(self) -> None:
        try:
            while self.is_connected:
                await asyncio.sleep(HEARTBEAT_INTERVAL)
                if self.is_connected:
                    await self._ws.send(json.dumps({"type": "heartbeat"}))
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
