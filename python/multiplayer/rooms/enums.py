"""Enums for the Rooms module."""

from __future__ import annotations

import enum


class RoomTargetPlayers(str, enum.Enum):
    """Target players for room actions and updates."""

    HOST = "host"
    ALL = "all"
    OTHERS = "others"
    SPECIFIC = "specific"
