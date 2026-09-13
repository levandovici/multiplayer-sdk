"""Request/response models for the Rooms module."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass, field
from typing import Any, ClassVar, List, Optional, Type

from ..api_response import ApiResponse
from ..errors.enums import (
    RoomCreateError,
    RoomCurrentError,
    RoomHeartbeatError,
    RoomJoinError,
    RoomKickError,
    RoomLeaveError,
    RoomListError,
    RoomPasswordError,
    RoomPlayersError,
    RoomStopError,
)


@dataclass
class RoomCreateResponse(ApiResponse):
    """Response containing the created room ID."""

    error_enum: ClassVar[Type[enum.Enum]] = RoomCreateError

    room_id: str = ""
    room_name: str = ""
    realtime: bool = False
    is_host: bool = False


@dataclass
class RoomShort:
    """Summary information about a game room."""

    room_id: str = ""
    room_name: str = ""
    max_players: int = 0
    current_players: int = 0
    has_password: bool = False
    host_switch: bool = False
    can_leave: bool = False
    realtime: bool = False
    rules: Any = None


@dataclass
class RoomListResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomListError

    rooms: List[RoomShort] = field(default_factory=list)


@dataclass
class RoomJoinResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomJoinError

    room_id: str = ""
    message: str = ""


@dataclass
class RoomLeaveResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomLeaveError

    message: str = ""


@dataclass
class RoomPlayer:
    """A player inside a game room."""

    player_id: int = 0
    is_local: bool = False
    player_name: str = ""
    is_host: bool = False
    is_online: bool = False
    last_heartbeat: Optional[datetime] = None
    player_data: Any = None


@dataclass
class RoomPlayersResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomPlayersError

    players: List[RoomPlayer] = field(default_factory=list)
    last_updated: str = ""


@dataclass
class HeartbeatResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomHeartbeatError

    status: str = ""


@dataclass
class CurrentRoomInfo:
    """Detailed information about the room the player is currently in."""

    room_id: str = ""
    room_name: str = ""
    is_host: bool = False
    is_online: bool = False
    max_players: int = 0
    current_players: int = 0
    has_password: bool = False
    host_switch: bool = False
    can_leave: bool = False
    realtime: bool = False
    is_active: bool = False
    player_name: str = ""
    joined_at: Optional[datetime] = None
    last_heartbeat: Optional[datetime] = None
    room_created_at: Optional[datetime] = None
    room_last_activity: Optional[datetime] = None
    rules: Any = None


@dataclass
class CurrentRoomResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomCurrentError

    in_room: bool = False
    room: Optional[CurrentRoomInfo] = None
    pending_actions: Optional[List[Any]] = None
    pending_updates: Optional[List[Any]] = None


@dataclass
class RoomKickResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomKickError

    message: str = ""
    kicked_player_id: int = 0


@dataclass
class RoomStopResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomStopError

    message: str = ""


@dataclass
class RoomPasswordResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = RoomPasswordError

    message: str = ""
