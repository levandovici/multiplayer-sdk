using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public Task<PlayerRegisterResponse> RegisterPlayer<T>(string name, T? playerData = null, CancellationToken ct = default) where T : class, new()
            => Send<PlayerRegisterResponse>(HttpMethod.Post, Url(Endpoints.GamePlayersRegister), new PlayerRegisterRequest<T>(name, playerData), ct);

        public Task<PlayerAuthResponse<T>> AuthenticatePlayer<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<PlayerAuthResponse<T>>(HttpMethod.Put, Url(Endpoints.GamePlayersLogin, $"&player_token={playerToken}"), null, ct);

        public Task<PlayerHeartbeatResponse> SendPlayerHeartbeatAsync(string playerToken, CancellationToken ct = default)
            => Send<PlayerHeartbeatResponse>(HttpMethod.Post, Url(Endpoints.GamePlayersHeartbeat, $"&player_token={playerToken}"), null, ct);

        public Task<PlayerLogoutResponse> LogoutPlayerAsync(string playerToken, CancellationToken ct = default)
            => Send<PlayerLogoutResponse>(HttpMethod.Post, Url(Endpoints.GamePlayersLogout, $"&player_token={playerToken}"), null, ct);

        public Task<PlayerListResponse> GetAllPlayers(CancellationToken ct = default)
            => Send<PlayerListResponse>(HttpMethod.Get, Url(Endpoints.GamePlayersList, $"&private_token={_apiPrivateToken}"), null, ct);

        // ==================== GAME DATA ====================
        public Task<GameDataResponse<T>> GetGameData<T>(CancellationToken ct = default) where T : class, new()
            => Send<GameDataResponse<T>>(HttpMethod.Get, Url(Endpoints.GameDataGameGet), null, ct);

        public Task<SuccessResponse> UpdateGameData<T>(T data, CancellationToken ct = default) where T : class, new()
            => Send<SuccessResponse>(HttpMethod.Put, Url(Endpoints.GameDataGameUpdate, $"&private_token={_apiPrivateToken}"), data, ct);

        public Task<PlayerDataResponse<T>> GetPlayerData<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<PlayerDataResponse<T>>(HttpMethod.Get, Url(Endpoints.GameDataPlayerGet, $"&player_token={playerToken}"), null, ct);

        public Task<SuccessResponse> UpdatePlayerData<T>(string playerToken, T data, CancellationToken ct = default) where T : class, new()
            => Send<SuccessResponse>(HttpMethod.Put, Url(Endpoints.GameDataPlayerUpdate, $"&player_token={playerToken}"), data, ct);

        // ==================== LEADERBOARD ====================
        public Task<LeaderboardResponse<T>> GetLeaderboardAsync<T>(string[] sortBy, int limit = 10, CancellationToken ct = default) where T : class, new()
            => Send<LeaderboardResponse<T>>(HttpMethod.Post, Url(Endpoints.Leaderboard), new LeaderboardRequest(sortBy, limit), ct);

        // ==================== TIME ====================
        public Task<ServerTimeResponse> GetServerTime(CancellationToken ct = default)
            => Send<ServerTimeResponse>(HttpMethod.Get, Url(Endpoints.Time), null, ct);

        public Task<ServerTimeWithOffsetResponse> GetServerTimeWithOffset(int utcOffset, CancellationToken ct = default)
            => Send<ServerTimeWithOffsetResponse>(HttpMethod.Get, Url(Endpoints.Time, $"&utc={utcOffset:+#;-#}"), null, ct);

        // ==================== GAME ROOMS ====================
        public Task<RoomCreateResponse> CreateRoomAsync<T>(string playerToken, string roomName, int maxPlayers = 4,
            string? password = null, T? rules = null, CancellationToken ct = default) where T : class, new()
            => Send<RoomCreateResponse>(HttpMethod.Post, Url(Endpoints.GameRoomCreate, $"&player_token={playerToken}"),
                new RoomCreateRequest<T>(roomName, maxPlayers, password, rules), ct);

        public Task<RoomListResponse<T>> GetRoomsAsync<T>(CancellationToken ct = default) where T : class, new()
            => Send<RoomListResponse<T>>(HttpMethod.Get, Url(Endpoints.GameRoomList), null, ct);

        public Task<RoomJoinResponse> JoinRoomAsync(string playerToken, string roomId, string? password = null, CancellationToken ct = default)
            => Send<RoomJoinResponse>(HttpMethod.Post, Url(string.Format(Endpoints.GameRoomJoin, roomId), $"&player_token={playerToken}"),
                password != null ? new RoomJoinRequest(password) : null, ct);

        public Task<RoomLeaveResponse> LeaveRoomAsync(string playerToken, CancellationToken ct = default)
            => Send<RoomLeaveResponse>(HttpMethod.Post, Url(Endpoints.GameRoomLeave, $"&player_token={playerToken}"), null, ct);

        public Task<RoomPlayersResponse> GetRoomPlayersAsync(string playerToken, CancellationToken ct = default)
            => Send<RoomPlayersResponse>(HttpMethod.Get, Url(Endpoints.GameRoomPlayers, $"&player_token={playerToken}"), null, ct);

        public Task<HeartbeatResponse> SendRoomHeartbeatAsync(string playerToken, CancellationToken ct = default)
            => Send<HeartbeatResponse>(HttpMethod.Post, Url(Endpoints.GameRoomHeartbeat, $"&player_token={playerToken}"), null, ct);

        public Task<ActionSubmitResponse> SubmitActionAsync<T>(string playerToken, string actionType,
            T? requestData = null, CancellationToken ct = default) where T : class, new()
            => Send<ActionSubmitResponse>(HttpMethod.Post, Url(Endpoints.GameRoomActions,
                $"&player_token={playerToken}"), new ActionSubmitRequest<T>(actionType, requestData), ct);

        public Task<ActionPollResponse<T>> PollActionsAsync<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<ActionPollResponse<T>>(HttpMethod.Get, Url(Endpoints.GameRoomActionsPoll, $"&player_token={playerToken}"), null, ct);

        public Task<ActionPendingResponse<T>> GetPendingActionsAsync<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<ActionPendingResponse<T>>(HttpMethod.Get, Url(Endpoints.GameRoomActionsPending, $"&player_token={playerToken}"), null, ct);

        public Task<ActionCompleteResponse> CompleteActionAsync<T>(string actionId, string playerToken,
            ActionComplete<T> request, CancellationToken ct = default) where T : class, new()
            => Send<ActionCompleteResponse>(HttpMethod.Post, Url(string.Format(Endpoints.GameRoomActionComplete, actionId),
                $"&player_token={playerToken}"), new ActionCompleteRequest<T>(request.Status, request.Response_data), ct);

        public Task<UpdatePlayersResponse> UpdatePlayersAsync<T>(string playerToken, UpdatePlayers<T> request, CancellationToken ct = default) where T : class, new()
            => Send<UpdatePlayersResponse>(HttpMethod.Post, Url(Endpoints.GameRoomUpdates, $"&player_token={playerToken}"),
                new UpdatePlayersRequest<T>(request.Target_players, request.Type, request.Data, request.Target_players_ids), ct);

        public Task<PollUpdatesResponse<T>> PollUpdatesAsync<T>(string playerToken,
            string? lastUpdateId = null, CancellationToken ct = default) where T : class, new()
        {
            string extra = $"&player_token={playerToken}";
            if (!string.IsNullOrEmpty(lastUpdateId)) extra += $"&lastUpdateId={lastUpdateId}";
            return Send<PollUpdatesResponse<T>>(HttpMethod.Get, Url(Endpoints.GameRoomUpdatesPoll, extra), null, ct);
        }

        public Task<CurrentRoomResponse<T>> GetCurrentRoomAsync<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<CurrentRoomResponse<T>>(HttpMethod.Get, Url(Endpoints.GameRoomCurrent, $"&player_token={playerToken}"), null, ct);

        // ==================== MATCHMAKING ====================
        public Task<MatchmakingListResponse<T>> GetMatchmakingLobbiesAsync<T>(CancellationToken ct = default) where T : class, new()
            => Send<MatchmakingListResponse<T>>(HttpMethod.Get, Url(Endpoints.MatchmakingList), null, ct);

        public Task<MatchmakingCreateResponse> CreateMatchmakingLobbyAsync<T>(string playerToken, int maxPlayers = 4, bool strictFull = false,
            bool joinByRequests = false, T? rules = null, CancellationToken ct = default) where T : class, new()
            => Send<MatchmakingCreateResponse>(HttpMethod.Post, Url(Endpoints.MatchmakingCreate, $"&player_token={playerToken}"),
                new MatchmakingCreateRequest<T>(maxPlayers, strictFull, joinByRequests, rules), ct);

        public Task<MatchmakingJoinRequestResponse> RequestToJoinMatchmakingAsync(string playerToken, string matchmakingId, CancellationToken ct = default)
            => Send<MatchmakingJoinRequestResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingRequest, matchmakingId), $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingPermissionResponse> RespondToJoinRequestAsync(string playerToken, string requestId, MatchmakingRequestAction action, CancellationToken ct = default)
            => Send<MatchmakingPermissionResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingResponse, requestId), $"&player_token={playerToken}"),
                new MatchmakingPermissionRequest(action), ct);

        public Task<MatchmakingRequestStatusResponse> CheckJoinRequestStatusAsync(string playerToken, string requestId, CancellationToken ct = default)
            => Send<MatchmakingRequestStatusResponse>(HttpMethod.Get, Url(string.Format(Endpoints.MatchmakingRequestStatus, requestId), $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingCurrentResponse<T>> GetCurrentMatchmakingStatusAsync<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<MatchmakingCurrentResponse<T>>(HttpMethod.Get, Url(Endpoints.MatchmakingCurrent, $"&player_token={playerToken}"), null, ct);

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

    public enum RoomTargetPlayers { All, Others, Specific }

    public enum MatchmakingRequestAction { Approve, Reject }

    // ====================== ALL PARAMETERS CLASSES ====================

    public class ActionComplete<T> where T : class, new()
    {
        public RoomCompleteActionStatus Status { get; private set; }

        public T? Response_data { get; private set; }



        public ActionComplete(RoomCompleteActionStatus status, T? responseData)
        {
            Status = status;
            Response_data = responseData;
        }
    }

    public class UpdatePlayers<T> where T : class, new()
    {
        [JsonInclude]
        public RoomTargetPlayers Target_players { get; private set; } = RoomTargetPlayers.All;
        [JsonInclude]
        public int[]? Target_players_ids { get; private set; }
        [JsonInclude]
        public string Type { get; private set; } = string.Empty;
        [JsonInclude]
        public T? Data { get; private set; } = new();



        public UpdatePlayers(RoomTargetPlayers targetPlayers, string type, T? data = null, int[]? targetPlayersIds = null)
        {
            Target_players = targetPlayers;
            Target_players_ids = targetPlayersIds;
            Type = type;
            Data = data;
        }
    }

    // ====================== ALL REQUEST CLASSES =======================

    internal class PlayerRegisterRequest<T> where T : class, new()
    {
        [JsonInclude]
        internal required string Player_name { get; set; }
        [JsonInclude]
        private T? Player_data { get; set; }



        [SetsRequiredMembers]
        public PlayerRegisterRequest(string playerName, T? playerData = null)
        {
            this.Player_name = playerName;
            this.Player_data = playerData;
        }
    }

    public class RoomCreateRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string Room_name { get; set; } = string.Empty;
        [JsonInclude]
        private string? Password { get; set; }
        [JsonInclude]
        private int Max_players { get; set; }
        [JsonInclude]
        private T? Rules { get; set; }



        public RoomCreateRequest(string room_name, int max_players, string? password = null, T? rules = null)
        {
            Room_name = room_name;
            Password = password;
            Max_players = max_players;
            Rules = rules;
        }
    }

    public class RoomJoinRequest
    {
        [JsonInclude]
        private string? Password { get; set; }



        public RoomJoinRequest(string? password = null)
        {
            this.Password = password;
        }
    }

    public class ActionSubmitRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string Action_type { get; set; } = string.Empty;
        [JsonInclude]
        private T? Request_data { get; set; }



        public ActionSubmitRequest(string actionType, T? requestData = null)
        {
            this.Action_type = actionType;
            this.Request_data = requestData;
        }
    }

    public class ActionCompleteRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string Status { get; set; } = RoomCompleteActionStatus.Completed.ToString().ToLower();
        [JsonInclude]
        private T? Response_data { get; set; }



        public ActionCompleteRequest(RoomCompleteActionStatus status, T? responseData)
        {
            this.Status = status.ToString().ToLower();
            this.Response_data = responseData;
        }
    }

    public class UpdatePlayersRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string Target_players { get; set; } = RoomTargetPlayers.All.ToString().ToLower();
        [JsonInclude]
        private int[]? Target_players_ids { get; set; }
        [JsonInclude]
        private string Type { get; set; } = string.Empty;
        [JsonInclude]
        private T? Data { get; set; } = new();



        public UpdatePlayersRequest(RoomTargetPlayers targetPlayers, string type, T? data = null, int[]? targetPlayersIds = null)
        {
            Target_players = targetPlayers.ToString().ToLower();
            Target_players_ids = targetPlayersIds;
            Type = type;
            Data = data;
        }
    }

    public class MatchmakingCreateRequest<T> where T : class, new()
    {
        [JsonInclude]
        private int Max_players { get; set; }
        [JsonInclude]
        private bool Strict_full { get; set; }
        [JsonInclude]
        private bool Join_by_requests { get; set; }
        [JsonInclude]
        private T? Rules { get; set; }



        public MatchmakingCreateRequest(int maxPlayers, bool strictFull, bool joinByRequests, T? rules)
        {
            this.Max_players = maxPlayers;
            this.Strict_full = strictFull;
            this.Join_by_requests = joinByRequests;
            this.Rules = rules;
        }
    }

    public class MatchmakingPermissionRequest
    {
        [JsonInclude]
        private string Action { get; set; } = MatchmakingRequestAction.Approve.ToString().ToLower();



        public MatchmakingPermissionRequest(MatchmakingRequestAction action)
        {
            this.Action = action.ToString().ToLower();
        }
    }

    public class LeaderboardRequest
    {
        [JsonInclude]
        private string[] Sort_by { get; set; } = Array.Empty<string>();
        [JsonInclude]
        private int Limit { get; set; }


        public LeaderboardRequest(string[] sortBy, int limit)
        {
            this.Sort_by = sortBy;
            this.Limit = limit;
        }
    }

    // ====================== ALL RESPONSE CLASSES ======================

    public class PlayerRegisterResponse : ApiResponse
    {
        public int Player_id { get; set; }
        public string Private_key { get; set; } = string.Empty;
        public string Player_name { get; set; } = string.Empty;
        public int Game_id { get; set; }
    }

    public class PlayerAuthResponse<T> : ApiResponse where T : class, new()
    {
        public PlayerInfo<T>? Player { get; set; }
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
        public bool Is_active { get; set; }
        public DateTimeOffset? Last_login { get; set; }
        public DateTimeOffset Created_at { get; set; }
    }

    public class PlayerInfo<T> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Player_data { get; set; }



        public int Id { get; set; }
        public int Game_id { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public bool Is_active { get; set; }
        public DateTimeOffset? Last_login { get; set; }
        public DateTimeOffset Created_at { get; set; }
        public DateTimeOffset Updated_at { get; set; }



        [JsonIgnore]
        public T PlayerData
        {
            get
            {
                return Player_data.Deserialize<T>(GameSDK.JsonOptions)!;
            }
        }
    }

    public class PlayerHeartbeatResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset Last_heartbeat { get; set; }
    }

    public class PlayerLogoutResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset? Last_logout { get; set; }
    }

    public class GameDataResponse<T> : ApiResponse where T : class, new()
    {
        [JsonInclude]
        private JsonElement Data { get; set; }



        public string Type { get; set; } = string.Empty;
        public int Game_id { get; set; }



        [JsonIgnore]
        public T GameData
        {
            get
            {
                return Data.Deserialize<T>(GameSDK.JsonOptions)!;
            }
        }
    }

    public class PlayerDataResponse<T> : ApiResponse where T : class, new()
    {
        [JsonInclude]
        private JsonElement Data { get; set; }



        public string Type { get; set; } = string.Empty;
        public int Player_id { get; set; }
        public string Player_name { get; set; } = string.Empty;



        [JsonIgnore]
        public T PlayerData
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
        public DateTimeOffset Updated_at { get; set; }
    }

    public class ServerTimeResponse : ApiResponse
    {
        public DateTimeOffset Utc { get; set; }
        public long Timestamp { get; set; }
        public string Readable { get; set; } = string.Empty;
    }

    public class ServerTimeWithOffsetResponse : ServerTimeResponse
    {
        public TimeOffset? Offset { get; set; }
    }

    public class TimeOffset
    {
        public int Offset_hours { get; set; }
        public string Offset_string { get; set; } = string.Empty;
        public DateTimeOffset Original_utc { get; set; }
        public long Original_timestamp { get; set; }
    }

    public class RoomCreateResponse : ApiResponse
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public bool Is_host { get; set; }
    }

    public class RoomShort<T> where T : class, new()
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public int Max_players { get; set; }
        public int Current_players { get; set; }
        public int Has_password { get; set; }
        public T? Rules { get; set; }
    }

    public class RoomListResponse<T> : ApiResponse where T : class, new()
    {
        public List<RoomShort<T>> Rooms { get; set; } = new();
    }

    public class RoomJoinResponse : ApiResponse
    {
        public string Room_id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class RoomPlayer
    {
        public int Player_id { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public bool Is_host { get; set; }
        public bool Is_online { get; set; }
        public DateTimeOffset Last_heartbeat { get; set; }
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

    public class ActionSubmitResponse : ApiResponse
    {
        public string Action_id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class ActionInfo<T> where T : class, new()
    {
        [JsonInclude]
        private string Status { get; set; } = string.Empty;



        public string Action_id { get; set; } = string.Empty;
        public string Action_type { get; set; } = string.Empty;
        public T? Response_data { get; set; }



        public RoomActionStatus ActionStatus
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

    public class ActionPollResponse<T> : ApiResponse where T : class, new()
    {
        public List<ActionInfo<T>> Actions { get; set; } = new();
    }

    public class PendingAction<T> where T : class, new()
    {
        public string Action_id { get; set; } = string.Empty;
        public int Player_id { get; set; }
        public string Action_type { get; set; } = string.Empty;
        public DateTimeOffset Created_at { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public T? Request_data { get; set; }
    }

    public class ActionPendingResponse<T> : ApiResponse where T : class, new()
    {
        public List<PendingAction<T>> Actions { get; set; } = new();
    }

    public class ActionCompleteResponse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class UpdatePlayersResponse : ApiResponse
    {
        public int Updates_sent { get; set; }
        public List<string> Update_ids { get; set; } = new();
        public List<int> Target_players_ids { get; set; } = new();
    }

    public class PlayerUpdate<T> where T : class, new()
    {
        public string Update_id { get; set; } = string.Empty;
        public int From_player_id { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTimeOffset Created_at { get; set; }
        public T? Data { get; set; }
    }

    public class PollUpdatesResponse<T> : ApiResponse where T : class, new()
    {
        public List<PlayerUpdate<T>> Updates { get; set; } = new();
        public string Last_update_id { get; set; } = string.Empty;
    }

    public class CurrentRoomInfo<T> where T : class, new()
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public bool Is_host { get; set; }
        public bool Is_online { get; set; }
        public int Max_players { get; set; }
        public int Current_players { get; set; }
        public bool Has_password { get; set; }
        public bool Is_active { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public DateTimeOffset Joined_at { get; set; }
        public DateTimeOffset Last_heartbeat { get; set; }
        public DateTimeOffset Room_created_at { get; set; }
        public DateTimeOffset Room_last_activity { get; set; }
        public T? Rules { get; set; }
    }

    public class CurrentRoomResponse<T> : ApiResponse where T : class, new()
    {
        public bool In_room { get; set; }
        public CurrentRoomInfo<T>? Room { get; set; }
        public List<object>? Pending_actions { get; set; }
        public List<object>? Pending_updates { get; set; }
    }

    public class MatchmakingListResponse<T> : ApiResponse where T : class, new()
    {
        public List<MatchmakingLobby<T>> Lobbies { get; set; } = new();
    }

    public class MatchmakingLobby<T> where T : class, new()
    {
        public string Matchmaking_id { get; set; } = string.Empty;
        public int Host_player_id { get; set; }
        public int Max_players { get; set; }
        public int Strict_full { get; set; }
        public DateTimeOffset Created_at { get; set; }
        public DateTimeOffset Last_heartbeat { get; set; }
        public int Current_players { get; set; }
        public string Host_name { get; set; } = string.Empty;
        public T? Rules { get; set; }
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

    public class MatchmakingCurrentResponse<T> : ApiResponse where T : class, new()
    {
        public bool In_matchmaking { get; set; }
        public MatchmakingInfo<T>? Matchmaking { get; set; }
        public List<MatchmakingRequestBase> Pending_requests { get; set; } = new();
    }

    public class MatchmakingRequestBase
    {
        public string Request_id { get; set; } = string.Empty;
        public string Matchmaking_id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset Requested_at { get; set; }
        public DateTimeOffset? Responded_at { get; set; }
    }

    public class MatchmakingInfo<T> where T : class, new()
    {
        public string Matchmaking_id { get; set; } = string.Empty;
        public bool Is_host { get; set; }
        public int Max_players { get; set; }
        public int Current_players { get; set; }
        public bool Strict_full { get; set; }
        public bool Join_by_requests { get; set; }
        public DateTimeOffset Joined_at { get; set; }
        public string Player_status { get; set; } = string.Empty;
        public DateTimeOffset Last_heartbeat { get; set; }
        public DateTimeOffset Lobby_heartbeat { get; set; }
        public bool Is_started { get; set; }
        public DateTimeOffset? Started_at { get; set; }
        public T? Rules { get; set; }
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
        public DateTimeOffset Joined_at { get; set; }
        public DateTimeOffset Last_heartbeat { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Player_name { get; set; } = string.Empty;
        public int Seconds_since_heartbeat { get; set; }
        public bool Is_host { get; set; }
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

    public class LeaderboardResponse<T> : ApiResponse where T : class, new()
    {
        public List<LeaderboardPlayer<T>> Leaderboard { get; set; } = new();
        public int Total { get; set; }
        public string[] Sort_by { get; set; } = Array.Empty<string>();
        public int Limit { get; set; }
    }

    public class LeaderboardPlayer<T> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Player_data { get; set; }



        public int Rank { get; set; }
        public int Player_id { get; set; }
        public string Player_name { get; set; } = string.Empty;



        [JsonIgnore]
        public T PlayerData
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