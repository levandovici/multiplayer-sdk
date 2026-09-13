"""Enums for room actions."""

from __future__ import annotations

import enum


class RoomActionStatus(str, enum.Enum):
    """Status of a room action."""

    PENDING = "pending"
    PROCESSING = "processing"
    COMPLETED = "completed"
    FAILED = "failed"
    READ = "read"


class RoomCompleteActionStatus(str, enum.Enum):
    """Allowed statuses when completing a room action."""

    PROCESSING = "processing"
    COMPLETED = "completed"
    FAILED = "failed"
