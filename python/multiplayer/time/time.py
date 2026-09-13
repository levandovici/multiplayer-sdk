"""Server time queries."""

from __future__ import annotations

from ..client import Client
from .models import ServerTimeResponse, ServerTimeWithOffsetResponse

TIME = "time.php"


class Time:
    """Query server time information."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def get_server_time(self) -> ServerTimeResponse:
        """Get the current server time in UTC."""
        return self._client.send(
            "GET", self._client.url(TIME), None, ServerTimeResponse
        )

    def get_server_time_with_offset(
        self, utc_offset: int
    ) -> ServerTimeWithOffsetResponse:
        """Get the server time adjusted by a UTC offset in hours.

        :param utc_offset: e.g. ``3`` for UTC+3, ``-5`` for UTC-5.
        """
        sign = "+" if utc_offset >= 0 else "-"
        return self._client.send(
            "GET",
            self._client.url(TIME, f"&utc={sign}{abs(utc_offset)}"),
            None,
            ServerTimeWithOffsetResponse,
        )
