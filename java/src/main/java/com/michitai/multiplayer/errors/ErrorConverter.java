package com.michitai.multiplayer.errors;

import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

/**
 * Utility class for converting between API error messages and typed error enums.
 * Provides bidirectional conversion for error handling and user-friendly error messages.
 */
public final class ErrorConverter {
    private ErrorConverter() {
    }

    private static final Map<String, String> ERROR_MAPPINGS;

    static {
        Map<String, String> m = new HashMap<>();

        // Common errors
        m.put("API token is required", "ApiTokenIsRequired");
        m.put("Invalid API token", "InvalidApiToken");
        m.put("Method not allowed", "MethodNotAllowed");
        m.put("Internal server error", "InternalServerError");
        m.put("Failed to deserialize response", "FailedToDeserializeResponse");
        m.put("Invalid endpoint", "InvalidEndpoint");
        m.put("Database error", "DatabaseError");
        m.put("An unexpected error occurred", "AnUnexpectedErrorOccurred");
        m.put("You are banned", "YouAreBanned");

        // Game data errors
        m.put("Game player token is required", "GamePlayerTokenIsRequired");
        m.put("Invalid game player token", "InvalidGamePlayerToken");
        m.put("Player does not belong to this game", "PlayerDoesNotBelongToThisGame");
        m.put("API private token is required", "ApiPrivateTokenIsRequired");
        m.put("Invalid API token or private token", "InvalidApiTokenOrPrivateToken");
        m.put("Invalid player or does not belong to game", "InvalidPlayerOrDoesNotBelongToGame");

        // Player errors
        m.put("Player name is required", "PlayerNameIsRequired");
        m.put("Failed to register player", "FailedToRegisterPlayer");
        m.put("API token and game player token are required", "ApiTokenAndGamePlayerTokenAreRequired");
        m.put("Invalid player token", "InvalidPlayerToken");
        m.put("Failed to update heartbeat", "FailedToUpdateHeartbeat");
        m.put("New name is required", "NewNameIsRequired");
        m.put("Player name must be between 2 and 50 characters", "PlayerNameMustBeBetween2And50Characters");
        m.put("Failed to update player name", "FailedToUpdatePlayerName");
        m.put("API token and private token are required", "ApiTokenAndPrivateTokenAreRequired");
        m.put("Invalid API credentials", "InvalidApiCredentials");
        m.put("player_id is required", "PlayerIdIsRequired");
        m.put("ban_duration is required (hour, day, week, month, quarter, year, forever)", "BanDurationIsRequired");
        m.put("Invalid ban_duration. Must be one of: hour, day, week, month, quarter, year, forever", "InvalidBanDuration");
        m.put("Player not found or does not belong to this game", "PlayerNotFoundOrDoesNotBelongToThisGame");

        // Room errors
        m.put("Player token is required", "PlayerTokenIsRequired");
        m.put("You are already in a game room. Leave current room first.", "YouAreAlreadyInAGameRoomLeaveCurrentRoomFirst");
        m.put("You cannot create a game room while in a matchmaking lobby.", "YouCannotCreateAGameRoomWhileInAMatchmakingLobby");
        m.put("Failed to create room", "FailedToCreateRoom");
        m.put("Failed to list rooms", "FailedToListRooms");
        m.put("Only host can update room password", "OnlyHostCanUpdateRoomPassword");
        m.put("Failed to update password", "FailedToUpdatePassword");
        m.put("You are already in another room", "YouAreAlreadyInAnotherRoom");
        m.put("Room not found", "RoomNotFound");
        m.put("Room inactive", "RoomInactive");
        m.put("Room is full", "RoomIsFull");
        m.put("Incorrect password", "IncorrectPassword");
        m.put("You are not in any room", "YouAreNotInAnyRoom");
        m.put("You are not in this room", "YouAreNotInThisRoom");
        m.put("Players are not allowed to leave this room", "PlayersAreNotAllowedToLeaveThisRoom");
        m.put("Player is not in any room", "PlayerIsNotInAnyRoom");
        m.put("Missing action_type", "MissingActionType");
        m.put("request_data_json must be a string", "RequestDataJsonMustBeAString");
        m.put("Only host can view pending actions", "OnlyHostCanViewPendingActions");
        m.put("Only host can complete actions", "OnlyHostCanCompleteActions");
        m.put("Status is required", "StatusIsRequired");
        m.put("response_data_json must be a string", "ResponseDataJsonMustBeAString");
        m.put("response_data is not valid JSON", "ResponseDataIsNotValidJson");
        m.put("Action not found or already processed", "ActionNotFoundOrAlreadyProcessed");
        m.put("Missing required field: target_players", "MissingRequiredFieldTargetPlayers");
        m.put("Missing required field: type", "MissingRequiredFieldType");
        m.put("Missing required field: target_players_ids", "MissingRequiredFieldTargetPlayersIds");
        m.put("Invalid target players ids", "InvalidTargetPlayersIds");
        m.put("No valid target players found", "NoValidTargetPlayersFound");
        m.put("Invalid target players", "InvalidTargetPlayers");
        m.put("Failed to send updates", "FailedToSendUpdates");
        m.put("No valid source players found", "NoValidSourcePlayersFound");
        m.put("Invalid from players", "InvalidFromPlayers");
        m.put("Only host can kick players", "OnlyHostCanKickPlayers");
        m.put("You cannot kick yourself", "YouCannotKickYourself");
        m.put("Missing required field: player_id", "MissingRequiredFieldPlayerId");
        m.put("Player not found in this room", "PlayerNotFoundInThisRoom");
        m.put("Failed to kick player", "FailedToKickPlayer");
        m.put("Only host can stop game room", "OnlyHostCanStopTheGameRoom");
        m.put("Failed to stop game room", "FailedToStopGameRoom");

        // Leaderboard errors
        m.put("Method not allowed. Use POST.", "MethodNotAllowedUsePost");
        m.put("api_token is required", "ApiTokenIsRequired");
        m.put("Invalid or expired api_token", "InvalidOrExpiredApiToken");
        m.put("Invalid JSON body", "InvalidJsonBody");
        m.put("sort_by must be a non-empty array of field names", "SortByMustBeANonEmptyArrayOfFieldNames");
        m.put("limit must be between 1 and 100", "LimitMustBeBetween1And100");
        m.put("No valid sort fields provided after sanitization", "NoValidSortFieldsProvidedAfterSanitization");
        m.put("Server error", "ServerError");

        // Matchmaking errors
        m.put("Failed to list matchmaking lobbies", "FailedToListMatchmakingLobbies");
        m.put("You are already in a matchmaking lobby", "YouAreAlreadyInAMatchmakingLobby");
        m.put("You cannot create matchmaking while in a game room. Leave room first.", "YouCannotCreateMatchmakingWhileInAGameRoomLeaveRoomFirst");
        m.put("Missing required field: max_players", "MissingRequiredFieldMaxPlayers");
        m.put("Failed to create matchmaking lobby", "FailedToCreateMatchmakingLobby");
        m.put("Missing required parameter: matchmakingId", "MissingRequiredParameterMatchmakingId");
        m.put("You already have a pending request to this matchmaking lobby", "YouAlreadyHaveAPendingRequestToThisMatchmakingLobby");
        m.put("Matchmaking lobby not found or already started", "MatchmakingLobbyNotFoundOrAlreadyStarted");
        m.put("Matchmaking lobby is full", "MatchmakingLobbyIsFull");
        m.put("This matchmaking lobby requires host approval. Use /request endpoint instead.", "ThisMatchmakingLobbyRequiresHostApprovalUseRequestEndpointInstead");
        m.put("You are not in any matchmaking lobby", "YouAreNotInAnyMatchmakingLobby");
        m.put("Players are not allowed to leave this matchmaking lobby", "PlayersAreNotAllowedToLeaveThisMatchmakingLobby");
        m.put("Failed to get players", "FailedToGetPlayers");
        m.put("You are not in a matchmaking lobby", "YouAreNotInAMatchmakingLobby");
        m.put("Only host can remove matchmaking lobby", "OnlyHostCanRemoveMatchmakingLobby");
        m.put("Failed to remove matchmaking lobby", "FailedToRemoveMatchmakingLobby");
        m.put("Failed to get matchmaking status", "FailedToGetMatchmakingStatus");
        m.put("Missing required parameter: requestId", "MissingRequiredParameterRequestId");
        m.put("Request not found or you are not the requester", "RequestNotFoundOrYouAreNotTheRequester");
        m.put("Missing required fields: requestId and action", "MissingRequiredFieldsRequestIdAndAction");
        m.put("Action must be \"approve\" or \"reject\"", "ActionMustBeApproveOrReject");
        m.put("Request not found or already processed", "RequestNotFoundOrAlreadyProcessed");
        m.put("Only the host can respond to join requests", "OnlyTheHostCanRespondToJoinRequests");
        m.put("Only host can start matchmaking", "OnlyHostCanStartMatchmaking");
        m.put("Lobby must be full to start (strict_full enabled)", "LobbyMustBeFullToStartStrictFullEnabled");
        m.put("Cannot kick players from matchmaking after it has been started", "CannotKickFromStartedMatchmaking");
        m.put("Player not found in this matchmaking lobby", "PlayerNotFoundInThisMatchmakingLobby");
        m.put("Failed to kick player", "FailedToKickPlayer");
        m.put("Only host can stop matchmaking lobby", "OnlyHostCanStopMatchmakingLobby");
        m.put("Cannot stop matchmaking lobby after it has been started", "CannotStopMatchmakingLobbyAfterItHasBeenStarted");
        m.put("Failed to stop matchmaking lobby", "FailedToStopMatchmakingLobby");
        m.put("Only host can update matchmaking password", "OnlyHostCanUpdateMatchmakingPassword");
        m.put("Cannot change password after matchmaking has started", "CannotChangePasswordAfterMatchmakingHasStarted");

        // Time errors
        m.put("API key is required", "ApiKeyIsRequired");
        m.put("Invalid API key", "InvalidApiKey");

        // Realtime errors
        m.put("Player is not in a realtime-enabled room", "PlayerIsNotInARealtimeEnabledRoom");
        m.put("Failed to generate realtime token", "FailedToGenerateRealtimeToken");

        ERROR_MAPPINGS = Collections.unmodifiableMap(m);
    }

    /**
     * Converts an API error message string to the corresponding enum value.
     *
     * @param <T> The error enum type to convert to.
     * @param errorMessage The error message from the API.
     * @param enumClass The error enum class (its first constant is used as the Unknown fallback).
     * @return The corresponding enum value, or the first constant (Unknown) if no match is found.
     */
    public static <T extends Enum<T>> T convertToEnum(String errorMessage, Class<T> enumClass) {
        T unknown = enumClass.getEnumConstants()[0];

        if (errorMessage == null || errorMessage.isEmpty()) {
            return unknown;
        }

        String enumName = ERROR_MAPPINGS.get(errorMessage);
        if (enumName != null) {
            try {
                return Enum.valueOf(enumClass, enumName);
            } catch (IllegalArgumentException ignored) {
                // The mapped constant does not exist on this enum; fall through
            }
        }

        return unknown;
    }

    /**
     * Converts an error enum value back to its user-friendly error message.
     *
     * @param errorType The error enum value to convert.
     * @return The user-friendly error message, or "Unknown error" if no match is found.
     */
    public static String getErrorMessage(Enum<?> errorType) {
        if (errorType == null) {
            return "Unknown error";
        }
        for (Map.Entry<String, String> entry : ERROR_MAPPINGS.entrySet()) {
            if (entry.getValue().equals(errorType.name())) {
                return entry.getKey();
            }
        }
        return "Unknown error";
    }
}
