"""Server-side text screening."""

from __future__ import annotations

from ..client import Client
from .models import CensorResponse

CENSOR = "censor.php"


class Moderation:
    """Screen player-visible text through the server-side moderation filter.

    The filter normalizes leetspeak, confusable scripts, separators and
    repeated-letter evasion before matching, so ``w e e d`` or ``w33d``
    are caught the same as ``weed``.
    """

    def __init__(self, client: Client) -> None:
        self._client = client

    def censor(self, text: str) -> CensorResponse:
        """Screen ``text`` and return the clean flag plus a filtered copy.

        :param text: The text to screen (max 1000 characters).
        """
        return self._client.send(
            "POST",
            self._client.url(CENSOR),
            {"text": text},
            CensorResponse,
        )
