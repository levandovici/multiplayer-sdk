"""Enums for realtime communication."""

from __future__ import annotations

import enum


class RoomTargetPlayer(str, enum.Enum):
    """Target players for realtime messages."""

    ALL = "all"
    HOST = "host"
    OTHERS = "others"
    SPECIFIC = "specific"
