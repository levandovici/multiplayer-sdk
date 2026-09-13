"""Leaderboard queries."""

from __future__ import annotations

import dataclasses
from typing import List, Optional, Type, TypeVar

from .._serialization import from_dict
from ..client import Client
from .models import LeaderboardResponse

T = TypeVar("T")

LEADERBOARD = "leaderboard.php"


class Leaderboard:
    """Query ranked leaderboards based on player data fields."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def get(
        self,
        sort_by: List[str],
        limit: int = 10,
        data_cls: Optional[Type[T]] = None,
    ) -> LeaderboardResponse:
        """Retrieve the leaderboard.

        :param sort_by: Player-data field names to sort by (e.g. ``["level", "wins"]``).
        :param limit: Maximum results (1-100, default 10).
        :param data_cls: Optional dataclass type for each entry's ``player_data``.
        """
        response = self._client.send(
            "POST",
            self._client.url(LEADERBOARD),
            {"sort_by": sort_by, "limit": limit},
            LeaderboardResponse,
        )
        if data_cls is not None and dataclasses.is_dataclass(data_cls):
            for entry in response.leaderboard:
                if isinstance(entry.player_data, dict):
                    entry.player_data = from_dict(data_cls, entry.player_data)
        return response
