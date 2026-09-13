"""Request/response models for room updates."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass, field
from typing import Any, ClassVar, List, Optional, Type

from ...api_response import ApiResponse
from ...errors.enums import RoomUpdatesError, RoomUpdatesPollError
from ..enums import RoomTargetPlayers


@dataclass
class UpdatePlayers:
    """Parameters for sending an update to players in the room.

    :param target_players: Which players to send the update to.
    :param type: The type of update.
    :param data: The update payload.
    :param target_players_ids: Player IDs when ``target_players`` is SPECIFIC.
    """

    target_players: RoomTargetPlayers = RoomTargetPlayers.ALL
    type: str = ""
    data: Any = None
    target_players_ids: Optional[List[int]] = None


@dataclass
class PollUpdates:
    """Parameters for polling updates.

    :param from_players: Which players to receive updates from.
    :param from_players_ids: Player IDs when ``from_players`` is SPECIFIC.
    :param last_update: Only receive updates after this update ID.
    """

    from_players: RoomTargetPlayers = RoomTargetPlayers.HOST
    from_players_ids: Optional[List[int]] = None
    last_update: Optional[str] = None


@dataclass
class PlayerUpdate:
    """An update received from another player in the room."""

    update_id: str = ""
    from_player_id: int = 0
    type: str = ""
    created_at: Optional[datetime] = None
    data: Any = None


@dataclass
class UpdatePlayersResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomUpdatesError

    updates_sent: int = 0
    update_ids: List[str] = field(default_factory=list)
    target_players_ids: List[int] = field(default_factory=list)


@dataclass
class PollUpdatesResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomUpdatesPollError

    updates: List[PlayerUpdate] = field(default_factory=list)
    last_update: str = ""
