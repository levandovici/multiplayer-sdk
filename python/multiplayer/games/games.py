"""Game-level operations: player list and global game data."""

from __future__ import annotations

import dataclasses
from typing import Any, Optional, Type, TypeVar

from .._serialization import from_dict
from ..api_response import SuccessResponse
from ..client import Client
from .models import GameDataResponse, PlayerListResponse

T = TypeVar("T")

# Endpoints
GAME_PLAYERS_LIST = "game_players.php/list"
GAME_DATA_GAME_GET = "game_data.php/game/get"
GAME_DATA_GAME_UPDATE = "game_data.php/game/update"


class Games:
    """Game-level operations (mostly admin — require the private token)."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def get_all_players(self) -> PlayerListResponse:
        """List all players in the game. Requires the private API token."""
        return self._client.send(
            "GET",
            self._client.private_url(GAME_PLAYERS_LIST),
            None,
            PlayerListResponse,
        )

    def get_data(self, data_cls: Optional[Type[T]] = None) -> GameDataResponse:
        """Retrieve global game data.

        :param data_cls: Optional dataclass type to deserialize ``data`` into.
        """
        response = self._client.send(
            "GET",
            self._client.url(GAME_DATA_GAME_GET),
            None,
            GameDataResponse,
        )
        if (
            data_cls is not None
            and dataclasses.is_dataclass(data_cls)
            and isinstance(response.data, dict)
        ):
            response.data = from_dict(data_cls, response.data)
        return response

    def update_data(self, data: Any) -> SuccessResponse:
        """Update global game data. Requires the private API token."""
        return self._client.send(
            "PUT",
            self._client.private_url(GAME_DATA_GAME_UPDATE),
            data,
            SuccessResponse,
        )
