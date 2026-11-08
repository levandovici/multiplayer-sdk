"""Region discovery & game resolution.

``client.regions.list()`` returns every enabled service node plus the
server's geo guess. A game is pinned to exactly one node — chosen when the
API key is created; ``client.regions.resolve_game(api_key)`` asks the
directory which node hosts the game. Player accounts live on that node
only and never roam — pin the client's base URL to it.
"""

from __future__ import annotations

from typing import Optional
from urllib.parse import quote

from ..client import Client
from .models import DiscoverResponse, GameNodeResponse

DISCOVER = "discover.php"


class Regions:
    """Discover service nodes and resolve a game's pinned node."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def list(self) -> DiscoverResponse:
        """Fetch the enabled service-node directory."""
        return self._client.send("GET", self._client.url(DISCOVER), None, DiscoverResponse)

    def resolve_game(self, api_key: str) -> GameNodeResponse:
        """Resolve which node hosts a game.

        Point every subsequent REST and realtime call at the returned
        node's ``api_url``: the game's players, rooms and leaderboards
        exist only there, and other nodes answer 421 with a
        ``home_api_url`` hint for the same resolution.

        :param api_key: the game's public api_key.
        :return: :class:`GameNodeResponse` — region code plus pinned node.
        """
        # The client attaches X-Api-Token on every request — POST with no
        # query string; the key never travels in a URL.
        return self._client.send("POST", self._client.url(DISCOVER), api_key, GameNodeResponse)
