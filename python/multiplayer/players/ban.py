"""Utilities for ban-related operations."""

from __future__ import annotations

from ..api_response import ApiResponse


def is_banned(response: ApiResponse) -> bool:
    """Return True if the API response indicates the player is banned."""
    if not response.success and response.error:
        return "You are banned" in response.error or response.ban_info is not None
    return False
