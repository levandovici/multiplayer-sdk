"""Request/response models for room actions."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass, field
from typing import Any, ClassVar, List, Optional, Type

from ...api_response import ApiResponse
from ...errors.enums import (
    RoomActionsCompleteError,
    RoomActionsError,
    RoomActionsPendingError,
    RoomActionsPollError,
)
from ..enums import RoomTargetPlayers
from .enums import RoomCompleteActionStatus


@dataclass
class SubmitAction:
    """Parameters for submitting an action to players in the room.

    :param target_players: Which players to target.
    :param action_type: The type of action being submitted.
    :param request_data: The request payload for the action.
    :param target_players_ids: Player IDs when ``target_players`` is SPECIFIC.
    """

    target_players: RoomTargetPlayers = RoomTargetPlayers.ALL
    action_type: str = ""
    request_data: Any = None
    target_players_ids: Optional[List[int]] = None


@dataclass
class ActionComplete:
    """Parameters for completing an action (host only).

    :param status: Completion status.
    :param response_data: Payload returned to the action's requester.
    """

    status: RoomCompleteActionStatus = RoomCompleteActionStatus.COMPLETED
    response_data: Any = None


@dataclass
class ActionInfo:
    """A completed action targeted at the current player."""

    action_id: str = ""
    action_type: str = ""
    is_host: bool = False
    response_data: Any = None


@dataclass
class PendingAction:
    """An action awaiting completion by the host."""

    action_id: str = ""
    player_id: int = 0
    target_id: int = 0
    action_type: str = ""
    created_at: Optional[datetime] = None
    player_name: str = ""
    is_host: bool = False
    request_data: Any = None


@dataclass
class ActionSubmitResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomActionsError

    actions_sent: int = 0
    action_ids: List[str] = field(default_factory=list)
    target_players_ids: List[int] = field(default_factory=list)


@dataclass
class ActionPollResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomActionsPollError

    actions: List[ActionInfo] = field(default_factory=list)


@dataclass
class ActionPendingResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomActionsPendingError

    actions: List[PendingAction] = field(default_factory=list)


@dataclass
class ActionCompleteResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomActionsCompleteError

    message: str = ""
