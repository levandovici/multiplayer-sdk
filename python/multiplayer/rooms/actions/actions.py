"""Room actions: submit, poll completed, list pending, complete (host)."""

from __future__ import annotations

import dataclasses
from typing import Any, Optional, Type, TypeVar

from ..._serialization import from_dict
from ...client import Client
from ..enums import RoomTargetPlayers
from .enums import RoomCompleteActionStatus
from .models import (
    ActionCompleteResponse,
    ActionPendingResponse,
    ActionPollResponse,
    ActionSubmitResponse,
)

T = TypeVar("T")

# Endpoints
GAME_ROOM_ACTIONS = "game_room.php/actions"
GAME_ROOM_ACTIONS_POLL = "game_room.php/actions/poll"
GAME_ROOM_ACTIONS_PENDING = "game_room.php/actions/pending"
GAME_ROOM_ACTION_COMPLETE = "game_room.php/actions/{action_id}/complete"


class Actions:
    """Manage room actions between players and the host."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def submit(
        self,
        player_token: str,
        action_type: str,
        request_data: Any = None,
        target_players: RoomTargetPlayers = RoomTargetPlayers.ALL,
        target_players_ids: Optional[list] = None,
    ) -> ActionSubmitResponse:
        """Submit an action to target players in the room."""
        return self._client.send(
            "POST",
            self._client.player_url(GAME_ROOM_ACTIONS, player_token),
            {
                "target_players": target_players.value,
                "target_players_ids": target_players_ids,
                "action_type": action_type,
                "request_data": request_data,
            },
            ActionSubmitResponse,
        )

    def poll(
        self,
        player_token: str,
        data_cls: Optional[Type[T]] = None,
    ) -> ActionPollResponse:
        """Poll completed actions that were targeted to the current player.

        :param data_cls: Optional dataclass type for each action's ``response_data``.
        """
        response = self._client.send(
            "GET",
            self._client.player_url(GAME_ROOM_ACTIONS_POLL, player_token),
            None,
            ActionPollResponse,
        )
        _apply_type(response.actions, "response_data", data_cls)
        return response

    def get_pending(
        self,
        player_token: str,
        data_cls: Optional[Type[T]] = None,
    ) -> ActionPendingResponse:
        """Get pending actions to complete (host only).

        :param data_cls: Optional dataclass type for each action's ``request_data``.
        """
        response = self._client.send(
            "GET",
            self._client.player_url(GAME_ROOM_ACTIONS_PENDING, player_token),
            None,
            ActionPendingResponse,
        )
        _apply_type(response.actions, "request_data", data_cls)
        return response

    def complete(
        self,
        player_token: str,
        action_id: str,
        status: RoomCompleteActionStatus = RoomCompleteActionStatus.COMPLETED,
        response_data: Any = None,
    ) -> ActionCompleteResponse:
        """Mark an action as complete with an optional response (host only)."""
        return self._client.send(
            "POST",
            self._client.player_url(
                GAME_ROOM_ACTION_COMPLETE.format(action_id=action_id), player_token
            ),
            {"status": status.value, "response_data": response_data},
            ActionCompleteResponse,
        )


def _apply_type(items: Any, attr: str, cls: Optional[Type[T]]) -> None:
    if cls is None:
        return
    for item in items:
        value = getattr(item, attr)
        if dataclasses.is_dataclass(cls) and isinstance(value, dict):
            setattr(item, attr, from_dict(cls, value))
