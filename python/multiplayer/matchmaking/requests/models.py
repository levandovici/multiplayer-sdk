"""Request/response models for matchmaking join requests."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass
from typing import Any, ClassVar, Optional, Type

from ...api_response import ApiResponse
from ...errors.enums import (
    MatchmakingRequestError,
    MatchmakingResponseError,
    MatchmakingStatusError,
)


@dataclass
class MatchmakingRequestBase:
    """Summary of a join request."""

    request_id: str = ""
    matchmaking_id: str = ""
    status: str = ""
    requested_at: Optional[datetime] = None
    responded_at: Optional[datetime] = None


@dataclass
class MatchmakingRequestInfo(MatchmakingRequestBase):
    """Detailed join request info including the responder."""

    responded_by: Optional[int] = None
    responder_name: Optional[str] = None
    join_by_requests: bool = False


@dataclass
class MatchmakingJoinRequestResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingRequestError

    request_id: str = ""
    message: str = ""


@dataclass
class MatchmakingPermissionResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingResponseError

    message: str = ""
    request_id: str = ""
    action: str = ""


@dataclass
class MatchmakingRequestStatusResponse(ApiResponse):
    error_enum: ClassVar[Type[enum.Enum]] = MatchmakingStatusError

    request: Optional[MatchmakingRequestInfo] = None
