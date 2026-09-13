"""Models for the server time API."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass
from typing import Any, ClassVar, Optional, Type

from ..api_response import ApiResponse
from ..errors.enums import TimeError


@dataclass
class ServerTimeResponse(ApiResponse):
    """Current server time in UTC."""

    error_enum: ClassVar[Type[enum.Enum]] = TimeError

    utc: Optional[datetime] = None
    timestamp: int = 0
    readable: str = ""


@dataclass
class TimeOffset:
    """Offset details when a UTC offset was requested."""

    offset_hours: int = 0
    offset_string: str = ""
    original_utc: Optional[datetime] = None
    original_timestamp: int = 0


@dataclass
class ServerTimeWithOffsetResponse(ServerTimeResponse):
    """Server time adjusted by a UTC offset."""

    offset: Optional[TimeOffset] = None
