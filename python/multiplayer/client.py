"""Main HTTP client for the Michitai Multiplayer API."""

from __future__ import annotations

import json
from typing import Any, Optional, Type, TypeVar

import requests

from ._serialization import from_dict, to_jsonable
from .api_response import ApiResponse
from .errors.logger import Logger

T = TypeVar("T", bound=ApiResponse)

DEFAULT_BASE_URL = "https://api.michitai.com/api"


class Client:
    """HTTP client for communicating with the Michitai Multiplayer API.

    Handles authentication, JSON serialization and error handling.

    :param api_token: Public API token for game identification.
    :param api_private_token: Private API token for admin operations.
    :param base_url: Base URL of the API (default ``https://api.michitai.com/api``).
    :param logger: Optional logger (see :class:`~.errors.ConsoleLogger`).
    :param timeout: Request timeout in seconds (default 30).
    :param session: Optional custom :class:`requests.Session`.
    """

    def __init__(
        self,
        api_token: str,
        api_private_token: str,
        base_url: str = DEFAULT_BASE_URL,
        logger: Optional[Logger] = None,
        timeout: float = 30.0,
        session: Optional[requests.Session] = None,
    ) -> None:
        if api_token is None:
            raise ValueError("api_token is required")
        if api_private_token is None:
            raise ValueError("api_private_token is required")

        self._api_token = api_token
        self._api_private_token = api_private_token
        self._base_url = base_url if base_url.endswith("/") else base_url + "/"
        self._logger = logger
        self._timeout = timeout
        self._session = session or requests.Session()

        # Service facades (imported lazily to avoid circular imports)
        from .games.games import Games
        from .leaderboard.leaderboard import Leaderboard
        from .matchmaking.matchmaking import Matchmaking
        from .matchmaking.requests.requests import MatchmakingRequests
        from .players.players import Players
        from .rooms.actions.actions import Actions
        from .rooms.rooms import Rooms
        from .rooms.updates.updates import Updates
        from .time.time import Time

        self.players = Players(self)
        self.rooms = Rooms(self)
        self.actions = Actions(self)
        self.updates = Updates(self)
        self.matchmaking = Matchmaking(self)
        self.matchmaking_requests = MatchmakingRequests(self)
        self.leaderboard = Leaderboard(self)
        self.games = Games(self)
        self.time = Time(self)

    @property
    def api_token(self) -> str:
        return self._api_token

    def url(self, endpoint: str, extra: str = "") -> str:
        """Build a URL for a public endpoint (``api_token`` attached)."""
        return f"{self._base_url}{endpoint}?api_token={self._api_token}{extra}"

    def private_url(self, endpoint: str, extra: str = "") -> str:
        """Build a URL for an admin endpoint (``api_token`` + ``private_token``)."""
        return (
            f"{self._base_url}{endpoint}?api_token={self._api_token}"
            f"&private_token={self._api_private_token}{extra}"
        )

    def player_url(self, endpoint: str, player_token: str, extra: str = "") -> str:
        """Build a URL for a player-scoped endpoint."""
        return self.url(endpoint, f"&player_token={player_token}{extra}")

    def send(
        self,
        method: str,
        url: str,
        body: Any = None,
        response_cls: Type[T] = ApiResponse,  # type: ignore[assignment]
    ) -> T:
        """Send an HTTP request and deserialize the JSON response.

        Never raises for API-level failures — check ``response.success`` and
        ``response.error``/``response.error_type`` instead.
        """
        payload = to_jsonable(body) if body is not None else None

        try:
            res = self._session.request(
                method.upper(),
                url,
                json=payload,
                timeout=self._timeout,
            )
            text = res.text
        except requests.RequestException as exc:
            if self._logger:
                self._logger.error(f"HTTP request failed: {exc}")
            return self._error_response(response_cls, f"Request failed: {exc}")

        if self._logger:
            self._logger.log(f"API Response: {text}")

        try:
            data = json.loads(text) if text else {}
        except (json.JSONDecodeError, ValueError) as exc:
            if self._logger:
                self._logger.warn(
                    f"JSON deserialization error. Raw: {text}. Exception: {exc}"
                )
            return self._error_response(response_cls, "Failed to deserialize response")

        if not isinstance(data, dict):
            data = {"data": data}

        response = from_dict(response_cls, data)

        if not response.success and self._logger:
            self._logger.error(f"API Error: {response.error or 'Unknown error'}")

        return response

    @staticmethod
    def _error_response(response_cls: Type[T], message: str) -> T:
        try:
            response = from_dict(response_cls, {})
        except TypeError:
            response = response_cls()  # type: ignore[call-arg]
        response.success = False
        response.error = message
        return response

    def close(self) -> None:
        """Close the underlying HTTP session."""
        self._session.close()

    def __enter__(self) -> "Client":
        return self

    def __exit__(self, *exc_info: Any) -> None:
        self.close()
