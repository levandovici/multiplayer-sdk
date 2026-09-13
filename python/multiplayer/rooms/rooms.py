"""Game room management: create, join, leave, players, heartbeat, admin ops."""

from __future__ import annotations

import dataclasses
from typing import Any, Optional, Type, TypeVar

from .._serialization import from_dict
from ..client import Client
from .models import (
    CurrentRoomResponse,
    HeartbeatResponse,
    RoomCreateResponse,
    RoomJoinResponse,
    RoomKickResponse,
    RoomLeaveResponse,
    RoomListResponse,
    RoomPasswordResponse,
    RoomPlayersResponse,
    RoomStopResponse,
)

T = TypeVar("T")

# Endpoints
GAME_ROOM_CREATE = "game_room.php/create"
GAME_ROOM_LIST = "game_room.php/list"
GAME_ROOM_JOIN = "game_room.php/{room_id}/join"
GAME_ROOM_LEAVE = "game_room.php/leave"
GAME_ROOM_PLAYERS = "game_room.php/players"
GAME_ROOM_HEARTBEAT = "game_room.php/heartbeat"
GAME_ROOM_CURRENT = "game_room.php/current"
GAME_ROOM_STOP = "game_room.php/stop"
GAME_ROOM_KICK = "game_room.php/kick"
GAME_ROOM_PASSWORD = "game_room.php/password"


class Rooms:
    """Game room lifecycle and player management."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def create(
        self,
        player_token: str,
        room_name: str,
        max_players: int = 4,
        password: Optional[str] = None,
        host_switch: bool = False,
        realtime: bool = False,
        player_data: Any = None,
        rules: Any = None,
    ) -> RoomCreateResponse:
        """Create a new game room.

        :param player_token: The host's player token.
        :param room_name: Name of the game room.
        :param max_players: Maximum players allowed (default 4).
        :param password: Optional room password.
        :param host_switch: Whether host switching is allowed.
        :param realtime: Whether the room supports realtime communication.
        :param player_data: Optional player data for the host.
        :param rules: Optional game rules for the room.
        """
        return self._client.send(
            "POST",
            self._client.player_url(GAME_ROOM_CREATE, player_token),
            {
                "room_name": room_name,
                "max_players": max_players,
                "password": password,
                "host_switch": host_switch,
                "realtime": realtime,
                "player_data": player_data,
                "rules": rules,
            },
            RoomCreateResponse,
        )

    def list(
        self,
        search: Optional[str] = None,
        limit: Optional[int] = None,
        rules_cls: Optional[Type[T]] = None,
    ) -> RoomListResponse:
        """List available game rooms.

        :param rules_cls: Optional dataclass type for each room's ``rules``.
        """
        response = self._client.send(
            "POST",
            self._client.url(GAME_ROOM_LIST),
            {"search": search, "limit": limit},
            RoomListResponse,
        )
        _apply_type((r for r in response.rooms), "rules", rules_cls)
        return response

    def join(
        self,
        player_token: str,
        room_id: str,
        password: Optional[str] = None,
        player_data: Any = None,
    ) -> RoomJoinResponse:
        """Join an existing game room."""
        body = None
        if password is not None or player_data is not None:
            body = {"password": password, "player_data": player_data}
        return self._client.send(
            "POST",
            self._client.player_url(
                GAME_ROOM_JOIN.format(room_id=room_id), player_token
            ),
            body,
            RoomJoinResponse,
        )

    def leave(self, player_token: str) -> RoomLeaveResponse:
        """Leave the current game room."""
        return self._client.send(
            "POST",
            self._client.player_url(GAME_ROOM_LEAVE, player_token),
            None,
            RoomLeaveResponse,
        )

    def get_players(
        self,
        player_token: str,
        data_cls: Optional[Type[T]] = None,
    ) -> RoomPlayersResponse:
        """Get the list of players in the current room.

        :param data_cls: Optional dataclass type for each player's ``player_data``.
        """
        response = self._client.send(
            "GET",
            self._client.player_url(GAME_ROOM_PLAYERS, player_token),
            None,
            RoomPlayersResponse,
        )
        _apply_type((p for p in response.players), "player_data", data_cls)
        return response

    def heartbeat(self, player_token: str) -> HeartbeatResponse:
        """Send a heartbeat to maintain presence in the room."""
        return self._client.send(
            "POST",
            self._client.player_url(GAME_ROOM_HEARTBEAT, player_token),
            None,
            HeartbeatResponse,
        )

    def get_current(
        self,
        player_token: str,
        rules_cls: Optional[Type[T]] = None,
    ) -> CurrentRoomResponse:
        """Get detailed info about the current room incl. pending items.

        :param rules_cls: Optional dataclass type for the room's ``rules``.
        """
        response = self._client.send(
            "GET",
            self._client.player_url(GAME_ROOM_CURRENT, player_token),
            None,
            CurrentRoomResponse,
        )
        if rules_cls is not None and response.room is not None:
            response.room.rules = _convert(response.room.rules, rules_cls)
        return response

    def stop(self, player_token: str) -> RoomStopResponse:
        """Stop the current game room and remove all its data (host only)."""
        return self._client.send(
            "POST",
            self._client.player_url(GAME_ROOM_STOP, player_token),
            None,
            RoomStopResponse,
        )

    def kick(self, player_token: str, player_id: int) -> RoomKickResponse:
        """Kick a player from the room (host only). Cannot kick yourself."""
        return self._client.send(
            "POST",
            self._client.player_url(GAME_ROOM_KICK, player_token),
            {"player_id": player_id},
            RoomKickResponse,
        )

    def update_password(
        self,
        player_token: str,
        password: Optional[str] = None,
    ) -> RoomPasswordResponse:
        """Update the room password (host only). Pass ``None`` to remove it."""
        return self._client.send(
            "POST",
            self._client.player_url(GAME_ROOM_PASSWORD, player_token),
            {"password": password},
            RoomPasswordResponse,
        )


def _convert(value: Any, cls: Type[T]) -> Any:
    if dataclasses.is_dataclass(cls) and isinstance(value, dict):
        return from_dict(cls, value)
    return value


def _apply_type(items: Any, attr: str, cls: Optional[Type[T]]) -> None:
    if cls is None:
        return
    for item in items:
        setattr(item, attr, _convert(getattr(item, attr), cls))
