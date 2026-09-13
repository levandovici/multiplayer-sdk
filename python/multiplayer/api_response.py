"""Base classes for all API responses."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass, field
from typing import Any, ClassVar, Optional, Type

from .errors.converter import convert_to_enum, get_error_message
from .errors.enums import CommonError


@dataclass
class BanInfo:
    """Ban details returned when a banned player calls an endpoint."""

    ban_id: str = ""
    ban_duration: str = ""
    ban_reason: Optional[str] = None
    banned_at: Optional[datetime] = None
    banned_until: Optional[datetime] = None


@dataclass
class ApiResponse:
    """Base class for every API response."""

    #: The error enum type used by :attr:`error_type`. Subclasses override this.
    error_enum: ClassVar[Type[enum.Enum]] = CommonError

    success: bool = False
    error: Optional[str] = None
    ban_info: Optional[BanInfo] = field(default=None)

    @property
    def error_type(self) -> enum.Enum:
        """The typed error for :attr:`error`, or ``Unknown`` when unmapped."""
        return convert_to_enum(self.error, self.error_enum)

    @property
    def error_message(self) -> str:
        """A user-friendly message for :attr:`error_type`."""
        return get_error_message(self.error_type)


@dataclass
class SuccessResponse(ApiResponse):
    """Standard success response for operations that return no specific data."""

    message: str = ""
    updated_at: Optional[datetime] = None
