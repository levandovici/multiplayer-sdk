"""Friends system: tag lookups, friend requests, Steam import, invites."""

from __future__ import annotations

from typing import Any, List, Optional

from ..client import Client
from .models import (
    FriendInviteAcceptResponse,
    FriendInviteResponse,
    FriendInvitesResponse,
    FriendListResponse,
    FriendLookupResponse,
    FriendStatusResponse,
    SteamFriendsLookupResponse,
)

FRIENDS_LIST = "friends.php/list"
FRIENDS_LOOKUP = "friends.php/lookup"
FRIENDS_REQUEST = "friends.php/request"
FRIENDS_ACCEPT = "friends.php/accept"
FRIENDS_DECLINE = "friends.php/decline"
FRIENDS_REMOVE = "friends.php/remove"
FRIENDS_STEAM = "friends.php/steam"
FRIENDS_INVITE = "friends.php/invite"
FRIENDS_INVITES = "friends.php/invites"
FRIENDS_INVITES_ACCEPT = "friends.php/invites/accept"
FRIENDS_INVITES_DECLINE = "friends.php/invites/decline"


class Friends:
    """Friends list, #tag lookup, Steam import, and lobby/room invites.

    Invited friends join without the lobby password — the invite itself is
    the server's authorization.
    """

    def __init__(self, client: Client) -> None:
        self._client = client

    def list(self, player_token: str) -> FriendListResponse:
        """Get the local player's friends, pending requests, and own tag."""
        return self._client.send(
            "GET",
            self._client.url(FRIENDS_LIST),
            None,
            FriendListResponse,
            player_token=player_token,
        )

    def lookup_by_tag(
        self, player_token: str, player_tag: str
    ) -> FriendLookupResponse:
        """Look up a player by friend tag (``#XXXXXXXX`` or the 8-char body)."""
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_LOOKUP),
            {"player_tag": player_tag},
            FriendLookupResponse,
            player_token=player_token,
        )

    def send_request_by_tag(
        self, player_token: str, player_tag: str
    ) -> FriendStatusResponse:
        """Send a friend request by tag. If the target already sent one, it
        is accepted automatically (status ``"accepted"``)."""
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_REQUEST),
            {"player_tag": player_tag},
            FriendStatusResponse,
            player_token=player_token,
        )

    def send_request(
        self, player_token: str, player_id: int
    ) -> FriendStatusResponse:
        """Send a friend request by player ID (e.g. from a Steam lookup)."""
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_REQUEST),
            {"player_id": player_id},
            FriendStatusResponse,
            player_token=player_token,
        )

    def accept_request(
        self, player_token: str, player_id: int
    ) -> FriendStatusResponse:
        """Accept a pending friend request."""
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_ACCEPT),
            {"player_id": player_id},
            FriendStatusResponse,
            player_token=player_token,
        )

    def decline_request(
        self, player_token: str, player_id: int
    ) -> FriendStatusResponse:
        """Decline a pending friend request."""
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_DECLINE),
            {"player_id": player_id},
            FriendStatusResponse,
            player_token=player_token,
        )

    def remove_friend(
        self, player_token: str, player_id: int
    ) -> FriendStatusResponse:
        """Remove a friend."""
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_REMOVE),
            {"player_id": player_id},
            FriendStatusResponse,
            player_token=player_token,
        )

    def lookup_steam_friends(
        self, player_token: str, steam_ids: List[str]
    ) -> SteamFriendsLookupResponse:
        """Look up game accounts linked to the given SteamID64s (max 100) —
        for importing Steam friends into the backend friends list."""
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_STEAM),
            {"steam_ids": steam_ids},
            SteamFriendsLookupResponse,
            player_token=player_token,
        )

    def invite_friend(
        self, player_token: str, player_id: int
    ) -> FriendInviteResponse:
        """Invite a friend into the local player's current lobby or room.
        The invitee bypasses password and host approval."""
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_INVITE),
            {"player_id": player_id},
            FriendInviteResponse,
            player_token=player_token,
        )

    def get_invites(self, player_token: str) -> FriendInvitesResponse:
        """List pending lobby/room invites for the local player."""
        return self._client.send(
            "GET",
            self._client.url(FRIENDS_INVITES),
            None,
            FriendInvitesResponse,
            player_token=player_token,
        )

    def accept_invite(
        self,
        player_token: str,
        invite_id: str,
        player_data: Any = None,
    ) -> FriendInviteAcceptResponse:
        """Accept a lobby/room invite and join it directly — no password or
        host approval required. Returns the joined matchmaking/room ID."""
        body = {"invite_id": invite_id}
        if player_data is not None:
            body["player_data"] = player_data
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_INVITES_ACCEPT),
            body,
            FriendInviteAcceptResponse,
            player_token=player_token,
        )

    def decline_invite(
        self, player_token: str, invite_id: str
    ) -> FriendInviteAcceptResponse:
        """Decline a lobby/room invite."""
        return self._client.send(
            "POST",
            self._client.url(FRIENDS_INVITES_DECLINE),
            {"invite_id": invite_id},
            FriendInviteAcceptResponse,
            player_token=player_token,
        )
