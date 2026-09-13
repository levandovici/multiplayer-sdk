"""Models for the leaderboard API."""

from __future__ import annotations

import enum
from dataclasses import dataclass, field
from typing import Any, ClassVar, List, Type

from ..api_response import ApiResponse
from ..errors.enums import LeaderboardError


@dataclass
class LeaderboardPlayer:
    """A player on the leaderboard."""

    rank: int = 0
    player_id: int = 0
    player_name: str = ""
    player_data: Any = None


@dataclass
class LeaderboardResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = LeaderboardError

    leaderboard: List[LeaderboardPlayer] = field(default_factory=list)
    total: int = 0
    sort_by: List[str] = field(default_factory=list)
    limit: int = 0
