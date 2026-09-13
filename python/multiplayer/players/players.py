"""Player management: registration, authentication, data, and banning."""

from __future__ import annotations

from typing import Any, Optional, Type, TypeVar

from ..api_response import SuccessResponse
from ..client import Client
from .enums import BanTime
from .models import (
    PlayerAuthResponse,
    PlayerBanResponse,
    PlayerDataResponse,
    PlayerHeartbeatResponse,
    PlayerLogoutResponse,
    PlayerRegisterResponse,
    PlayerRenameResponse,
    PlayerUnbanResponse,
)

T = TypeVar("T")

# Endpoints
GAME_PLAYERS_REGISTER = "game_players.php/register"
GAME_PLAYERS_LOGIN = "game_players.php/login"
GAME_PLAYERS_HEARTBEAT = "game_players.php/heartbeat"
GAME_PLAYERS_LOGOUT = "game_players.php/logout"
GAME_PLAYERS_RENAME = "game_players.php/rename"
GAME_PLAYERS_BAN = "game_players.php/ban"
GAME_PLAYERS_UNBAN = "game_players.php/unban"
GAME_DATA_PLAYER_GET = "game_data.php/player/get"
GAME_DATA_PLAYER_UPDATE = "game_data.php/player/update"


class Players:
    """Player registration, authentication, data management and admin ops."""

    def __init__(self, client: Client) -> None:
        self._client = client

    def register(
        self,
        name: str,
        player_data: Any = None,
    ) -> PlayerRegisterResponse:
        """Register a new player with the game.

        :param name: The player's display name.
        :param player_data: Optional initial player data (dict or dataclass).
        :returns: Response with ``player_id`` and ``private_key`` (player token).
        """
        return self._client.send(
            "POST",
            self._client.url(GAME_PLAYERS_REGISTER),
            {"player_name": name, "player_data": player_data},
            PlayerRegisterResponse,
        )

    def authenticate(
        self,
        player_token: str,
        data_cls: Optional[Type[T]] = None,
    ) -> PlayerAuthResponse:
        """Authenticate a player using their private token.

        :param player_token: The player's private authentication token.
        :param data_cls: Optional dataclass type to deserialize ``player_data`` into.
        """
        response = self._client.send(
            "PUT",
            self._client.player_url(GAME_PLAYERS_LOGIN, player_token),
            None,
            PlayerAuthResponse,
        )
        if data_cls is not None and response.player is not None:
            response.player.player_data = _convert_data(
                response.player.player_data, data_cls
            )
        return response

    def heartbeat(self, player_token: str) -> PlayerHeartbeatResponse:
        """Send a heartbeat to maintain the player's online status."""
        return self._client.send(
            "POST",
            self._client.player_url(GAME_PLAYERS_HEARTBEAT, player_token),
            None,
            PlayerHeartbeatResponse,
        )

    def logout(self, player_token: str) -> PlayerLogoutResponse:
        """Log out a player from the game."""
        return self._client.send(
            "POST",
            self._client.player_url(GAME_PLAYERS_LOGOUT, player_token),
            None,
            PlayerLogoutResponse,
        )

    def rename(self, player_token: str, new_name: str) -> PlayerRenameResponse:
        """Rename a player (new name must be 2-50 characters)."""
        return self._client.send(
            "PUT",
            self._client.player_url(GAME_PLAYERS_RENAME, player_token),
            {"new_name": new_name},
            PlayerRenameResponse,
        )

    def ban(
        self,
        player_id: int,
        ban_duration: BanTime,
        ban_reason: Optional[str] = None,
    ) -> PlayerBanResponse:
        """Ban a player. Requires the private API token.

        :param player_id: The ID of the player to ban.
        :param ban_duration: Duration of the ban.
        :param ban_reason: Optional reason for the ban.
        """
        return self._client.send(
            "POST",
            self._client.private_url(GAME_PLAYERS_BAN),
            {
                "player_id": player_id,
                "ban_duration": ban_duration.value,
                "ban_reason": ban_reason,
            },
            PlayerBanResponse,
        )

    def unban(self, player_id: int) -> PlayerUnbanResponse:
        """Unban a previously banned player. Requires the private API token."""
        return self._client.send(
            "POST",
            self._client.private_url(GAME_PLAYERS_UNBAN),
            {"player_id": player_id},
            PlayerUnbanResponse,
        )

    def get_data(
        self,
        player_token: str,
        data_cls: Optional[Type[T]] = None,
    ) -> PlayerDataResponse:
        """Retrieve a player's custom data.

        :param data_cls: Optional dataclass type to deserialize ``data`` into.
        """
        response = self._client.send(
            "GET",
            self._client.player_url(GAME_DATA_PLAYER_GET, player_token),
            None,
            PlayerDataResponse,
        )
        if data_cls is not None and response.data is not None:
            response.data = _convert_data(response.data, data_cls)
        return response

    def update_data(self, player_token: str, data: Any) -> SuccessResponse:
        """Replace a player's custom data with the provided object."""
        return self._client.send(
            "PUT",
            self._client.player_url(GAME_DATA_PLAYER_UPDATE, player_token),
            data,
            SuccessResponse,
        )


def _convert_data(value: Any, data_cls: Type[T]) -> Any:
    from .._serialization import from_dict
    import dataclasses

    if dataclasses.is_dataclass(data_cls) and isinstance(value, dict):
        return from_dict(data_cls, value)
    return value
