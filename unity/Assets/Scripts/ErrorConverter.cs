using System;
using System.Collections.Generic;
using System.Linq;

namespace michitai
{
    // ====================== ERROR CONVERTER ======================
    
    public static class ErrorConverter
    {
        private static readonly Dictionary<string, string> ErrorMappings = new()
        {
            // Common errors
            { "API token is required", nameof(ECommonError.ApiTokenIsRequired) },
            { "Invalid API token", nameof(ECommonError.InvalidApiToken) },
            { "Method not allowed", nameof(ECommonError.MethodNotAllowed) },
            { "Internal server error", nameof(ECommonError.InternalServerError) },
            { "Failed to deserialize response", nameof(ECommonError.FailedToDeserializeResponse) },
            { "Invalid endpoint", nameof(ECommonError.InvalidEndpoint) },
            { "Database error", nameof(ECommonError.DatabaseError) },
            { "An unexpected error occurred", nameof(ECommonError.AnUnexpectedErrorOccurred) },
            
            // Game data errors
            { "Game player token is required", nameof(EGameDataPlayerGetError.GamePlayerTokenIsRequired) },
            { "Invalid game player token", nameof(EGameDataPlayerGetError.InvalidGamePlayerToken) },
            { "Player does not belong to this game", nameof(EGameDataPlayerGetError.PlayerDoesNotBelongToThisGame) },
            { "API private token is required", nameof(EGameDataGameUpdateError.ApiPrivateTokenIsRequired) },
            { "Invalid API token or private token", nameof(EGameDataGameUpdateError.InvalidApiTokenOrPrivateToken) },
            { "Invalid player or does not belong to game", nameof(EGameDataPlayerUpdateError.InvalidPlayerOrDoesNotBelongToGame) },
            
            // Player errors
            { "Player name is required", nameof(EPlayerRegisterError.PlayerNameIsRequired) },
            { "Failed to register player", nameof(EPlayerRegisterError.FailedToRegisterPlayer) },
            { "API token and game player token are required", nameof(EPlayerLoginError.ApiTokenAndGamePlayerTokenAreRequired) },
            { "Invalid game player token", nameof(EPlayerLoginError.InvalidGamePlayerToken) },
            { "Invalid player token", nameof(EPlayerHeartbeatError.InvalidPlayerToken) },
            { "Failed to update heartbeat", nameof(EPlayerHeartbeatError.FailedToUpdateHeartbeat) },
            { "New name is required", nameof(EPlayerRenameError.NewNameIsRequired) },
            { "Player name must be between 2 and 50 characters", nameof(EPlayerRenameError.PlayerNameMustBeBetween2And50Characters) },
            { "Failed to update player name", nameof(EPlayerRenameError.FailedToUpdatePlayerName) },
            { "API token and private token are required", nameof(EPlayerListError.ApiTokenAndPrivateTokenAreRequired) },
            { "Invalid API credentials", nameof(EPlayerListError.InvalidApiCredentials) },
            
            // Room errors
            { "Invalid player token", nameof(ERoomCreateError.InvalidPlayerToken) },
            { "Player token is required", nameof(ERoomCreateError.PlayerTokenIsRequired) },
            { "You are already in a game room. Leave current room first.", nameof(ERoomCreateError.YouAreAlreadyInAGameRoomLeaveCurrentRoomFirst) },
            { "You cannot create a game room while in a matchmaking lobby.", nameof(ERoomCreateError.YouCannotCreateAGameRoomWhileInAMatchmakingLobby) },
            { "Failed to create room", nameof(ERoomCreateError.FailedToCreateRoom) },
            { "Failed to list rooms", nameof(ERoomListError.FailedToListRooms) },
            { "You are already in another room", nameof(ERoomJoinError.YouAreAlreadyInAnotherRoom) },
            { "Room not found", nameof(ERoomJoinError.RoomNotFound) },
            { "Room inactive", nameof(ERoomJoinError.RoomInactive) },
            { "Room is full", nameof(ERoomJoinError.RoomIsFull) },
            { "Incorrect password", nameof(ERoomJoinError.IncorrectPassword) },
            { "You are not in any room", nameof(ERoomPlayersError.YouAreNotInAnyRoom) },
            { "You are not in this room", nameof(ERoomLeaveError.YouAreNotInThisRoom) },
            { "Players are not allowed to leave this room", nameof(ERoomLeaveError.PlayersAreNotAllowedToLeaveThisRoom) },
            { "Player is not in any room", nameof(ERoomHeartbeatError.PlayerIsNotInAnyRoom) },
            { "Missing action_type", nameof(ERoomActionsError.MissingActionType) },
            { "request_data_json must be a string", nameof(ERoomActionsError.RequestDataJsonMustBeAString) },
            { "Only host can view pending actions", nameof(ERoomActionsPendingError.OnlyHostCanViewPendingActions) },
            { "Only host can complete actions", nameof(ERoomActionsCompleteError.OnlyHostCanCompleteActions) },
            { "Status is required", nameof(ERoomActionsCompleteError.StatusIsRequired) },
            { "response_data_json must be a string", nameof(ERoomActionsCompleteError.ResponseDataJsonMustBeAString) },
            { "response_data is not valid JSON", nameof(ERoomActionsCompleteError.ResponseDataIsNotValidJson) },
            { "Action not found or already processed", nameof(ERoomActionsCompleteError.ActionNotFoundOrAlreadyProcessed) },
            { "Only host can send updates", nameof(ERoomUpdatesError.OnlyHostCanSendUpdates) },
            { "Missing required field: target_players", nameof(ERoomUpdatesError.MissingRequiredFieldTargetPlayers) },
            { "Missing required field: type", nameof(ERoomUpdatesError.MissingRequiredFieldType) },
            { "Missing required field: target_players_ids", nameof(ERoomUpdatesError.MissingRequiredFieldTargetPlayersIds) },
            { "Invalid target players ids", nameof(ERoomUpdatesError.InvalidTargetPlayersIds) },
            { "No valid target players found", nameof(ERoomUpdatesError.NoValidTargetPlayersFound) },
            { "Invalid target players", nameof(ERoomUpdatesError.InvalidTargetPlayers) },
            { "Failed to send updates", nameof(ERoomUpdatesError.FailedToSendUpdates) },
            
            // Leaderboard errors
            { "Method not allowed. Use POST.", nameof(ELeaderboardError.MethodNotAllowedUsePost) },
            { "api_token is required", nameof(ELeaderboardError.ApiTokenIsRequired) },
            { "Invalid or expired api_token", nameof(ELeaderboardError.InvalidOrExpiredApiToken) },
            { "Invalid JSON body", nameof(ELeaderboardError.InvalidJsonBody) },
            { "sort_by must be a non-empty array of field names", nameof(ELeaderboardError.SortByMustBeANonEmptyArrayOfFieldNames) },
            { "limit must be between 1 and 1000", nameof(ELeaderboardError.LimitMustBeBetween1And1000) },
            { "No valid sort fields provided after sanitization", nameof(ELeaderboardError.NoValidSortFieldsProvidedAfterSanitization) },
            { "Database error", nameof(ELeaderboardError.DatabaseError) },
            { "Server error", nameof(ELeaderboardError.ServerError) },
            
            // Matchmaking errors
            { "You are already in a matchmaking lobby", nameof(EMatchmakingCreateError.YouAreAlreadyInAMatchmakingLobby) },
            { "You cannot create matchmaking while in a game room. Leave room first.", nameof(EMatchmakingCreateError.YouCannotCreateMatchmakingWhileInAGameRoomLeaveRoomFirst) },
            { "Missing required field: max_players", nameof(EMatchmakingCreateError.MissingRequiredFieldMaxPlayers) },
            { "Failed to create matchmaking lobby", nameof(EMatchmakingCreateError.FailedToCreateMatchmakingLobby) },
            { "Missing required parameter: matchmakingId", nameof(EMatchmakingRequestError.MissingRequiredParameterMatchmakingId) },
            { "You already have a pending request to this matchmaking lobby", nameof(EMatchmakingRequestError.YouAlreadyHaveAPendingRequestToThisMatchmakingLobby) },
            { "Matchmaking lobby not found or already started", nameof(EMatchmakingRequestError.MatchmakingLobbyNotFoundOrAlreadyStarted) },
            { "Matchmaking lobby is full", nameof(EMatchmakingRequestError.MatchmakingLobbyIsFull) },
            { "This matchmaking lobby requires host approval. Use /request endpoint instead.", nameof(EMatchmakingJoinError.ThisMatchmakingLobbyRequiresHostApprovalUseRequestEndpointInstead) },
            { "You are not in any matchmaking lobby", nameof(EMatchmakingLeaveError.YouAreNotInAnyMatchmakingLobby) },
            { "Players are not allowed to leave this matchmaking lobby", nameof(EMatchmakingLeaveError.PlayersAreNotAllowedToLeaveThisMatchmakingLobby) },
            { "Failed to get players", nameof(EMatchmakingPlayersError.FailedToGetPlayers) },
            { "You are not in a matchmaking lobby", nameof(EMatchmakingRemoveError.YouAreNotInAMatchmakingLobby) },
            { "Only host can remove matchmaking lobby", nameof(EMatchmakingRemoveError.OnlyHostCanRemoveMatchmakingLobby) },
            { "Failed to remove matchmaking lobby", nameof(EMatchmakingRemoveError.FailedToRemoveMatchmakingLobby) },
            { "Failed to get matchmaking status", nameof(EMatchmakingCurrentError.FailedToGetMatchmakingStatus) },
            { "Missing required parameter: requestId", nameof(EMatchmakingStatusError.MissingRequiredParameterRequestId) },
            { "Request not found or you are not the requester", nameof(EMatchmakingStatusError.RequestNotFoundOrYouAreNotTheRequester) },
            { "Missing required fields: requestId and action", nameof(EMatchmakingResponseError.MissingRequiredFieldsRequestIdAndAction) },
            { "Action must be \"approve\" or \"reject\"", nameof(EMatchmakingResponseError.ActionMustBeApproveOrReject) },
            { "Request not found or already processed", nameof(EMatchmakingResponseError.RequestNotFoundOrAlreadyProcessed) },
            { "Only the host can respond to join requests", nameof(EMatchmakingResponseError.OnlyTheHostCanRespondToJoinRequests) },
            { "You are not in a matchmaking lobby", nameof(EMatchmakingStartError.YouAreNotInAMatchmakingLobby) },
            { "Only host can start matchmaking", nameof(EMatchmakingStartError.OnlyHostCanStartMatchmaking) },
            { "Lobby must be full to start (strict_full enabled)", nameof(EMatchmakingStartError.LobbyMustBeFullToStartStrictFullEnabled) },
            
            // Time errors
            { "API key is required", nameof(ETimeError.ApiKeyIsRequired) },
            { "Invalid API key", nameof(ETimeError.InvalidApiKey) }
        };

        public static T ConvertToEnum<T>(string errorMessage) where T : Enum, IConvertible
        {
            if (string.IsNullOrEmpty(errorMessage))
                return (T)Enum.ToObject(typeof(T), 0); // Unknown

            // Try to find exact match first
            if (ErrorMappings.TryGetValue(errorMessage, out string enumName))
            {
                if (Enum.TryParse(typeof(T), enumName, out object result))
                    return (T)result!;
            }

            // Try case-insensitive search
            var mapping = ErrorMappings.FirstOrDefault(kvp => 
                kvp.Value.Equals(typeof(T).Name, StringComparison.OrdinalIgnoreCase));
            
            if (mapping.Key != null && Enum.TryParse(typeof(T), mapping.Value, out object fallbackResult))
                return (T)fallbackResult!;

            // Return Unknown as fallback
            return (T)Enum.ToObject(typeof(T), 0);
        }

        public static string GetErrorMessage<T>(T errorType) where T : Enum, IConvertible
        {
            var mapping = ErrorMappings.FirstOrDefault(kvp => kvp.Value == errorType.ToString());
            return mapping.Key ?? "Unknown error";
        }
    }
}
