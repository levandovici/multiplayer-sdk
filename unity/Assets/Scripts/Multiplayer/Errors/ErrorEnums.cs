using System;

namespace Michitai.Multiplayer.Errors
{
    // ====================== BASE ERROR ENUMS ======================
    
    // Common default errors that apply to most endpoints
    public enum ECommonError
    {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        MethodNotAllowed,
        InternalServerError,
        FailedToDeserializeResponse,
        InvalidEndpoint,
        DatabaseError,
        AnUnexpectedErrorOccurred
    }

    // ====================== GAME_DATA.PHP ERRORS ======================
    
    public enum EGameDataGameGetError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken
    }

    public enum EGameDataPlayerGetError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        GamePlayerTokenIsRequired,
        InvalidGamePlayerToken,
        PlayerDoesNotBelongToThisGame
    }

    public enum EGameDataGameUpdateError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiTokenOrPrivateToken,
        ApiPrivateTokenIsRequired
    }

    public enum EGameDataPlayerUpdateError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        GamePlayerTokenIsRequired,
        InvalidPlayerOrDoesNotBelongToGame
    }

    // ====================== GAME_PLAYERS.PHP ERRORS ======================
    
    public enum EPlayerRegisterError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        PlayerNameIsRequired,
        FailedToRegisterPlayer
    }

    public enum EPlayerLoginError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenAndGamePlayerTokenAreRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidGamePlayerToken
    }

    public enum EPlayerHeartbeatError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenAndGamePlayerTokenAreRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        FailedToUpdateHeartbeat
    }

    public enum EPlayerLogoutError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenAndGamePlayerTokenAreRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken
    }

    public enum EPlayerRenameError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenAndGamePlayerTokenAreRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        NewNameIsRequired,
        PlayerNameMustBeBetween2And50Characters,
        FailedToUpdatePlayerName
    }

    public enum EPlayerListError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenAndPrivateTokenAreRequired,
        InvalidApiCredentials
    }

    // ====================== GAME_ROOM.PHP ERRORS ======================
    
    public enum ERoomCreateError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreAlreadyInAGameRoomLeaveCurrentRoomFirst,
        YouCannotCreateAGameRoomWhileInAMatchmakingLobby,
        FailedToCreateRoom
    }

    public enum ERoomListError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        FailedToListRooms
    }

    public enum ERoomJoinError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreAlreadyInAnotherRoom,
        RoomNotFound,
        RoomInactive,
        RoomIsFull,
        IncorrectPassword
    }

    public enum ERoomPlayersError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyRoom
    }

    public enum ERoomLeaveError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyRoom,
        YouAreNotInThisRoom,
        RoomNotFound,
        PlayersAreNotAllowedToLeaveThisRoom
    }

    public enum ERoomHeartbeatError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        PlayerIsNotInAnyRoom,
        FailedToUpdateHeartbeat
    }

    public enum ERoomActionsError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingActionType,
        RequestDataJsonMustBeAString,
        PlayerIsNotInAnyRoom
    }

    public enum ERoomActionsPollError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        PlayerIsNotInAnyRoom
    }

    public enum ERoomActionsPendingError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        OnlyHostCanViewPendingActions,
        YouAreNotInAnyRoom
    }

    public enum ERoomActionsCompleteError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        OnlyHostCanCompleteActions,
        StatusIsRequired,
        ResponseDataJsonMustBeAString,
        ResponseDataIsNotValidJson,
        ActionNotFoundOrAlreadyProcessed
    }

    public enum ERoomUpdatesError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        PlayerIsNotInAnyRoom,
        MissingRequiredFieldTargetPlayers,
        MissingRequiredFieldType,
        MissingRequiredFieldTargetPlayersIds,
        InvalidTargetPlayersIds,
        NoValidTargetPlayersFound,
        InvalidTargetPlayers,
        FailedToSendUpdates
    }

    public enum ERoomUpdatesPollError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        PlayerIsNotInAnyRoom,
        NoValidSourcePlayersFound,
        InvalidFromPlayers
    }

    public enum ERoomCurrentError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired
    }

    // ====================== LEADERBOARD.PHP ERRORS ======================
    
    public enum ELeaderboardError 
    {
        Unknown = ECommonError.Unknown,
        MethodNotAllowedUsePost,
        ApiTokenIsRequired,
        InvalidOrExpiredApiToken,
        InvalidJsonBody,
        SortByMustBeANonEmptyArrayOfFieldNames,
        LimitMustBeBetween1And1000,
        NoValidSortFieldsProvidedAfterSanitization,
        DatabaseError,
        ServerError
    }

    // ====================== MATCHMAKING.PHP ERRORS ======================
    
    public enum EMatchmakingListError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired
    }

    public enum EMatchmakingCreateError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreAlreadyInAMatchmakingLobby,
        YouCannotCreateMatchmakingWhileInAGameRoomLeaveRoomFirst,
        MissingRequiredFieldMaxPlayers,
        FailedToCreateMatchmakingLobby
    }

    public enum EMatchmakingRequestError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingRequiredParameterMatchmakingId,
        YouAreAlreadyInAMatchmakingLobby,
        YouAlreadyHaveAPendingRequestToThisMatchmakingLobby,
        MatchmakingLobbyNotFoundOrAlreadyStarted,
        MatchmakingLobbyIsFull
    }

    public enum EMatchmakingJoinError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingRequiredParameterMatchmakingId,
        YouAreAlreadyInAMatchmakingLobby,
        MatchmakingLobbyNotFoundOrAlreadyStarted,
        ThisMatchmakingLobbyRequiresHostApprovalUseRequestEndpointInstead,
        MatchmakingLobbyIsFull
    }

    public enum EMatchmakingLeaveError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyMatchmakingLobby,
        PlayersAreNotAllowedToLeaveThisMatchmakingLobby
    }

    public enum EMatchmakingPlayersError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyMatchmakingLobby,
        FailedToGetPlayers
    }

    public enum EMatchmakingHeartbeatError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyMatchmakingLobby,
        FailedToUpdateHeartbeat
    }

    public enum EMatchmakingRemoveError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAMatchmakingLobby,
        OnlyHostCanRemoveMatchmakingLobby,
        FailedToRemoveMatchmakingLobby
    }

    public enum EMatchmakingCurrentError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        FailedToGetMatchmakingStatus
    }

    public enum EMatchmakingStatusError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingRequiredParameterRequestId,
        RequestNotFoundOrYouAreNotTheRequester
    }

    public enum EMatchmakingResponseError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingRequiredFieldsRequestIdAndAction,
        ActionMustBeApproveOrReject,
        RequestNotFoundOrAlreadyProcessed,
        OnlyTheHostCanRespondToJoinRequests,
        MatchmakingLobbyNotFoundOrAlreadyStarted,
        MatchmakingLobbyIsFull
    }

    public enum EMatchmakingStartError 
    {
        Unknown = ECommonError.Unknown,
        ApiTokenIsRequired = ECommonError.ApiTokenIsRequired,
        InvalidApiToken = ECommonError.InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAMatchmakingLobby,
        OnlyHostCanStartMatchmaking,
        MatchmakingLobbyNotFoundOrAlreadyStarted,
        LobbyMustBeFullToStartStrictFullEnabled
    }

    // ====================== TIME.PHP ERRORS ======================
    
    public enum ETimeError 
    {
        Unknown = ECommonError.Unknown,
        MethodNotAllowed = ECommonError.MethodNotAllowed,
        ApiKeyIsRequired,
        InvalidApiKey,
        InternalServerError = ECommonError.InternalServerError
    }
}
