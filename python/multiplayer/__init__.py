"""Official Python SDK for the Michitai Multiplayer API (api.michitai.com).

Quick start::

    from multiplayer import Client

    client = Client("YOUR_API_TOKEN", "YOUR_PRIVATE_TOKEN")

    reg = client.players.register("PlayerOne", {"level": 1})
    player_token = reg.private_key

    room = client.rooms.create(player_token, "Lobby 1", max_players=4)
    print(room.room_id)
"""

from .api_response import ApiResponse, BanInfo, SuccessResponse
from .client import Client
from .errors import (
    CommonError,
    ConsoleLogger,
    Logger,
    convert_to_enum,
    get_error_message,
)
from .games import Games
from .leaderboard import Leaderboard
from .matchmaking import Matchmaking, MatchmakingRequests
from .players import BanTime, Players, is_banned
from .rooms import Rooms, RoomTargetPlayers
from .rooms.actions import Actions, RoomActionStatus, RoomCompleteActionStatus
from .rooms.realtime import Realtime, RoomTargetPlayer
from .rooms.updates import Updates
from .time import Time

__version__ = "1.0.0"

__all__ = [
    "Client",
    "ApiResponse",
    "BanInfo",
    "SuccessResponse",
    "CommonError",
    "ConsoleLogger",
    "Logger",
    "convert_to_enum",
    "get_error_message",
    "Games",
    "Leaderboard",
    "Matchmaking",
    "MatchmakingRequests",
    "Players",
    "BanTime",
    "is_banned",
    "Rooms",
    "RoomTargetPlayers",
    "Actions",
    "RoomActionStatus",
    "RoomCompleteActionStatus",
    "Realtime",
    "RoomTargetPlayer",
    "Updates",
    "Time",
    "__version__",
]
