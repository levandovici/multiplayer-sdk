"""Request/response models for the Players module."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass, field
from typing import Any, ClassVar, Optional, Type

from ..api_response import ApiResponse
from ..errors.enums import (
    CommonError,
    GameDataPlayerGetError,
    PlayerBanError,
    PlayerHeartbeatError,
    PlayerLoginError,
    PlayerLogoutError,
    PlayerRegisterError,
    PlayerRenameError,
    PlayerUnbanError,
)


@dataclass
class PlayerInfo:
    """Full player information returned by authentication."""

    id: int = 0
    game_id: int = 0
    player_name: str = ""
    player_data: Any = None
    is_online: bool = False
    last_login: Optional[datetime] = None
    last_logout: Optional[datetime] = None
    last_heartbeat: Optional[datetime] = None
    created_at: Optional[datetime] = None


@dataclass
class PlayerRegisterResponse(ApiResponse):
    """Response containing the new player's ID and private key token."""

    error_enum: ClassVar[Type[enum.Enum]] = PlayerRegisterError

    player_id: int = 0
    private_key: str = ""
    player_name: str = ""
    game_id: int = 0


@dataclass
class PlayerAuthResponse(ApiResponse):
    """Response returned when a player is successfully authenticated."""

    error_enum: ClassVar[Type[enum.Enum]] = PlayerLoginError

    player: Optional[PlayerInfo] = None


@dataclass
class PlayerHeartbeatResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = PlayerHeartbeatError

    message: str = ""
    last_heartbeat: Optional[datetime] = None


@dataclass
class PlayerLogoutResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = PlayerLogoutError

    message: str = ""
    last_logout: Optional[datetime] = None


@dataclass
class PlayerRenameResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = PlayerRenameError

    message: str = ""
    new_name: str = ""
    player_id: int = 0


@dataclass
class PlayerBanResponse(ApiResponse):
    """Response containing the ban details."""

    error_enum: ClassVar[Type[enum.Enum]] = PlayerBanError

    message: str = ""
    ban_id: str = ""
    player_id: int = 0
    ban_duration: str = ""
    banned_until: str = ""


@dataclass
class PlayerUnbanResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = PlayerUnbanError

    message: str = ""
    player_id: int = 0


@dataclass
class PlayerDataResponse(ApiResponse):
    """Response containing a player's custom data."""

    error_enum: ClassVar[Type[enum.Enum]] = GameDataPlayerGetError

    type: str = ""
    player_id: int = 0
    player_name: str = ""
    data: Any = None


@dataclass
class BanResponse(ApiResponse):
    """Detailed ban information."""

    error_enum: ClassVar[Type[enum.Enum]] = CommonError

    ban_id: str = ""
    player_id: int = 0
    ban_duration: str = ""
    banned_until: str = ""
    ban_reason: Optional[str] = None

    @property
    def is_banned(self) -> bool:
        return not self.success and bool(self.error and "You are banned" in self.error)
