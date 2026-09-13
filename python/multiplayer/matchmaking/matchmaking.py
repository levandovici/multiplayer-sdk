"""Matchmaking lobby management: create, join, players, start, admin ops."""

from __future__ import annotations

import dataclasses
from typing import Any, Optional, Type, TypeVar

from .._serialization import from_dict
from ..client import Client
from .models import (
    MatchmakingCreateResponse,
    MatchmakingCurrentResponse,
    MatchmakingDirectJoinResponse,
    MatchmakingHeartbeatResponse,
    MatchmakingKickResponse,
    MatchmakingLeaveResponse,
    MatchmakingListResponse,
    MatchmakingPasswordResponse,
    MatchmakingPlayersResponse,
    MatchmakingRemoveResponse,
    MatchmakingStartResponse,
    MatchmakingStopResponse,
)

T = TypeVar("T")

# Endpoints
MATCHMAKING_LIST = "matchmaking.php/list"
MATCHMAKING_CREATE = "matchmaking.php/create"
MATCHMAKING_CURRENT = "matchmaking.php/current"
MATCHMAKING_JOIN = "matchmaking.php/{matchmaking_id}/join"
MATCHMAKING_LEAVE = "matchmaking.php/leave"
MATCHMAKING_PLAYERS = "matchmaking.php/players"
MATCHMAKING_HEARTBEAT = "matchmaking.php/heartbeat"
MATCHMAKING_REMOVE = "matchmaking.php/remove"
MATCHMAKING_START = "matchmaking.php/start"
MATCHMAKING_STOP = "matchmaking.php/stop"
MATCHMAKING_KICK = "matchmaking.php/kick"
MATCHMAKING_PASSWORD = "matchmaking.php/password"


class Matchmaking:
    """Matchmaking lobby management."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def list(
        self,
        search: Optional[str] = None,
        limit: Optional[int] = None,
        rules_cls: Optional[Type[T]] = None,
    ) -> MatchmakingListResponse:
        """List available matchmaking lobbies.

        :param rules_cls: Optional dataclass type for each lobby's ``rules``.
        """
        response = self._client.send(
            "POST",
            self._client.url(MATCHMAKING_LIST),
            {"search": search, "limit": limit},
            MatchmakingListResponse,
        )
        _apply_type(response.lobbies, "rules", rules_cls)
        return response

    def create(
        self,
        player_token: str,
        matchmaking_name: str,
        max_players: int = 4,
        strict_full: bool = False,
        host_switch: bool = False,
        can_leave_room: bool = False,
        realtime_room: bool = False,
        password: Optional[str] = None,
        player_data: Any = None,
        rules: Any = None,
    ) -> MatchmakingCreateResponse:
        """Create a new matchmaking lobby (direct join, no approval needed).

        To require host approval for joins, use
        :meth:`client.matchmaking_requests.create` with ``join_by_requests=True``.
        """
        return self._client.send(
            "POST",
            self._client.player_url(MATCHMAKING_CREATE, player_token),
            {
                "matchmaking_name": matchmaking_name,
                "max_players": max_players,
                "strict_full": strict_full,
                "join_by_requests": False,
                "host_switch": host_switch,
                "can_leave_room": can_leave_room,
                "realtime_room": realtime_room,
                "password": password,
                "player_data": player_data,
                "rules": rules,
            },
            MatchmakingCreateResponse,
        )

    def get_current(
        self,
        player_token: str,
        rules_cls: Optional[Type[T]] = None,
    ) -> MatchmakingCurrentResponse:
        """Get the status of the player's current matchmaking lobby.

        :param rules_cls: Optional dataclass type for the lobby's ``rules``.
        """
        response = self._client.send(
            "GET",
            self._client.player_url(MATCHMAKING_CURRENT, player_token),
            None,
            MatchmakingCurrentResponse,
        )
        if rules_cls is not None and response.matchmaking is not None:
            response.matchmaking.rules = _convert(
                response.matchmaking.rules, rules_cls
            )
        return response

    def join(
        self,
        player_token: str,
        matchmaking_id: str,
        player_data: Any = None,
    ) -> MatchmakingDirectJoinResponse:
        """Join a matchmaking lobby directly (no host approval)."""
        return self._client.send(
            "POST",
            self._client.player_url(
                MATCHMAKING_JOIN.format(matchmaking_id=matchmaking_id), player_token
            ),
            player_data,
            MatchmakingDirectJoinResponse,
        )

    def leave(self, player_token: str) -> MatchmakingLeaveResponse:
        """Leave the current matchmaking lobby."""
        return self._client.send(
            "POST",
            self._client.player_url(MATCHMAKING_LEAVE, player_token),
            None,
            MatchmakingLeaveResponse,
        )

    def get_players(
        self,
        player_token: str,
        data_cls: Optional[Type[T]] = None,
    ) -> MatchmakingPlayersResponse:
        """Get the list of players in the current lobby.

        :param data_cls: Optional dataclass type for each player's ``player_data``.
        """
        response = self._client.send(
            "GET",
            self._client.player_url(MATCHMAKING_PLAYERS, player_token),
            None,
            MatchmakingPlayersResponse,
        )
        _apply_type(response.players, "player_data", data_cls)
        return response

    def heartbeat(self, player_token: str) -> MatchmakingHeartbeatResponse:
        """Send a heartbeat to maintain presence in the lobby."""
        return self._client.send(
            "POST",
            self._client.player_url(MATCHMAKING_HEARTBEAT, player_token),
            None,
            MatchmakingHeartbeatResponse,
        )

    def remove(self, player_token: str) -> MatchmakingRemoveResponse:
        """Remove the current matchmaking lobby (host only)."""
        return self._client.send(
            "POST",
            self._client.player_url(MATCHMAKING_REMOVE, player_token),
            None,
            MatchmakingRemoveResponse,
        )

    def start(self, player_token: str) -> MatchmakingStartResponse:
        """Start the game: creates a game room and transfers all players."""
        return self._client.send(
            "POST",
            self._client.player_url(MATCHMAKING_START, player_token),
            None,
            MatchmakingStartResponse,
        )

    def stop(self, player_token: str) -> MatchmakingStopResponse:
        """Stop the current lobby (host only, before the game started)."""
        return self._client.send(
            "POST",
            self._client.player_url(MATCHMAKING_STOP, player_token),
            None,
            MatchmakingStopResponse,
        )

    def kick(self, player_token: str, player_id: int) -> MatchmakingKickResponse:
        """Kick a player from the lobby (host only, before the game started)."""
        return self._client.send(
            "POST",
            self._client.player_url(MATCHMAKING_KICK, player_token),
            {"player_id": player_id},
            MatchmakingKickResponse,
        )

    def update_password(
        self,
        player_token: str,
        password: Optional[str] = None,
    ) -> MatchmakingPasswordResponse:
        """Update the lobby password (host only, before the game started)."""
        return self._client.send(
            "POST",
            self._client.player_url(MATCHMAKING_PASSWORD, player_token),
            {"password": password},
            MatchmakingPasswordResponse,
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
