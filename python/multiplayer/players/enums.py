"""Enums for the Players module."""

from __future__ import annotations

import enum


class BanTime(str, enum.Enum):
    """Ban duration options for player bans."""

    HOUR = "hour"
    DAY = "day"
    WEEK = "week"
    MONTH = "month"
    QUARTER = "quarter"
    YEAR = "year"
    FOREVER = "forever"
