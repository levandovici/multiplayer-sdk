"""Request/response models for the friends system."""

from __future__ import annotations

import enum
from datetime import datetime
from dataclasses import dataclass, field
from typing import ClassVar, List, Optional, Type

from ..api_response import ApiResponse
from ..errors.enums import FriendsError


@dataclass
class FriendInfo:
    """Public information about a player in a friends context."""

    player_id: int = 0
    player_name: str = ""
    player_tag: Optional[str] = None
    is_online: bool = False
    steam_id: Optional[str] = None
    relationship: Optional[str] = None
    requested_at: Optional[datetime] = None


@dataclass
class FriendInvite:
    """A pending lobby or room invitation received from a friend."""

    invite_id: str = ""
    kind: str = ""  # "matchmaking" or "room"
    name: str = ""
    from_player: Optional[FriendInfo] = None
    created_at: Optional[datetime] = None


@dataclass
class FriendListResponse(ApiResponse):
    """Friends list, pending requests, and the local player's own tag."""

    error_enum: ClassVar[Type[enum.Enum]] = FriendsError

    player_tag: Optional[str] = None
    friends: List[FriendInfo] = field(default_factory=list)
    incoming: List[FriendInfo] = field(default_factory=list)
    outgoing: List[FriendInfo] = field(default_factory=list)


@dataclass
class FriendLookupResponse(ApiResponse):
    """Result of a player-tag lookup."""

    error_enum: ClassVar[Type[enum.Enum]] = FriendsError

    player: Optional[FriendInfo] = None
    relationship: Optional[str] = None


@dataclass
class FriendStatusResponse(ApiResponse):
    """Result of a friend request/accept/decline/remove operation."""

    error_enum: ClassVar[Type[enum.Enum]] = FriendsError

    status: str = ""
    player_id: int = 0


@dataclass
class FriendInviteResponse(ApiResponse):
    """Result of sending a lobby/room invite."""

    error_enum: ClassVar[Type[enum.Enum]] = FriendsError

    invite_id: str = ""
    status: str = ""


@dataclass
class FriendInvitesResponse(ApiResponse):
    """Pending lobby/room invites for the local player."""

    error_enum: ClassVar[Type[enum.Enum]] = FriendsError

    invites: List[FriendInvite] = field(default_factory=list)


@dataclass
class FriendInviteAcceptResponse(ApiResponse):
    """Result of accepting or declining an invite. On accept,
    ``matchmaking_id`` or ``room_id`` identifies the joined target."""

    error_enum: ClassVar[Type[enum.Enum]] = FriendsError

    status: str = ""
    invite_id: str = ""
    matchmaking_id: str = ""
    room_id: str = ""


@dataclass
class SteamFriendsLookupResponse(ApiResponse):
    """Players linked to the SteamIDs passed to the lookup."""

    error_enum: ClassVar[Type[enum.Enum]] = FriendsError

    players: List[FriendInfo] = field(default_factory=list)
