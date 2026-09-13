"""Bidirectional conversion between API error strings and typed error enums."""

from __future__ import annotations

import enum
from typing import Optional, Type, TypeVar

E = TypeVar("E", bound=enum.Enum)

# Maps the exact error strings returned by the PHP API to enum member names.
ERROR_MAPPINGS = {
    # Common errors
    "API token is required": "ApiTokenIsRequired",
    "Invalid API token": "InvalidApiToken",
    "Method not allowed": "MethodNotAllowed",
    "Internal server error": "InternalServerError",
    "Failed to deserialize response": "FailedToDeserializeResponse",
    "Invalid endpoint": "InvalidEndpoint",
    "Database error": "DatabaseError",
    "An unexpected error occurred": "AnUnexpectedErrorOccurred",
    "You are banned": "YouAreBanned",

    # Game data errors
    "Game player token is required": "GamePlayerTokenIsRequired",
    "Invalid game player token": "InvalidGamePlayerToken",
    "Player does not belong to this game": "PlayerDoesNotBelongToThisGame",
    "API private token is required": "ApiPrivateTokenIsRequired",
    "Invalid API token or private token": "InvalidApiTokenOrPrivateToken",
    "Invalid player or does not belong to game": "InvalidPlayerOrDoesNotBelongToGame",

    # Player errors
    "Player name is required": "PlayerNameIsRequired",
    "Failed to register player": "FailedToRegisterPlayer",
    "API token and game player token are required": "ApiTokenAndGamePlayerTokenAreRequired",
    "Invalid player token": "InvalidPlayerToken",
    "Failed to update heartbeat": "FailedToUpdateHeartbeat",
    "New name is required": "NewNameIsRequired",
    "Player name must be between 2 and 50 characters": "PlayerNameMustBeBetween2And50Characters",
    "Failed to update player name": "FailedToUpdatePlayerName",
    "API token and private token are required": "ApiTokenAndPrivateTokenAreRequired",
    "Invalid API credentials": "InvalidApiCredentials",
    "player_id is required": "PlayerIdIsRequired",
    "ban_duration is required (hour, day, week, month, quarter, year, forever)": "BanDurationIsRequired",
    "Invalid ban_duration. Must be one of: hour, day, week, month, quarter, year, forever": "InvalidBanDuration",
    "Player not found or does not belong to this game": "PlayerNotFoundOrDoesNotBelongToThisGame",

    # Room errors
    "Player token is required": "PlayerTokenIsRequired",
    "You are already in a game room. Leave current room first.": "YouAreAlreadyInAGameRoomLeaveCurrentRoomFirst",
    "You cannot create a game room while in a matchmaking lobby.": "YouCannotCreateAGameRoomWhileInAMatchmakingLobby",
    "Failed to create room": "FailedToCreateRoom",
    "Failed to list rooms": "FailedToListRooms",
    "Only host can update room password": "OnlyHostCanUpdateRoomPassword",
    "Failed to update password": "FailedToUpdatePassword",
    "You are already in another room": "YouAreAlreadyInAnotherRoom",
    "Room not found": "RoomNotFound",
    "Room inactive": "RoomInactive",
    "Room is full": "RoomIsFull",
    "Incorrect password": "IncorrectPassword",
    "You are not in any room": "YouAreNotInAnyRoom",
    "You are not in this room": "YouAreNotInThisRoom",
    "Players are not allowed to leave this room": "PlayersAreNotAllowedToLeaveThisRoom",
    "Player is not in any room": "PlayerIsNotInAnyRoom",
    "Missing action_type": "MissingActionType",
    "request_data_json must be a string": "RequestDataJsonMustBeAString",
    "Only host can view pending actions": "OnlyHostCanViewPendingActions",
    "Only host can complete actions": "OnlyHostCanCompleteActions",
    "Status is required": "StatusIsRequired",
    "response_data_json must be a string": "ResponseDataJsonMustBeAString",
    "response_data is not valid JSON": "ResponseDataIsNotValidJson",
    "Action not found or already processed": "ActionNotFoundOrAlreadyProcessed",
    "Missing required field: target_players": "MissingRequiredFieldTargetPlayers",
    "Missing required field: type": "MissingRequiredFieldType",
    "Missing required field: target_players_ids": "MissingRequiredFieldTargetPlayersIds",
    "Invalid target players ids": "InvalidTargetPlayersIds",
    "No valid target players found": "NoValidTargetPlayersFound",
    "Invalid target players": "InvalidTargetPlayers",
    "Failed to send updates": "FailedToSendUpdates",
    "No valid source players found": "NoValidSourcePlayersFound",
    "Invalid from players": "InvalidFromPlayers",
    "Only host can kick players": "OnlyHostCanKickPlayers",
    "You cannot kick yourself": "YouCannotKickYourself",
    "Missing required field: player_id": "MissingRequiredFieldPlayerId",
    "Player not found in this room": "PlayerNotFoundInThisRoom",
    "Failed to kick player": "FailedToKickPlayer",
    "You are not in any game room": "YouAreNotInAnyGameRoom",
    "Only host can stop game room": "OnlyHostCanStopTheGameRoom",
    "Failed to stop game room": "FailedToStopGameRoom",

    # Leaderboard errors
    "Method not allowed. Use POST.": "MethodNotAllowedUsePost",
    "api_token is required": "ApiTokenIsRequired",
    "Invalid or expired api_token": "InvalidOrExpiredApiToken",
    "Invalid JSON body": "InvalidJsonBody",
    "sort_by must be a non-empty array of field names": "SortByMustBeANonEmptyArrayOfFieldNames",
    "limit must be between 1 and 100": "LimitMustBeBetween1And100",
    "No valid sort fields provided after sanitization": "NoValidSortFieldsProvidedAfterSanitization",
    "Server error": "ServerError",

    # Matchmaking errors
    "Failed to list matchmaking lobbies": "FailedToListMatchmakingLobbies",
    "You are already in a matchmaking lobby": "YouAreAlreadyInAMatchmakingLobby",
    "You cannot create matchmaking while in a game room. Leave room first.": "YouCannotCreateMatchmakingWhileInAGameRoomLeaveRoomFirst",
    "Missing required field: max_players": "MissingRequiredFieldMaxPlayers",
    "Failed to create matchmaking lobby": "FailedToCreateMatchmakingLobby",
    "Missing required parameter: matchmakingId": "MissingRequiredParameterMatchmakingId",
    "You already have a pending request to this matchmaking lobby": "YouAlreadyHaveAPendingRequestToThisMatchmakingLobby",
    "Matchmaking lobby not found or already started": "MatchmakingLobbyNotFoundOrAlreadyStarted",
    "Matchmaking lobby is full": "MatchmakingLobbyIsFull",
    "This matchmaking lobby requires host approval. Use /request endpoint instead.": "ThisMatchmakingLobbyRequiresHostApprovalUseRequestEndpointInstead",
    "You are not in any matchmaking lobby": "YouAreNotInAnyMatchmakingLobby",
    "Players are not allowed to leave this matchmaking lobby": "PlayersAreNotAllowedToLeaveThisMatchmakingLobby",
    "Failed to get players": "FailedToGetPlayers",
    "You are not in a matchmaking lobby": "YouAreNotInAMatchmakingLobby",
    "Only host can remove matchmaking lobby": "OnlyHostCanRemoveMatchmakingLobby",
    "Failed to remove matchmaking lobby": "FailedToRemoveMatchmakingLobby",
    "Failed to get matchmaking status": "FailedToGetMatchmakingStatus",
    "Missing required parameter: requestId": "MissingRequiredParameterRequestId",
    "Request not found or you are not the requester": "RequestNotFoundOrYouAreNotTheRequester",
    "Missing required fields: requestId and action": "MissingRequiredFieldsRequestIdAndAction",
    'Action must be "approve" or "reject"': "ActionMustBeApproveOrReject",
    "Request not found or already processed": "RequestNotFoundOrAlreadyProcessed",
    "Only the host can respond to join requests": "OnlyTheHostCanRespondToJoinRequests",
    "Only host can start matchmaking": "OnlyHostCanStartMatchmaking",
    "Lobby must be full to start (strict_full enabled)": "LobbyMustBeFullToStartStrictFullEnabled",
    "Cannot kick players from matchmaking after it has been started": "CannotKickFromStartedMatchmaking",
    "Player not found in this matchmaking lobby": "PlayerNotFoundInThisMatchmakingLobby",
    "Only host can stop matchmaking lobby": "OnlyHostCanStopMatchmakingLobby",
    "Cannot stop matchmaking lobby after it has been started": "CannotStopMatchmakingLobbyAfterItHasBeenStarted",
    "Only host can update matchmaking password": "OnlyHostCanUpdateMatchmakingPassword",
    "Cannot change password after matchmaking has started": "CannotChangePasswordAfterMatchmakingHasStarted",

    # Time errors
    "API key is required": "ApiKeyIsRequired",
    "Invalid API key": "InvalidApiKey",

    # Realtime errors
    "Player is not in a realtime-enabled room": "PlayerIsNotInARealtimeEnabledRoom",
    "Failed to generate realtime token": "FailedToGenerateRealtimeToken",
}


def convert_to_enum(error_message: Optional[str], enum_type: Type[E]) -> E:
    """Convert an API error message string to the corresponding enum member.

    Falls back to the enum's ``Unknown`` member (or the first member) when no
    mapping matches.
    """
    unknown = getattr(enum_type, "Unknown", None)
    if unknown is None:
        unknown = next(iter(enum_type))

    if not error_message:
        return unknown

    member_name = ERROR_MAPPINGS.get(error_message)
    if member_name is not None:
        member = getattr(enum_type, member_name, None)
        if member is not None:
            return member

    # Case-insensitive fallback: compare against member names too
    for name, member in enum_type.__members__.items():
        if name.lower() == error_message.lower():
            return member

    return unknown


def get_error_message(error_type: enum.Enum) -> str:
    """Return the original API error string for an enum member, if known."""
    for message, name in ERROR_MAPPINGS.items():
        if name == error_type.name:
            return message
    return "Unknown error"
