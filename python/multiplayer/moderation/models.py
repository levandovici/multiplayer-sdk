"""Models for the moderation API."""

from __future__ import annotations

import enum
from dataclasses import dataclass
from typing import ClassVar, Optional, Type

from ..api_response import ApiResponse
from ..errors.enums import CensorError


@dataclass
class CensorResponse(ApiResponse):
    """Result of screening a text through the server-side moderation filter.

    ``clean`` is true when the text contains no listed word after
    normalization; ``filtered`` is the masked, display-safe copy.
    ``retry_after`` is set only on a 429 reply.
    """

    error_enum: ClassVar[Type[enum.Enum]] = CensorError

    clean: bool = False
    filtered: Optional[str] = None
    retry_after: int = 0
