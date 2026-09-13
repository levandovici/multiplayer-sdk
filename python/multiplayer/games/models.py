"""Models for game-level operations."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass, field
from typing import Any, ClassVar, List, Optional, Type

from ..api_response import ApiResponse
from ..errors.enums import GameDataGameGetError, PlayerListError


@dataclass
class PlayerShort:
    """Basic info about a player in the game."""

    id: int = 0
    player_name: str = ""
    is_online: bool = False
    last_login: Optional[datetime] = None
    created_at: Optional[datetime] = None


@dataclass
class PlayerListResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = PlayerListError

    count: int = 0
    players: List[PlayerShort] = field(default_factory=list)


@dataclass
class GameDataResponse(ApiResponse):
    """Response containing global game data."""

    error_enum: ClassVar[Type[enum.Enum]] = GameDataGameGetError

    type: str = ""
    game_id: int = 0
    data: Any = None
