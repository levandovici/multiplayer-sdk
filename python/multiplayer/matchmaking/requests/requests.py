"""Matchmaking join requests: request to join, approve/reject, status."""

from __future__ import annotations

from typing import Any, Optional

from ...client import Client
from ..models import MatchmakingCreateResponse, MatchmakingJoinRequestResponse
from .enums import MatchmakingRequestAction
from .models import (
    MatchmakingPermissionResponse,
    MatchmakingRequestStatusResponse,
)

# Endpoints
MATCHMAKING_CREATE = "matchmaking.php/create"
MATCHMAKING_REQUEST = "matchmaking.php/{matchmaking_id}/request"
MATCHMAKING_RESPONSE = "matchmaking.php/{request_id}/response"
MATCHMAKING_REQUEST_STATUS = "matchmaking.php/{request_id}/status"


class MatchmakingRequests:
    """Manage matchmaking lobbies that join by host approval."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def create(
        self,
        player_token: str,
        matchmaking_name: str,
        max_players: int = 4,
        strict_full: bool = False,
        join_by_requests: bool = False,
        host_switch: bool = False,
        can_leave_room: bool = False,
        realtime_room: bool = False,
        password: Optional[str] = None,
        player_data: Any = None,
        rules: Any = None,
    ) -> MatchmakingCreateResponse:
        """Create a matchmaking lobby, optionally requiring join approval."""
        return self._client.send(
            "POST",
            self._client.player_url(MATCHMAKING_CREATE, player_token),
            {
                "matchmaking_name": matchmaking_name,
                "max_players": max_players,
                "strict_full": strict_full,
                "join_by_requests": join_by_requests,
                "host_switch": host_switch,
                "can_leave_room": can_leave_room,
                "realtime_room": realtime_room,
                "password": password,
                "player_data": player_data,
                "rules": rules,
            },
            MatchmakingCreateResponse,
        )

    def request_to_join(
        self,
        player_token: str,
        matchmaking_id: str,
        player_data: Any = None,
    ) -> MatchmakingJoinRequestResponse:
        """Request to join an existing matchmaking lobby."""
        return self._client.send(
            "POST",
            self._client.player_url(
                MATCHMAKING_REQUEST.format(matchmaking_id=matchmaking_id),
                player_token,
            ),
            player_data,
            MatchmakingJoinRequestResponse,
        )

    def respond(
        self,
        player_token: str,
        request_id: str,
        action: MatchmakingRequestAction,
    ) -> MatchmakingPermissionResponse:
        """Approve or reject a pending join request (host only)."""
        return self._client.send(
            "POST",
            self._client.player_url(
                MATCHMAKING_RESPONSE.format(request_id=request_id), player_token
            ),
            {"action": action.value},
            MatchmakingPermissionResponse,
        )

    def check_status(
        self,
        player_token: str,
        request_id: str,
    ) -> MatchmakingRequestStatusResponse:
        """Check the status of a join request."""
        return self._client.send(
            "GET",
            self._client.player_url(
                MATCHMAKING_REQUEST_STATUS.format(request_id=request_id),
                player_token,
            ),
            None,
            MatchmakingRequestStatusResponse,
        )
