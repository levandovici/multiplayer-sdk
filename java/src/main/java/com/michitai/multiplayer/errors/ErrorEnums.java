package com.michitai.multiplayer.errors;

/**
 * Typed error enums for all API endpoints.
 * The first constant of each enum is always the Unknown fallback.
 */
public final class ErrorEnums {
    private ErrorEnums() {
    }

    // ====================== BASE ERROR ENUMS ======================

    /** Common default errors that apply to most endpoints. */
    public enum ECommonError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        MethodNotAllowed,
        InternalServerError,
        FailedToDeserializeResponse,
        InvalidEndpoint,
        DatabaseError,
        AnUnexpectedErrorOccurred,
        YouAreBanned
    }

    // ====================== GAME_DATA.PHP ERRORS ======================

    public enum EGameDataGameGetError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken
    }

    public enum EGameDataPlayerGetError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        GamePlayerTokenIsRequired,
        InvalidGamePlayerToken,
        PlayerDoesNotBelongToThisGame
    }

    public enum EGameDataGameUpdateError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiTokenOrPrivateToken,
        ApiPrivateTokenIsRequired
    }

    public enum EGameDataPlayerUpdateError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        GamePlayerTokenIsRequired,
        InvalidPlayerOrDoesNotBelongToGame
    }

    // ====================== GAME_PLAYERS.PHP ERRORS ======================

    public enum EPlayerRegisterError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        PlayerNameIsRequired,
        FailedToRegisterPlayer
    }

    public enum EPlayerLoginError {
        Unknown,
        ApiTokenAndGamePlayerTokenAreRequired,
        InvalidApiToken,
        InvalidGamePlayerToken
    }

    public enum EPlayerHeartbeatError {
        Unknown,
        ApiTokenAndGamePlayerTokenAreRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        FailedToUpdateHeartbeat
    }

    public enum EPlayerLogoutError {
        Unknown,
        ApiTokenAndGamePlayerTokenAreRequired,
        InvalidApiToken,
        InvalidPlayerToken
    }

    public enum EPlayerRenameError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        NewNameIsRequired,
        PlayerNameMustBeBetween2And50Characters,
        FailedToUpdatePlayerName
    }

    public enum EPlayerListError {
        Unknown,
        ApiTokenAndPrivateTokenAreRequired,
        InvalidApiCredentials
    }

    public enum EPlayerBanError {
        Unknown,
        ApiTokenAndPrivateTokenAreRequired,
        InvalidApiCredentials,
        PlayerIdIsRequired,
        BanDurationIsRequired,
        InvalidBanDuration,
        PlayerNotFoundOrDoesNotBelongToThisGame
    }

    public enum EPlayerUnbanError {
        Unknown,
        ApiTokenAndPrivateTokenAreRequired,
        InvalidApiCredentials,
        PlayerIdIsRequired,
        PlayerNotFoundOrDoesNotBelongToThisGame
    }

    // ====================== GAME_ROOM.PHP ERRORS ======================

    public enum ERoomCreateError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreAlreadyInAGameRoomLeaveCurrentRoomFirst,
        YouCannotCreateAGameRoomWhileInAMatchmakingLobby,
        FailedToCreateRoom
    }

    public enum ERoomListError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        YouAreBanned,
        FailedToListRooms
    }

    public enum ERoomJoinError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreAlreadyInAnotherRoom,
        RoomNotFound,
        RoomInactive,
        RoomIsFull,
        IncorrectPassword
    }

    public enum ERoomPlayersError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyRoom
    }

    public enum ERoomLeaveError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyRoom,
        YouAreNotInThisRoom,
        RoomNotFound,
        PlayersAreNotAllowedToLeaveThisRoom
    }

    public enum ERoomHeartbeatError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        PlayerIsNotInAnyRoom,
        FailedToUpdateHeartbeat
    }

    public enum ERoomActionsError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingActionType,
        RequestDataJsonMustBeAString,
        PlayerIsNotInAnyRoom
    }

    public enum ERoomActionsPollError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        PlayerIsNotInAnyRoom
    }

    public enum ERoomActionsPendingError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        OnlyHostCanViewPendingActions,
        YouAreNotInAnyRoom
    }

    public enum ERoomActionsCompleteError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        OnlyHostCanCompleteActions,
        StatusIsRequired,
        ResponseDataJsonMustBeAString,
        ResponseDataIsNotValidJson,
        ActionNotFoundOrAlreadyProcessed
    }

    public enum ERoomUpdatesError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
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

    public enum ERoomUpdatesPollError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        PlayerIsNotInAnyRoom,
        NoValidSourcePlayersFound,
        InvalidFromPlayers
    }

    public enum ERoomCurrentError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired
    }

    public enum ERoomKickError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyGameRoom,
        OnlyHostCanKickPlayers,
        YouCannotKickYourself,
        MissingRequiredFieldPlayerId,
        PlayerNotFoundInThisRoom,
        FailedToKickPlayer
    }

    public enum ERoomStopError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyGameRoom,
        OnlyHostCanStopTheGameRoom,
        FailedToStopGameRoom
    }

    public enum ERoomPasswordError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyRoom,
        OnlyHostCanUpdateRoomPassword,
        FailedToUpdatePassword,
        YouAreBanned
    }

    // ====================== LEADERBOARD.PHP ERRORS ======================

    public enum ELeaderboardError {
        Unknown,
        MethodNotAllowedUsePost,
        ApiTokenIsRequired,
        InvalidOrExpiredApiToken,
        InvalidJsonBody,
        SortByMustBeANonEmptyArrayOfFieldNames,
        LimitMustBeBetween1And100,
        NoValidSortFieldsProvidedAfterSanitization,
        DatabaseError,
        ServerError
    }

    // ====================== MATCHMAKING.PHP ERRORS ======================

    public enum EMatchmakingListError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        YouAreBanned,
        FailedToListMatchmakingLobbies
    }

    public enum EMatchmakingCreateError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreAlreadyInAMatchmakingLobby,
        YouCannotCreateMatchmakingWhileInAGameRoomLeaveRoomFirst,
        MissingRequiredFieldMaxPlayers,
        FailedToCreateMatchmakingLobby
    }

    public enum EMatchmakingRequestError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingRequiredParameterMatchmakingId,
        YouAreAlreadyInAMatchmakingLobby,
        YouAlreadyHaveAPendingRequestToThisMatchmakingLobby,
        MatchmakingLobbyNotFoundOrAlreadyStarted,
        MatchmakingLobbyIsFull
    }

    public enum EMatchmakingJoinError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingRequiredParameterMatchmakingId,
        YouAreAlreadyInAMatchmakingLobby,
        MatchmakingLobbyNotFoundOrAlreadyStarted,
        ThisMatchmakingLobbyRequiresHostApprovalUseRequestEndpointInstead,
        MatchmakingLobbyIsFull
    }

    public enum EMatchmakingLeaveError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyMatchmakingLobby,
        PlayersAreNotAllowedToLeaveThisMatchmakingLobby
    }

    public enum EMatchmakingPlayersError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyMatchmakingLobby,
        FailedToGetPlayers
    }

    public enum EMatchmakingHeartbeatError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAnyMatchmakingLobby,
        FailedToUpdateHeartbeat
    }

    public enum EMatchmakingRemoveError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAMatchmakingLobby,
        OnlyHostCanRemoveMatchmakingLobby,
        FailedToRemoveMatchmakingLobby
    }

    public enum EMatchmakingCurrentError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        FailedToGetMatchmakingStatus
    }

    public enum EMatchmakingStatusError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingRequiredParameterRequestId,
        RequestNotFoundOrYouAreNotTheRequester
    }

    public enum EMatchmakingResponseError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        MissingRequiredFieldsRequestIdAndAction,
        ActionMustBeApproveOrReject,
        RequestNotFoundOrAlreadyProcessed,
        OnlyTheHostCanRespondToJoinRequests,
        MatchmakingLobbyNotFoundOrAlreadyStarted,
        MatchmakingLobbyIsFull
    }

    public enum EMatchmakingStartError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAMatchmakingLobby,
        OnlyHostCanStartMatchmaking,
        MatchmakingLobbyNotFoundOrAlreadyStarted,
        LobbyMustBeFullToStartStrictFullEnabled
    }

    public enum EMatchmakingKickError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAMatchmakingLobby,
        OnlyHostCanKickPlayers,
        YouCannotKickYourself,
        CannotKickFromStartedMatchmaking,
        MissingRequiredFieldPlayerId,
        PlayerNotFoundInThisMatchmakingLobby,
        FailedToKickPlayer
    }

    public enum EMatchmakingStopError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAMatchmakingLobby,
        OnlyHostCanStopMatchmakingLobby,
        CannotStopMatchmakingLobbyAfterItHasBeenStarted,
        FailedToStopMatchmakingLobby
    }

    public enum EMatchmakingPasswordError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        YouAreNotInAMatchmakingLobby,
        OnlyHostCanUpdateMatchmakingPassword,
        CannotChangePasswordAfterMatchmakingHasStarted,
        FailedToUpdatePassword,
        YouAreBanned
    }

    // ====================== TIME.PHP ERRORS ======================

    public enum ETimeError {
        Unknown,
        MethodNotAllowed,
        ApiKeyIsRequired,
        InvalidApiKey,
        InternalServerError
    }

    // ====================== REALTIME.PHP ERRORS ======================

    public enum ERealtimeTokenError {
        Unknown,
        ApiTokenIsRequired,
        InvalidApiToken,
        InvalidPlayerToken,
        PlayerTokenIsRequired,
        PlayerIsNotInARealtimeEnabledRoom,
        FailedToGenerateRealtimeToken
    }
}
