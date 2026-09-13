"""Enums for matchmaking join requests."""

from __future__ import annotations

import enum


class MatchmakingRequestAction(str, enum.Enum):
    """Actions for responding to matchmaking join requests."""

    APPROVE = "approve"
    REJECT = "reject"
