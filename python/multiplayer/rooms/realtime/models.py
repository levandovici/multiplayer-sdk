"""Models for the realtime WebSocket API."""

from __future__ import annotations

import enum
from dataclasses import dataclass
from typing import Any, ClassVar, Optional, Type

from ...api_response import ApiResponse
from ...errors.enums import RealtimeTokenError


@dataclass
class RealtimePlayerInfo:
    """The player's identity inside the realtime room."""

    player_id: int = 0
    player_name: str = ""
    room_id: str = ""
    is_host: bool = False


@dataclass
class RealtimeServerInfo:
    """Connection info for the realtime server."""

    host: str = ""
    port: int = 0
    protocol: str = ""


@dataclass
class TokenResponse(ApiResponse):
    """Realtime authentication token for WebSocket connections."""

    error_enum: ClassVar[Type[enum.Enum]] = RealtimeTokenError

    token: str = ""
    player_info: Optional[RealtimePlayerInfo] = None
    realtime_server: Optional[RealtimeServerInfo] = None


@dataclass
class SenderInfo:
    """Information about the sender of a realtime message."""

    is_host: bool = False
    game_player_id: int = 0
    player_name: str = ""


@dataclass
class RealtimeMessage:
    """A message received over the realtime WebSocket."""

    type: Optional[str] = None
    command: Optional[str] = None
    data: Any = None
    sender: Optional[SenderInfo] = None
