"""Typed error enums mirroring the error strings returned by each endpoint."""

from __future__ import annotations

import enum


class CommonError(enum.Enum):
    """Errors that apply to most endpoints."""

    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    MethodNotAllowed = enum.auto()
    InternalServerError = enum.auto()
    FailedToDeserializeResponse = enum.auto()
    InvalidEndpoint = enum.auto()
    DatabaseError = enum.auto()
    AnUnexpectedErrorOccurred = enum.auto()
    YouAreBanned = enum.auto()


# ====================== game_data.php ======================

class GameDataGameGetError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()


class GameDataPlayerGetError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    GamePlayerTokenIsRequired = enum.auto()
    InvalidGamePlayerToken = enum.auto()
    PlayerDoesNotBelongToThisGame = enum.auto()


class GameDataGameUpdateError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiTokenOrPrivateToken = enum.auto()
    ApiPrivateTokenIsRequired = enum.auto()


class GameDataPlayerUpdateError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    GamePlayerTokenIsRequired = enum.auto()
    InvalidPlayerOrDoesNotBelongToGame = enum.auto()


# ====================== game_players.php ======================

class PlayerRegisterError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    PlayerNameIsRequired = enum.auto()
    FailedToRegisterPlayer = enum.auto()


class PlayerLoginError(enum.Enum):
    Unknown = 0
    ApiTokenAndGamePlayerTokenAreRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidGamePlayerToken = enum.auto()
    YouAreBanned = enum.auto()


class PlayerHeartbeatError(enum.Enum):
    Unknown = 0
    ApiTokenAndGamePlayerTokenAreRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    FailedToUpdateHeartbeat = enum.auto()


class PlayerLogoutError(enum.Enum):
    Unknown = 0
    ApiTokenAndGamePlayerTokenAreRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()


class PlayerRenameError(enum.Enum):
    Unknown = 0
    ApiTokenAndGamePlayerTokenAreRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    NewNameIsRequired = enum.auto()
    PlayerNameMustBeBetween2And50Characters = enum.auto()
    FailedToUpdatePlayerName = enum.auto()


class PlayerListError(enum.Enum):
    Unknown = 0
    ApiTokenAndPrivateTokenAreRequired = enum.auto()
    InvalidApiCredentials = enum.auto()


class PlayerBanError(enum.Enum):
    Unknown = 0
    ApiTokenAndPrivateTokenAreRequired = enum.auto()
    InvalidApiCredentials = enum.auto()
    PlayerIdIsRequired = enum.auto()
    BanDurationIsRequired = enum.auto()
    InvalidBanDuration = enum.auto()
    PlayerNotFoundOrDoesNotBelongToThisGame = enum.auto()


class PlayerUnbanError(enum.Enum):
    Unknown = 0
    ApiTokenAndPrivateTokenAreRequired = enum.auto()
    InvalidApiCredentials = enum.auto()
    PlayerIdIsRequired = enum.auto()
    PlayerNotFoundOrDoesNotBelongToThisGame = enum.auto()


# ====================== game_room.php ======================

class RoomCreateError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreAlreadyInAGameRoomLeaveCurrentRoomFirst = enum.auto()
    YouCannotCreateAGameRoomWhileInAMatchmakingLobby = enum.auto()
    FailedToCreateRoom = enum.auto()


class RoomListError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    YouAreBanned = enum.auto()
    FailedToListRooms = enum.auto()


class RoomJoinError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreAlreadyInAnotherRoom = enum.auto()
    RoomNotFound = enum.auto()
    RoomInactive = enum.auto()
    RoomIsFull = enum.auto()
    IncorrectPassword = enum.auto()


class RoomPlayersError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAnyRoom = enum.auto()


class RoomLeaveError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAnyRoom = enum.auto()
    YouAreNotInThisRoom = enum.auto()
    RoomNotFound = enum.auto()
    PlayersAreNotAllowedToLeaveThisRoom = enum.auto()


class RoomHeartbeatError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    PlayerIsNotInAnyRoom = enum.auto()
    FailedToUpdateHeartbeat = enum.auto()


class RoomActionsError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    MissingActionType = enum.auto()
    RequestDataJsonMustBeAString = enum.auto()
    PlayerIsNotInAnyRoom = enum.auto()


class RoomActionsPollError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    PlayerIsNotInAnyRoom = enum.auto()


class RoomActionsPendingError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    OnlyHostCanViewPendingActions = enum.auto()
    YouAreNotInAnyRoom = enum.auto()


class RoomActionsCompleteError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    OnlyHostCanCompleteActions = enum.auto()
    StatusIsRequired = enum.auto()
    ResponseDataJsonMustBeAString = enum.auto()
    ResponseDataIsNotValidJson = enum.auto()
    ActionNotFoundOrAlreadyProcessed = enum.auto()


class RoomUpdatesError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    PlayerIsNotInAnyRoom = enum.auto()
    MissingRequiredFieldTargetPlayers = enum.auto()
    MissingRequiredFieldType = enum.auto()
    MissingRequiredFieldTargetPlayersIds = enum.auto()
    InvalidTargetPlayersIds = enum.auto()
    NoValidTargetPlayersFound = enum.auto()
    InvalidTargetPlayers = enum.auto()
    FailedToSendUpdates = enum.auto()


class RoomUpdatesPollError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    PlayerIsNotInAnyRoom = enum.auto()
    NoValidSourcePlayersFound = enum.auto()
    InvalidFromPlayers = enum.auto()


class RoomCurrentError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()


class RoomKickError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAnyGameRoom = enum.auto()
    OnlyHostCanKickPlayers = enum.auto()
    YouCannotKickYourself = enum.auto()
    MissingRequiredFieldPlayerId = enum.auto()
    PlayerNotFoundInThisRoom = enum.auto()
    FailedToKickPlayer = enum.auto()


class RoomStopError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAnyGameRoom = enum.auto()
    OnlyHostCanStopTheGameRoom = enum.auto()
    FailedToStopGameRoom = enum.auto()


class RoomPasswordError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAnyRoom = enum.auto()
    OnlyHostCanUpdateRoomPassword = enum.auto()
    FailedToUpdatePassword = enum.auto()
    YouAreBanned = enum.auto()


# ====================== leaderboard.php ======================

class LeaderboardError(enum.Enum):
    Unknown = 0
    MethodNotAllowedUsePost = enum.auto()
    ApiTokenIsRequired = enum.auto()
    InvalidOrExpiredApiToken = enum.auto()
    InvalidJsonBody = enum.auto()
    SortByMustBeANonEmptyArrayOfFieldNames = enum.auto()
    LimitMustBeBetween1And100 = enum.auto()
    NoValidSortFieldsProvidedAfterSanitization = enum.auto()
    DatabaseError = enum.auto()
    ServerError = enum.auto()


# ====================== matchmaking.php ======================

class MatchmakingListError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    YouAreBanned = enum.auto()
    FailedToListMatchmakingLobbies = enum.auto()


class MatchmakingCreateError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreAlreadyInAMatchmakingLobby = enum.auto()
    YouCannotCreateMatchmakingWhileInAGameRoomLeaveRoomFirst = enum.auto()
    MissingRequiredFieldMaxPlayers = enum.auto()
    FailedToCreateMatchmakingLobby = enum.auto()


class MatchmakingRequestError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    MissingRequiredParameterMatchmakingId = enum.auto()
    YouAreAlreadyInAMatchmakingLobby = enum.auto()
    YouAlreadyHaveAPendingRequestToThisMatchmakingLobby = enum.auto()
    MatchmakingLobbyNotFoundOrAlreadyStarted = enum.auto()
    MatchmakingLobbyIsFull = enum.auto()


class MatchmakingJoinError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    MissingRequiredParameterMatchmakingId = enum.auto()
    YouAreAlreadyInAMatchmakingLobby = enum.auto()
    MatchmakingLobbyNotFoundOrAlreadyStarted = enum.auto()
    ThisMatchmakingLobbyRequiresHostApprovalUseRequestEndpointInstead = enum.auto()
    MatchmakingLobbyIsFull = enum.auto()


class MatchmakingLeaveError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAnyMatchmakingLobby = enum.auto()
    PlayersAreNotAllowedToLeaveThisMatchmakingLobby = enum.auto()


class MatchmakingPlayersError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAnyMatchmakingLobby = enum.auto()
    FailedToGetPlayers = enum.auto()


class MatchmakingHeartbeatError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAnyMatchmakingLobby = enum.auto()
    FailedToUpdateHeartbeat = enum.auto()


class MatchmakingRemoveError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAMatchmakingLobby = enum.auto()
    OnlyHostCanRemoveMatchmakingLobby = enum.auto()
    FailedToRemoveMatchmakingLobby = enum.auto()


class MatchmakingCurrentError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    FailedToGetMatchmakingStatus = enum.auto()


class MatchmakingStatusError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    MissingRequiredParameterRequestId = enum.auto()
    RequestNotFoundOrYouAreNotTheRequester = enum.auto()


class MatchmakingResponseError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    MissingRequiredFieldsRequestIdAndAction = enum.auto()
    ActionMustBeApproveOrReject = enum.auto()
    RequestNotFoundOrAlreadyProcessed = enum.auto()
    OnlyTheHostCanRespondToJoinRequests = enum.auto()
    MatchmakingLobbyNotFoundOrAlreadyStarted = enum.auto()
    MatchmakingLobbyIsFull = enum.auto()


class MatchmakingStartError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAMatchmakingLobby = enum.auto()
    OnlyHostCanStartMatchmaking = enum.auto()
    MatchmakingLobbyNotFoundOrAlreadyStarted = enum.auto()
    LobbyMustBeFullToStartStrictFullEnabled = enum.auto()


class MatchmakingKickError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAMatchmakingLobby = enum.auto()
    OnlyHostCanKickPlayers = enum.auto()
    YouCannotKickYourself = enum.auto()
    CannotKickFromStartedMatchmaking = enum.auto()
    MissingRequiredFieldPlayerId = enum.auto()
    PlayerNotFoundInThisMatchmakingLobby = enum.auto()
    FailedToKickPlayer = enum.auto()


class MatchmakingStopError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAMatchmakingLobby = enum.auto()
    OnlyHostCanStopMatchmakingLobby = enum.auto()
    CannotStopMatchmakingLobbyAfterItHasBeenStarted = enum.auto()
    FailedToStopMatchmakingLobby = enum.auto()


class MatchmakingPasswordError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    YouAreNotInAMatchmakingLobby = enum.auto()
    OnlyHostCanUpdateMatchmakingPassword = enum.auto()
    CannotChangePasswordAfterMatchmakingHasStarted = enum.auto()
    FailedToUpdatePassword = enum.auto()
    YouAreBanned = enum.auto()


# ====================== time.php ======================

class TimeError(enum.Enum):
    Unknown = 0
    MethodNotAllowed = enum.auto()
    ApiKeyIsRequired = enum.auto()
    InvalidApiKey = enum.auto()
    InternalServerError = enum.auto()


# ====================== realtime.php ======================

class RealtimeTokenError(enum.Enum):
    Unknown = 0
    ApiTokenIsRequired = enum.auto()
    InvalidApiToken = enum.auto()
    InvalidPlayerToken = enum.auto()
    PlayerTokenIsRequired = enum.auto()
    PlayerIsNotInARealtimeEnabledRoom = enum.auto()
    FailedToGenerateRealtimeToken = enum.auto()
