using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace michitai
{
    // ====================== BASE RESPONSE ======================
    public abstract class ApiResponse
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    // ====================== LOGGER ======================
    public interface ILogger
    {
        void Log(string message);
        void Warn(string message);
        void Error(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public virtual void Error(string message)
        {
            Console.WriteLine($"[Error] {message}");
        }

        public virtual void Log(string message)
        {
            Console.WriteLine($"[Log] {message}");
        }

        public virtual void Warn(string message)
        {
            Console.WriteLine($"[Warning] {message}");
        }
    }

    // ====================== MAIN SDK ======================
    public class GameSDK
    {
        private readonly string _apiToken;
        private readonly string _apiPrivateToken;
        private readonly string _baseUrl;
        private readonly HttpClient _http;
        private readonly ILogger? _logger;

        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static class Endpoints
        {
            public const string GamePlayersRegister = "game_players.php/register";
            public const string GamePlayersLogin = "game_players.php/login";
            public const string GamePlayersHeartbeat = "game_players.php/heartbeat";
            public const string GamePlayersLogout = "game_players.php/logout";
            public const string GamePlayersList = "game_players.php/list";

            public const string GameDataGameGet = "game_data.php/game/get";
            public const string GameDataGameUpdate = "game_data.php/game/update";
            public const string GameDataPlayerGet = "game_data.php/player/get";
            public const string GameDataPlayerUpdate = "game_data.php/player/update";

            public const string Time = "time.php";

            public const string GameRoomCreate = "game_room.php/create";
            public const string GameRoomList = "game_room.php/list";
            public const string GameRoomJoin = "game_room.php/{0}/join";
            public const string GameRoomLeave = "game_room.php/leave";
            public const string GameRoomPlayers = "game_room.php/players";
            public const string GameRoomHeartbeat = "game_room.php/heartbeat";
            public const string GameRoomActions = "game_room.php/actions";
            public const string GameRoomActionsPoll = "game_room.php/actions/poll";
            public const string GameRoomActionsPending = "game_room.php/actions/pending";
            public const string GameRoomActionComplete = "game_room.php/actions/{0}/complete";
            public const string GameRoomUpdates = "game_room.php/updates";
            public const string GameRoomUpdatesPoll = "game_room.php/updates/poll";
            public const string GameRoomCurrent = "game_room.php/current";

            public const string MatchmakingList = "matchmaking.php/list";
            public const string MatchmakingCreate = "matchmaking.php/create";
            public const string MatchmakingRequest = "matchmaking.php/{0}/request";
            public const string MatchmakingResponse = "matchmaking.php/{0}/response";
            public const string MatchmakingRequestStatus = "matchmaking.php/{0}/status";
            public const string MatchmakingCurrent = "matchmaking.php/current";
            public const string MatchmakingJoin = "matchmaking.php/{0}/join";
            public const string MatchmakingLeave = "matchmaking.php/leave";
            public const string MatchmakingPlayers = "matchmaking.php/players";
            public const string MatchmakingHeartbeat = "matchmaking.php/heartbeat";
            public const string MatchmakingRemove = "matchmaking.php/remove";
            public const string MatchmakingStart = "matchmaking.php/start";

            public const string Leaderboard = "leaderboard.php";
        }

        public GameSDK(string apiToken, string apiPrivateToken, string baseUrl = "https://api.michitai.com/api",
                       ILogger? logger = null, HttpClient? httpClient = null)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _apiPrivateToken = apiPrivateToken ?? throw new ArgumentNullException(nameof(apiPrivateToken));
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _logger = logger;
            _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        }

        private string Url(string endpoint, string extra = "") => $"{_baseUrl}{endpoint}?api_token={_apiToken}{extra}";

        private async Task<T> Send<T>(HttpMethod method, string url, object? body = null, CancellationToken ct = default) where T : ApiResponse, new()
        {
            var req = new HttpRequestMessage(method, url);
            if (body != null)
            {
                string json = JsonSerializer.Serialize(body, JsonOptions);
                req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var res = await _http.SendAsync(req, ct);
            string responseText = await res.Content.ReadAsStringAsync(ct);

            _logger?.Log($"API Response: {responseText}");

            try
            {
                var response = JsonSerializer.Deserialize<T>(responseText, JsonOptions) ?? new T();

                if (!response.Success)
                {
                    _logger?.Error($"API Error: {response.Error ?? "Unknown error"}");
                    throw new ApiException(response.Error ?? "Unknown API error", responseText);
                }

                return response;
            }
            catch (JsonException ex)
            {
                _logger?.Warn($"JSON Deserialization Error: {ex.Message}. Raw: {responseText}");

                try
                {
                    var error = JsonSerializer.Deserialize<ErrorResponse>(responseText, JsonOptions);
                    if (error != null && !error.Success)
                        throw new ApiException(error.Error ?? "API error", responseText);
                }
                catch { }

                throw new ApiException($"Failed to deserialize response: {ex.Message}", responseText, ex);
            }
        }

        // ==================== PLAYER ====================
        public Task<PlayerRegisterResponse> RegisterPlayer(string name, object? playerData = null, CancellationToken ct = default)
            => Send<PlayerRegisterResponse>(HttpMethod.Post, Url(Endpoints.GamePlayersRegister), new PlayerRegisterRequest { Player_name = name, Player_data = playerData }, ct);

        public Task<PlayerAuthResponse> AuthenticatePlayer(string playerToken, CancellationToken ct = default)
            => Send<PlayerAuthResponse>(HttpMethod.Put, Url(Endpoints.GamePlayersLogin, $"&player_token={playerToken}"), null, ct);

        public Task<PlayerHeartbeatResponse> SendPlayerHeartbeatAsync(string playerToken, CancellationToken ct = default)
            => Send<PlayerHeartbeatResponse>(HttpMethod.Post, Url(Endpoints.GamePlayersHeartbeat, $"&player_token={playerToken}"), null, ct);

        public Task<PlayerLogoutResponse> LogoutPlayerAsync(string playerToken, CancellationToken ct = default)
            => Send<PlayerLogoutResponse>(HttpMethod.Post, Url(Endpoints.GamePlayersLogout, $"&player_token={playerToken}"), null, ct);

        public Task<PlayerListResponse> GetAllPlayers(CancellationToken ct = default)
            => Send<PlayerListResponse>(HttpMethod.Get, Url(Endpoints.GamePlayersList, $"&private_token={_apiPrivateToken}"), null, ct);

        // ==================== GAME DATA ====================
        public Task<GameDataResponse<T>> GetGameData<T>(CancellationToken ct = default) where T : class, new()
            => Send<GameDataResponse<T>>(HttpMethod.Get, Url(Endpoints.GameDataGameGet), null, ct);

        public Task<SuccessResponse> UpdateGameData(object data, CancellationToken ct = default)
            => Send<SuccessResponse>(HttpMethod.Put, Url(Endpoints.GameDataGameUpdate, $"&private_token={_apiPrivateToken}"), data, ct);

        public Task<PlayerDataResponse<T>> GetPlayerData<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<PlayerDataResponse<T>>(HttpMethod.Get, Url(Endpoints.GameDataPlayerGet, $"&player_token={playerToken}"), null, ct);

        public Task<SuccessResponse> UpdatePlayerData(string playerToken, object data, CancellationToken ct = default)
            => Send<SuccessResponse>(HttpMethod.Put, Url(Endpoints.GameDataPlayerUpdate, $"&player_token={playerToken}"), data, ct);

        // ==================== LEADERBOARD ====================
        public Task<LeaderboardResponse<T>> GetLeaderboardAsync<T>(string[] sortBy, int limit = 10, CancellationToken ct = default) where T : class, new()
            => Send<LeaderboardResponse<T>>(HttpMethod.Post, Url(Endpoints.Leaderboard), new LeaderboardRequest { Sort_by = sortBy, Limit = limit }, ct);

        // ==================== TIME ====================
        public Task<ServerTimeResponse> GetServerTime(CancellationToken ct = default)
            => Send<ServerTimeResponse>(HttpMethod.Get, Url(Endpoints.Time), null, ct);

        public Task<ServerTimeWithOffsetResponse> GetServerTimeWithOffset(int utcOffset, CancellationToken ct = default)
            => Send<ServerTimeWithOffsetResponse>(HttpMethod.Get, Url(Endpoints.Time, $"&utc={utcOffset:+#;-#}"), null, ct);

        // ==================== GAME ROOMS ====================
        public Task<RoomCreateResponse> CreateRoomAsync(string playerToken, string roomName, string? password = null, int maxPlayers = 4, object? rules = null, CancellationToken ct = default)
            => Send<RoomCreateResponse>(HttpMethod.Post, Url(Endpoints.GameRoomCreate, $"&player_token={playerToken}"),
                new RoomCreateRequest { Room_name = roomName, Password = password, Max_players = maxPlayers, Rules = rules }, ct);

        public Task<RoomListResponse> GetRoomsAsync(CancellationToken ct = default)
            => Send<RoomListResponse>(HttpMethod.Get, Url(Endpoints.GameRoomList), null, ct);

        public Task<RoomJoinResponse> JoinRoomAsync(string playerToken, string roomId, string? password = null, CancellationToken ct = default)
            => Send<RoomJoinResponse>(HttpMethod.Post, Url(string.Format(Endpoints.GameRoomJoin, roomId), $"&player_token={playerToken}"),
                password != null ? new RoomJoinRequest { Password = password } : null, ct);

        public Task<RoomLeaveResponse> LeaveRoomAsync(string playerToken, CancellationToken ct = default)
            => Send<RoomLeaveResponse>(HttpMethod.Post, Url(Endpoints.GameRoomLeave, $"&player_token={playerToken}"), null, ct);

        public Task<RoomPlayersResponse> GetRoomPlayersAsync(string playerToken, CancellationToken ct = default)
            => Send<RoomPlayersResponse>(HttpMethod.Get, Url(Endpoints.GameRoomPlayers, $"&player_token={playerToken}"), null, ct);

        public Task<HeartbeatResponse> SendRoomHeartbeatAsync(string playerToken, CancellationToken ct = default)
            => Send<HeartbeatResponse>(HttpMethod.Post, Url(Endpoints.GameRoomHeartbeat, $"&player_token={playerToken}"), null, ct);

        public Task<ActionSubmitResponse> SubmitActionAsync(string playerToken, string actionType, object? requestData = null, CancellationToken ct = default)
            => Send<ActionSubmitResponse>(HttpMethod.Post, Url(Endpoints.GameRoomActions, $"&player_token={playerToken}"),
                new ActionSubmitRequest { Action_type = actionType, Request_data = requestData }, ct);

        public Task<ActionPollResponse> PollActionsAsync(string playerToken, CancellationToken ct = default)
            => Send<ActionPollResponse>(HttpMethod.Get, Url(Endpoints.GameRoomActionsPoll, $"&player_token={playerToken}"), null, ct);

        public Task<ActionPendingResponse> GetPendingActionsAsync(string playerToken, CancellationToken ct = default)
            => Send<ActionPendingResponse>(HttpMethod.Get, Url(Endpoints.GameRoomActionsPending, $"&player_token={playerToken}"), null, ct);

        public Task<ActionCompleteResponse> CompleteActionAsync(string actionId, string playerToken, ActionComplete request, CancellationToken ct = default)
            => Send<ActionCompleteResponse>(HttpMethod.Post, Url(string.Format(Endpoints.GameRoomActionComplete, actionId), $"&player_token={playerToken}"),
                new ActionCompleteRequest { Status = request.Status.ToString().ToLower(), Response_data = request.Response_data }, ct);

        public Task<UpdatePlayersResponse> UpdatePlayersAsync(string playerToken, UpdatePlayersRequest request, CancellationToken ct = default)
            => Send<UpdatePlayersResponse>(HttpMethod.Post, Url(Endpoints.GameRoomUpdates, $"&player_token={playerToken}"), request, ct);

        public Task<PollUpdatesResponse> PollUpdatesAsync(string playerToken, string? lastUpdateId = null, CancellationToken ct = default)
        {
            string extra = $"&player_token={playerToken}";
            if (!string.IsNullOrEmpty(lastUpdateId)) extra += $"&lastUpdateId={lastUpdateId}";
            return Send<PollUpdatesResponse>(HttpMethod.Get, Url(Endpoints.GameRoomUpdatesPoll, extra), null, ct);
        }

        public Task<CurrentRoomResponse> GetCurrentRoomAsync(string playerToken, CancellationToken ct = default)
            => Send<CurrentRoomResponse>(HttpMethod.Get, Url(Endpoints.GameRoomCurrent, $"&player_token={playerToken}"), null, ct);

        // ==================== MATCHMAKING ====================
        public Task<MatchmakingListResponse> GetMatchmakingLobbiesAsync(CancellationToken ct = default)
            => Send<MatchmakingListResponse>(HttpMethod.Get, Url(Endpoints.MatchmakingList), null, ct);

        public Task<MatchmakingCreateResponse> CreateMatchmakingLobbyAsync(string playerToken, int maxPlayers = 4, bool strictFull = false,
            bool joinByRequests = false, object? rules = null, CancellationToken ct = default)
            => Send<MatchmakingCreateResponse>(HttpMethod.Post, Url(Endpoints.MatchmakingCreate, $"&player_token={playerToken}"),
                new MatchmakingCreateRequest { Max_players = maxPlayers, Strict_full = strictFull, Join_by_requests = joinByRequests, Rules = rules }, ct);

        public Task<MatchmakingJoinRequestResponse> RequestToJoinMatchmakingAsync(string playerToken, string matchmakingId, CancellationToken ct = default)
            => Send<MatchmakingJoinRequestResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingRequest, matchmakingId), $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingPermissionResponse> RespondToJoinRequestAsync(string playerToken, string requestId, MatchmakingRequestAction action, CancellationToken ct = default)
            => Send<MatchmakingPermissionResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingResponse, requestId), $"&player_token={playerToken}"),
                new MatchmakingPermissionRequest { Action = action.ToString().ToLower() }, ct);

        public Task<MatchmakingRequestStatusResponse> CheckJoinRequestStatusAsync(string playerToken, string requestId, CancellationToken ct = default)
            => Send<MatchmakingRequestStatusResponse>(HttpMethod.Get, Url(string.Format(Endpoints.MatchmakingRequestStatus, requestId), $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingCurrentResponse> GetCurrentMatchmakingStatusAsync(string playerToken, CancellationToken ct = default)
            => Send<MatchmakingCurrentResponse>(HttpMethod.Get, Url(Endpoints.MatchmakingCurrent, $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingDirectJoinResponse> JoinMatchmakingDirectlyAsync(string playerToken, string matchmakingId, CancellationToken ct = default)
            => Send<MatchmakingDirectJoinResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingJoin, matchmakingId), $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingLeaveResponse> LeaveMatchmakingAsync(string playerToken, CancellationToken ct = default)
            => Send<MatchmakingLeaveResponse>(HttpMethod.Post, Url(Endpoints.MatchmakingLeave, $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingPlayersResponse> GetMatchmakingPlayersAsync(string playerToken, CancellationToken ct = default)
            => Send<MatchmakingPlayersResponse>(HttpMethod.Get, Url(Endpoints.MatchmakingPlayers, $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingHeartbeatResponse> SendMatchmakingHeartbeatAsync(string playerToken, CancellationToken ct = default)
            => Send<MatchmakingHeartbeatResponse>(HttpMethod.Post, Url(Endpoints.MatchmakingHeartbeat, $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingRemoveResponse> RemoveMatchmakingLobbyAsync(string playerToken, CancellationToken ct = default)
            => Send<MatchmakingRemoveResponse>(HttpMethod.Post, Url(Endpoints.MatchmakingRemove, $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingStartResponse> StartGameFromMatchmakingAsync(string playerToken, CancellationToken ct = default)
            => Send<MatchmakingStartResponse>(HttpMethod.Post, Url(Endpoints.MatchmakingStart, $"&player_token={playerToken}"), null, ct);
    }

    public enum RoomActionStatus { Pending, Processing, Completed, Failed, Read }

    public enum RoomCompleteActionStatus { Processing, Completed, Failed }

    public enum MatchmakingRequestAction { Approve, Reject }

    // ====================== ALL REQUEST CLASSES =======================


    public class PlayerRegisterRequest
    {
        public required string Player_name { get; set; }
        public object? Player_data { get; set; }
    }

    // ====================== ALL RESPONSE CLASSES ======================

    public class PlayerRegisterResponse : ApiResponse
    {
        public string Player_id { get; set; } = string.Empty;
        public string Private_key { get; set; } = string.Empty;
        public string Player_name { get; set; } = string.Empty;
        public int Game_id { get; set; }
    }

    public class PlayerAuthResponse : ApiResponse
    {
        public PlayerInfo? Player { get; set; }
    }

    public class PlayerListResponse : ApiResponse
    {
        public int Count { get; set; }
        public List<PlayerShort> Players { get; set; } = new();
    }

    public class PlayerShort
    {
        public int Id { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public int Is_active { get; set; }
        public string Last_login { get; set; } = string.Empty;
        public string Created_at { get; set; } = string.Empty;
    }

    public class PlayerInfo
    {
        public int Id { get; set; }
        public int Game_id { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public Dictionary<string, object> Player_data { get; set; } = new();
        public bool Is_active { get; set; }
        public string Last_login { get; set; } = string.Empty;
        public string Created_at { get; set; } = string.Empty;
        public string Updated_at { get; set; } = string.Empty;
    }

    public class PlayerHeartbeatResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Last_heartbeat { get; set; } = string.Empty;
    }

    public class PlayerLogoutResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Last_logout { get; set; } = string.Empty;
    }

    public class GameDataResponse<T> : ApiResponse where T : class, new()
    {
        public string Type { get; set; } = string.Empty;
        public int Game_id { get; set; }
        public JsonElement Data { get; set; }



        [JsonIgnore]
        public T GetData
        {
            get
            {
                return Data.Deserialize<T>(GameSDK.JsonOptions)!;
            }
        }
    }

    public class PlayerDataResponse<T> : ApiResponse where T : class, new()
    {
        public string Type { get; set; } = string.Empty;
        public int Player_id { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public JsonElement Data { get; set; }



        [JsonIgnore]
        public T GetData
        {
            get
            {
                return Data.Deserialize<T>(GameSDK.JsonOptions)!;
            }
        }
    }

    public class SuccessResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Updated_at { get; set; } = string.Empty;
    }

    public class ServerTimeResponse : ApiResponse
    {
        public string Utc { get; set; } = string.Empty;
        public long Timestamp { get; set; }
        public string Readable { get; set; } = string.Empty;
    }

    public class ServerTimeWithOffsetResponse : ApiResponse
    {
        public string Utc { get; set; } = string.Empty;
        public long Timestamp { get; set; }
        public string Readable { get; set; } = string.Empty;
        public TimeOffset? Offset { get; set; }
    }

    public class TimeOffset
    {
        public int Offset_hours { get; set; }
        public string Offset_string { get; set; } = string.Empty;
        public string Original_utc { get; set; } = string.Empty;
        public long Original_timestamp { get; set; }
    }

    public class RoomCreateRequest
    {
        public string Room_name { get; set; } = string.Empty;
        public string? Password { get; set; }
        public int Max_players { get; set; }
        public object? Rules { get; set; }
    }

    public class RoomCreateResponse : ApiResponse
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public bool Is_host { get; set; }
    }

    public class RoomShort
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public int Max_players { get; set; }
        public int Current_players { get; set; }
        public int Has_password { get; set; }
        public string? Rules { get; set; }
    }

    public class RoomListResponse : ApiResponse
    {
        public List<RoomShort> Rooms { get; set; } = new();
    }

    public class RoomJoinRequest
    {
        public string? Password { get; set; }
    }

    public class RoomJoinResponse : ApiResponse
    {
        public string Room_id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class RoomPlayer
    {
        public string Player_id { get; set; } = string.Empty;
        public string Player_name { get; set; } = string.Empty;
        public int Is_host { get; set; }
        public int Is_online { get; set; }
        public string Last_heartbeat { get; set; } = string.Empty;
    }

    public class RoomPlayersResponse : ApiResponse
    {
        public List<RoomPlayer> Players { get; set; } = new();
        public string Last_updated { get; set; } = string.Empty;
    }

    public class RoomLeaveResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class HeartbeatResponse : ApiResponse
    {
        public string Status { get; set; } = string.Empty;
    }

    public class ActionSubmitRequest
    {
        public string Action_type { get; set; } = string.Empty;
        public object? Request_data { get; set; }
    }

    public class ActionSubmitResponse : ApiResponse
    {
        public string Action_id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class ActionInfo
    {
        public string Action_id { get; set; } = string.Empty;
        public string Action_type { get; set; } = string.Empty;
        public object? Response_data { get; set; }
        public string Status { get; set; } = string.Empty;



        public RoomActionStatus GetStatus
        {
            get
            {
                switch (Status)
                {
                    case "pending":
                        return RoomActionStatus.Pending;
                    case "processing":
                        return RoomActionStatus.Processing;
                    case "completed":
                        return RoomActionStatus.Completed;
                    case "failed":
                        return RoomActionStatus.Failed;
                    case "read":
                        return RoomActionStatus.Read;
                    default:
                        throw new ArgumentException($"Unknown action status: {Status}");
                }
            }
        }
    }

    public class ActionPollResponse : ApiResponse
    {
        public List<ActionInfo> Actions { get; set; } = new();
    }

    public class PendingAction
    {
        public string Action_id { get; set; } = string.Empty;
        public string Player_id { get; set; } = string.Empty;
        public string Action_type { get; set; } = string.Empty;
        public object? Request_data { get; set; }
        public string Created_at { get; set; } = string.Empty;
        public string Player_name { get; set; } = string.Empty;
    }

    public class ActionPendingResponse : ApiResponse
    {
        public List<PendingAction> Actions { get; set; } = new();
    }

    public class ActionComplete
    {
        public RoomCompleteActionStatus Status { get; private set; }

        public object? Response_data { get; private set; }



        public ActionComplete(RoomCompleteActionStatus status, object? responseData)
        {
            Status = status;
            Response_data = responseData;
        }
    }

    public class ActionCompleteRequest
    {
        public string Status { get; set; } = RoomCompleteActionStatus.Completed.ToString().ToLower();
        public object? Response_data { get; set; }
    }

    public class ActionCompleteResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class UpdatePlayersRequest
    {
        public object? Target_player_ids { get; set; }   // "all" or string[]
        public string Type { get; set; } = string.Empty;
        public object Data { get; set; } = new();

        public UpdatePlayersRequest(object targetPlayerIds, string type, object data)
        {
            Target_player_ids = targetPlayerIds;
            Type = type;
            Data = data;
        }
    }

    public class UpdatePlayersResponse : ApiResponse
    {
        public int Updates_sent { get; set; }
        public List<string> Update_ids { get; set; } = new();
        public List<string> Target_players { get; set; } = new();
    }

    public class PlayerUpdate
    {
        public string Update_id { get; set; } = string.Empty;
        public string From_player_id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public object Data { get; set; } = new();
        public string Created_at { get; set; } = string.Empty;
    }

    public class PollUpdatesResponse : ApiResponse
    {
        public List<PlayerUpdate> Updates { get; set; } = new();
        public string Last_update_id { get; set; } = string.Empty;
    }

    public class CurrentRoomInfo
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public bool Is_host { get; set; }
        public bool Is_online { get; set; }
        public int Max_players { get; set; }
        public int Current_players { get; set; }
        public bool Has_password { get; set; }
        public bool Is_active { get; set; }
        public string? Rules { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public string Joined_at { get; set; } = string.Empty;
        public string Last_heartbeat { get; set; } = string.Empty;
        public string Room_created_at { get; set; } = string.Empty;
        public string Room_last_activity { get; set; } = string.Empty;
    }

    public class CurrentRoomResponse : ApiResponse
    {
        public bool In_room { get; set; }
        public CurrentRoomInfo? Room { get; set; }
        public List<object>? Pending_actions { get; set; }
        public List<object>? Pending_updates { get; set; }
    }

    public class MatchmakingListResponse : ApiResponse
    {
        public List<MatchmakingLobby> Lobbies { get; set; } = new();
    }

    public class MatchmakingLobby
    {
        public string Matchmaking_id { get; set; } = string.Empty;
        public int Host_player_id { get; set; }
        public int Max_players { get; set; }
        public int Strict_full { get; set; }
        public object? Rules { get; set; }
        public string Created_at { get; set; } = string.Empty;
        public string Last_heartbeat { get; set; } = string.Empty;
        public int Current_players { get; set; }
        public string Host_name { get; set; } = string.Empty;
    }

    public class MatchmakingCreateRequest
    {
        public int Max_players { get; set; }
        public bool Strict_full { get; set; }
        public bool Join_by_requests { get; set; }
        public object? Rules { get; set; }
    }

    public class MatchmakingCreateResponse : ApiResponse
    {
        public string Matchmaking_id { get; set; } = string.Empty;
        public int Max_players { get; set; }
        public bool Strict_full { get; set; }
        public bool Join_by_requests { get; set; }
        public bool Is_host { get; set; }
    }

    public class MatchmakingJoinRequestResponse : ApiResponse
    {
        public string Request_id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class MatchmakingPermissionRequest
    {
        public string Action { get; set; } = MatchmakingRequestAction.Approve.ToString().ToLower();
    }

    public class MatchmakingPermissionResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Request_id { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }

    public class MatchmakingRequestStatusResponse : ApiResponse
    {
        public MatchmakingRequestInfo Request { get; set; } = new();
    }

    public class MatchmakingRequestInfo : MatchmakingRequestBase
    {

        public int? Responded_by { get; set; }
        public string? Responder_name { get; set; }
        public bool Join_by_requests { get; set; }
    }

    public class MatchmakingCurrentResponse : ApiResponse
    {
        public bool In_matchmaking { get; set; }
        public MatchmakingInfo? Matchmaking { get; set; }
        public List<MatchmakingRequestBase> Pending_requests { get; set; } = new();
    }

    public class MatchmakingRequestBase
    {
        public string Request_id { get; set; } = string.Empty;
        public string Matchmaking_id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Requested_at { get; set; } = string.Empty;
        public string? Responded_at { get; set; }
    }

    public class MatchmakingInfo
    {
        public string Matchmaking_id { get; set; } = string.Empty;
        public bool Is_host { get; set; }
        public int Max_players { get; set; }
        public int Current_players { get; set; }
        public bool Strict_full { get; set; }
        public bool Join_by_requests { get; set; }
        public object? Rules { get; set; }
        public string Joined_at { get; set; } = string.Empty;
        public string Player_status { get; set; } = string.Empty;
        public string Last_heartbeat { get; set; } = string.Empty;
        public string Lobby_heartbeat { get; set; } = string.Empty;
        public bool Is_started { get; set; }
        public object? Started_at { get; set; }
    }

    public class MatchmakingDirectJoinResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Matchmaking_id { get; set; } = string.Empty;
    }

    public class MatchmakingLeaveResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class MatchmakingPlayersResponse : ApiResponse
    {
        public List<MatchmakingPlayer> Players { get; set; } = new();
    }

    public class MatchmakingPlayer
    {
        public int Player_id { get; set; }
        public string Joined_at { get; set; } = string.Empty;
        public string Last_heartbeat { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Player_name { get; set; } = string.Empty;
        public int Seconds_since_heartbeat { get; set; }
        public int Is_host { get; set; }
    }

    public class MatchmakingHeartbeatResponse : ApiResponse
    {
        public string? Status { get; set; }
    }

    public class MatchmakingRemoveResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class MatchmakingStartResponse : ApiResponse
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public int Players_transferred { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class LeaderboardRequest
    {
        public string[] Sort_by { get; set; } = Array.Empty<string>();
        public int Limit { get; set; }
    }

    public class LeaderboardResponse<T> : ApiResponse where T : class, new()
    {
        public List<LeaderboardPlayer<T>> Leaderboard { get; set; } = new();
        public int Total { get; set; }
        public string[] Sort_by { get; set; } = Array.Empty<string>();
        public int Limit { get; set; }
    }

    public class LeaderboardPlayer<T> where T : class, new()
    {
        public int Rank { get; set; }
        public int Player_id { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public JsonElement Player_data { get; set; } = new();



        [JsonIgnore]
        public T GetData
        {
            get
            {
                return Player_data.Deserialize<T>(GameSDK.JsonOptions)!;
            }
        }
    }

    // ====================== EXCEPTION ======================
    public class ApiException : Exception
    {
        public string? RawResponse { get; }
        public string? ApiError { get; }

        public ApiException(string message, string? rawResponse = null) : base(message)
        {
            RawResponse = rawResponse;
            ApiError = message;
        }

        public ApiException(string message, string? rawResponse, Exception inner) : base(message, inner)
        {
            RawResponse = rawResponse;
            ApiError = message;
        }
    }

    public class ErrorResponse : ApiResponse { }
}