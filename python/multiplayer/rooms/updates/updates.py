"""Room updates: send updates to players and poll received updates."""

from __future__ import annotations

import dataclasses
from typing import Any, List, Optional, Type, TypeVar

from ..._serialization import from_dict
from ...client import Client
from ..enums import RoomTargetPlayers
from .models import PollUpdatesResponse, UpdatePlayersResponse

T = TypeVar("T")

# Endpoints
GAME_ROOM_UPDATES = "game_room.php/updates"
GAME_ROOM_UPDATES_POLL = "game_room.php/updates/poll"


class Updates:
    """Send and receive room updates (polling-based state sync)."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def send(
        self,
        player_token: str,
        type: str,
        data: Any = None,
        target_players: RoomTargetPlayers = RoomTargetPlayers.ALL,
        target_players_ids: Optional[List[int]] = None,
    ) -> UpdatePlayersResponse:
        """Send an update to specific players in the room.

        :param type: The update type (e.g. ``"game_start"``).
        :param data: The update payload.
        :param target_players: Which players to target.
        :param target_players_ids: Player IDs when targeting SPECIFIC.
        """
        return self._client.send(
            "POST",
            self._client.player_url(GAME_ROOM_UPDATES, player_token),
            {
                "target_players": target_players.value,
                "target_players_ids": target_players_ids,
                "type": type,
                "data": data,
            },
            UpdatePlayersResponse,
        )

    def poll(
        self,
        player_token: str,
        from_players: RoomTargetPlayers = RoomTargetPlayers.HOST,
        from_players_ids: Optional[List[int]] = None,
        last_update: Optional[str] = None,
        data_cls: Optional[Type[T]] = None,
    ) -> PollUpdatesResponse:
        """Poll updates that were sent to the current player.

        :param from_players: Which players to receive updates from.
        :param from_players_ids: Player IDs when ``from_players`` is SPECIFIC.
        :param last_update: Only receive updates after this update ID.
        :param data_cls: Optional dataclass type for each update's ``data``.
        """
        response = self._client.send(
            "POST",
            self._client.player_url(GAME_ROOM_UPDATES_POLL, player_token),
            {
                "from_players": from_players.value,
                "from_players_ids": from_players_ids,
                "last_update": last_update,
            },
            PollUpdatesResponse,
        )
        if data_cls is not None:
            for update in response.updates:
                if dataclasses.is_dataclass(data_cls) and isinstance(update.data, dict):
                    update.data = from_dict(data_cls, update.data)
        return response
