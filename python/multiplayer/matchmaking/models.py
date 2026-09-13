"""Request/response models for matchmaking lobbies."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass, field
from typing import Any, ClassVar, List, Optional, Type

from ..api_response import ApiResponse
from ..errors.enums import (
    MatchmakingCreateError,
    MatchmakingCurrentError,
    MatchmakingHeartbeatError,
    MatchmakingJoinError,
    MatchmakingKickError,
    MatchmakingLeaveError,
    MatchmakingListError,
    MatchmakingPasswordError,
    MatchmakingPlayersError,
    MatchmakingRemoveError,
    MatchmakingRequestError,
    MatchmakingStartError,
    MatchmakingStopError,
)


@dataclass
class MatchmakingCreateResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingCreateError

    matchmaking_id: str = ""
    matchmaking_name: str = ""
    max_players: int = 0
    strict_full: bool = False
    join_by_requests: bool = False
    host_switch: bool = False
    can_leave_room: bool = False
    realtime_room: bool = False
    is_host: bool = False


@dataclass
class MatchmakingRequestBase:
    """Summary of a join request."""

    request_id: str = ""
    matchmaking_id: str = ""
    status: str = ""
    requested_at: Optional[datetime] = None
    responded_at: Optional[datetime] = None


@dataclass
class MatchmakingInfo:
    """Status of the matchmaking lobby the player is in."""

    matchmaking_id: str = ""
    matchmaking_name: str = ""
    is_host: bool = False
    max_players: int = 0
    current_players: int = 0
    strict_full: bool = False
    join_by_requests: bool = False
    host_switch: bool = False
    can_leave_room: bool = False
    realtime_room: bool = False
    has_password: bool = False
    joined_at: Optional[datetime] = None
    is_online: bool = False
    last_heartbeat: Optional[datetime] = None
    lobby_heartbeat: Optional[datetime] = None
    is_started: bool = False
    started_at: Optional[datetime] = None
    rules: Any = None


@dataclass
class MatchmakingCurrentResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingCurrentError

    in_matchmaking: bool = False
    matchmaking: Optional[MatchmakingInfo] = None
    pending_requests: List[MatchmakingRequestBase] = field(default_factory=list)


@dataclass
class MatchmakingDirectJoinResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingJoinError

    message: str = ""
    matchmaking_id: str = ""


@dataclass
class MatchmakingJoinRequestResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingRequestError

    request_id: str = ""
    message: str = ""


@dataclass
class MatchmakingLeaveResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingLeaveError

    message: str = ""


@dataclass
class MatchmakingLobby:
    """Summary information about a matchmaking lobby."""

    matchmaking_id: str = ""
    matchmaking_name: str = ""
    host_player_id: int = 0
    max_players: int = 0
    strict_full: bool = False
    join_by_requests: bool = False
    host_switch: bool = False
    can_leave_room: bool = False
    realtime_room: bool = False
    has_password: bool = False
    created_at: Optional[datetime] = None
    last_heartbeat: Optional[datetime] = None
    current_players: int = 0
    host_name: str = ""
    rules: Any = None


@dataclass
class MatchmakingListResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingListError

    lobbies: List[MatchmakingLobby] = field(default_factory=list)


@dataclass
class MatchmakingPlayer:
    """A player inside a matchmaking lobby."""

    player_id: int = 0
    is_local: bool = False
    joined_at: Optional[datetime] = None
    last_heartbeat: Optional[datetime] = None
    is_online: bool = False
    player_name: str = ""
    seconds_since_heartbeat: int = 0
    is_host: bool = False
    player_data: Any = None


@dataclass
class MatchmakingPlayersResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingPlayersError

    players: List[MatchmakingPlayer] = field(default_factory=list)


@dataclass
class MatchmakingHeartbeatResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingHeartbeatError

    status: Optional[str] = None


@dataclass
class MatchmakingRemoveResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingRemoveError

    message: str = ""


@dataclass
class MatchmakingStartResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingStartError

    room_id: str = ""
    room_name: str = ""
    players_transferred: int = 0
    message: str = ""


@dataclass
class MatchmakingStopResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingStopError

    message: str = ""


@dataclass
class MatchmakingKickResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingKickError

    message: str = ""
    kicked_player_id: int = 0


@dataclass
class MatchmakingPasswordResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingPasswordError

    message: str = ""
