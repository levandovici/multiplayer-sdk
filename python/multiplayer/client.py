"""Main HTTP client for the Michitai Multiplayer API."""

from __future__ import annotations

import json
from typing import Any, Dict, Optional, Type, TypeVar

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
    :param api_private_token: Private API token for admin operations (optional —
        omit it in shipped/game clients; admin calls then raise ``ValueError``).
    :param base_url: Base URL of the API (default ``https://api.michitai.com/api``).
    :param logger: Optional logger (see :class:`~.errors.ConsoleLogger`).
    :param timeout: Request timeout in seconds (default 30).
    :param session: Optional custom :class:`requests.Session`.
    :param debug_logging: Log full request/response bodies (default ``False`` —
        responses can contain credentials; keep off in production).
    """

    def __init__(
        self,
        api_token: str,
        api_private_token: str = "",
        base_url: str = DEFAULT_BASE_URL,
        logger: Optional[Logger] = None,
        timeout: float = 30.0,
        session: Optional[requests.Session] = None,
        debug_logging: bool = False,
    ) -> None:
        if api_token is None:
            raise ValueError("api_token is required")

        self._api_token = api_token
        self._api_private_token = api_private_token or ""
        self._base_url = base_url if base_url.endswith("/") else base_url + "/"
        self._logger = logger
        self._timeout = timeout
        self._session = session or requests.Session()
        self._debug_logging = debug_logging

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
        """Build a URL for an API endpoint.

        Credentials are never placed in the URL — they are sent as headers
        by :meth:`send`. ``extra`` may start with ``?`` or ``&``.
        """
        normalized = extra.lstrip("?&")
        if normalized:
            return f"{self._base_url}{endpoint}?{normalized}"
        return f"{self._base_url}{endpoint}"

    def send(
        self,
        method: str,
        url: str,
        body: Any = None,
        response_cls: Type[T] = ApiResponse,  # type: ignore[assignment]
        *,
        player_token: Optional[str] = None,
        use_private_token: bool = False,
    ) -> T:
        """Send an HTTP request and deserialize the JSON response.

        Never raises for API-level failures — check ``response.success`` and
        ``response.error``/``response.error_type`` instead.

        :param player_token: Player token, sent as ``X-Game-Player-Token``.
        :param use_private_token: Send the admin ``X-Api-Private-Token`` header.
        """
        payload = to_jsonable(body) if body is not None else None
        headers: Dict[str, str] = {"X-Api-Token": self._api_token}
        if player_token is not None:
            headers["X-Game-Player-Token"] = player_token
        if use_private_token:
            if not self._api_private_token:
                raise ValueError(
                    "Admin operations require api_private_token — construct "
                    "Client with the private key (server-side tooling only)"
                )
            headers["X-Api-Private-Token"] = self._api_private_token

        try:
            res = self._session.request(
                method.upper(),
                url,
                json=payload,
                headers=headers,
                timeout=self._timeout,
            )
            text = res.text
        except requests.RequestException as exc:
            if self._logger:
                self._logger.error(f"HTTP request failed: {exc}")
            return self._error_response(response_cls, f"Request failed: {exc}")

        if self._logger and self._debug_logging:
            self._logger.log(f"API Response: {text}")

        try:
            data = json.loads(text) if text else {}
        except (json.JSONDecodeError, ValueError) as exc:
            if self._logger:
                self._logger.warn(
                    f"JSON deserialization error. Raw: {text}. Exception: {exc}"
                    if self._debug_logging
                    else f"JSON deserialization error. Exception: {exc}"
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
