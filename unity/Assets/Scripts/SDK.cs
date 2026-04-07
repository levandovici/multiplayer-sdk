using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace michitai
{
    // ====================== BASE RESPONSE ======================
    [System.Serializable]
    public abstract class ApiResponse
    {
        public bool success;
        public string error;
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
        public void Error(string message) => Debug.LogError($"[SDK Error] {message}");
        public void Log(string message) => Debug.Log($"[SDK] {message}");
        public void Warn(string message) => Debug.LogWarning($"[SDK Warning] {message}");
    }

    // ====================== MAIN SDK (Unity + JsonUtility) ======================
    public class GameSDK
    {
        private readonly string _apiToken;
        private readonly string _apiPrivateToken;
        private readonly string _baseUrl;
        private readonly HttpClient _http;
        private readonly ILogger _logger;
        private readonly bool _useUnityFormat;

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
                       ILogger logger = null, HttpClient httpClient = null, bool useUnityFormat = true)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _apiPrivateToken = apiPrivateToken ?? throw new ArgumentNullException(nameof(apiPrivateToken));
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _logger = logger ?? new ConsoleLogger();
            _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _useUnityFormat = useUnityFormat;
        }

        private string Url(string endpoint, string extra = "")
        {
            string format = _useUnityFormat ? "unity" : "json";
            return $"{_baseUrl}{endpoint}?api_token={_apiToken}&format={format}{extra}";
        }

        private async Task<T> Send<T>(HttpMethod method, string url, object body = null, CancellationToken ct = default) where T : ApiResponse, new()
        {
            var req = new HttpRequestMessage(method, url);

            if (body != null)
            {
                string jsonBody = JsonUtility.ToJson(body);
                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            var res = await _http.SendAsync(req, ct);
            string responseText = await res.Content.ReadAsStringAsync();

            _logger.Log($"API Response: {responseText}");

            if (!res.IsSuccessStatusCode)
            {
                _logger.Error($"HTTP {(int)res.StatusCode}: {responseText}");
                throw new ApiException($"HTTP error {(int)res.StatusCode}", responseText);
            }

            try
            {
                T response = JsonUtility.FromJson<T>(responseText) ?? new T();

                if (!response.success)
                {
                    _logger.Error($"API Error: {response.error ?? "Unknown error"}");
                    throw new ApiException(response.error ?? "Unknown API error", responseText);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Warn($"JsonUtility deserialization failed: {ex.Message}. Raw: {responseText}");
                throw new ApiException("Failed to deserialize response with JsonUtility", responseText, ex);
            }
        }

        internal static DateTimeOffset? ParseUtc(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            if (DateTimeOffset.TryParse(value, out var dto))
                return dto;

            return null;
        }

        // ==================== PLAYER ====================
        public Task<PlayerRegisterResponse> RegisterPlayer<T>(string name, T playerData = null, CancellationToken ct = default) where T : class, new()
            => Send<PlayerRegisterResponse>(HttpMethod.Post, Url(Endpoints.GamePlayersRegister), new PlayerRegisterRequest(name, JsonUtility.ToJson(playerData)), ct);

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
            => Send<LeaderboardResponse<T>>(HttpMethod.Post, Url(Endpoints.Leaderboard), new LeaderboardRequest { sort_by = sortBy, limit = limit }, ct);

        // ==================== TIME ====================
        public Task<ServerTimeResponse> GetServerTime(CancellationToken ct = default)
            => Send<ServerTimeResponse>(HttpMethod.Get, Url(Endpoints.Time), null, ct);

        public Task<ServerTimeWithOffsetResponse> GetServerTimeWithOffset(int utcOffset, CancellationToken ct = default)
            => Send<ServerTimeWithOffsetResponse>(HttpMethod.Get, Url(Endpoints.Time, $"&utc={utcOffset:+#;-#}"), null, ct);

        // ==================== GAME ROOMS ====================
        public Task<RoomCreateResponse> CreateRoomAsync<TPlayerData, TRules>(string playerToken, string roomName, string password = null,
            int maxPlayers = 4, bool hostSwitch = false, TPlayerData playerData = null, TRules rules = null, CancellationToken ct = default)
            where TPlayerData : class, new() where TRules : class, new()
            => Send<RoomCreateResponse>(HttpMethod.Post, Url(Endpoints.GameRoomCreate, $"&player_token={playerToken}"),
                new RoomCreateRequest(roomName, password, maxPlayers, hostSwitch, JsonUtility.ToJson(playerData), JsonUtility.ToJson(rules)), ct);

        public Task<RoomListResponse<T>> GetRoomsAsync<T>(CancellationToken ct = default) where T : class, new()
            => Send<RoomListResponse<T>>(HttpMethod.Get, Url(Endpoints.GameRoomList), null, ct);

        public Task<RoomJoinResponse> JoinRoomAsync<T>(string playerToken, string roomId, string password = null, T playerData = null, CancellationToken ct = default) where T : class, new()
            => Send<RoomJoinResponse>(HttpMethod.Post, Url(string.Format(Endpoints.GameRoomJoin, roomId), $"&player_token={playerToken}"),
                (password != null || playerData != null) ? new RoomJoinRequest(password, JsonUtility.ToJson(playerData)) : null, ct);

        public Task<RoomLeaveResponse> LeaveRoomAsync(string playerToken, CancellationToken ct = default)
            => Send<RoomLeaveResponse>(HttpMethod.Post, Url(Endpoints.GameRoomLeave, $"&player_token={playerToken}"), null, ct);

        public Task<RoomPlayersResponse<T>> GetRoomPlayersAsync<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<RoomPlayersResponse<T>>(HttpMethod.Get, Url(Endpoints.GameRoomPlayers, $"&player_token={playerToken}"), null, ct);

        public Task<HeartbeatResponse> SendRoomHeartbeatAsync(string playerToken, CancellationToken ct = default)
            => Send<HeartbeatResponse>(HttpMethod.Post, Url(Endpoints.GameRoomHeartbeat, $"&player_token={playerToken}"), null, ct);

        public Task<ActionSubmitResponse> SubmitActionAsync<T>(string playerToken, string actionType, T requestData = null, CancellationToken ct = default) where T : class, new()
            => Send<ActionSubmitResponse>(HttpMethod.Post, Url(Endpoints.GameRoomActions, $"&player_token={playerToken}"),
                new ActionSubmitRequest(actionType, JsonUtility.ToJson(requestData)), ct);

        public Task<ActionPollResponse> PollActionsAsync(string playerToken, CancellationToken ct = default)
            => Send<ActionPollResponse>(HttpMethod.Get, Url(Endpoints.GameRoomActionsPoll, $"&player_token={playerToken}"), null, ct);

        public Task<ActionPendingResponse<T>> GetPendingActionsAsync<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<ActionPendingResponse<T>>(HttpMethod.Get, Url(Endpoints.GameRoomActionsPending, $"&player_token={playerToken}"), null, ct);

        public Task<ActionCompleteResponse> CompleteActionAsync<T>(string actionId, string playerToken, ActionComplete<T> request, CancellationToken ct = default) where T : class, new()
            => Send<ActionCompleteResponse>(HttpMethod.Post, Url(string.Format(Endpoints.GameRoomActionComplete, actionId), $"&player_token={playerToken}"),
                new ActionCompleteRequest(request.Status.ToString().ToLower(), JsonUtility.ToJson(request.ResponseData)), ct);

        public Task<UpdatePlayersResponse> UpdatePlayersAsync<T>(string playerToken, UpdatePlayers<T> request, CancellationToken ct = default) where T : class, new()
            => Send<UpdatePlayersResponse>(HttpMethod.Post, Url(Endpoints.GameRoomUpdates, $"&player_token={playerToken}"),
                new UpdatePlayersRequest(request.TargetPlayers, request.Type, JsonUtility.ToJson(request.Data), request.TargetPlayersIds), ct);

        public Task<PollUpdatesResponse> PollUpdatesAsync(string playerToken, string lastUpdateId = null, CancellationToken ct = default)
        {
            string extra = $"&player_token={playerToken}";
            if (!string.IsNullOrEmpty(lastUpdateId)) extra += $"&lastUpdateId={lastUpdateId}";
            return Send<PollUpdatesResponse>(HttpMethod.Get, Url(Endpoints.GameRoomUpdatesPoll, extra), null, ct);
        }

        public Task<CurrentRoomResponse<T>> GetCurrentRoomAsync<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<CurrentRoomResponse<T>>(HttpMethod.Get, Url(Endpoints.GameRoomCurrent, $"&player_token={playerToken}"), null, ct);

        // ==================== MATCHMAKING ====================
        public Task<MatchmakingListResponse<T>> GetMatchmakingLobbiesAsync<T>(CancellationToken ct = default) where T : class, new()
            => Send<MatchmakingListResponse<T>>(HttpMethod.Get, Url(Endpoints.MatchmakingList), null, ct);

        public Task<MatchmakingCreateResponse> CreateMatchmakingLobbyAsync<TPlayerData, TRules>(string playerToken, string matchmakingName, int maxPlayers = 4, bool strictFull = false,
            bool joinByRequests = false, bool hostSwitch = false, TPlayerData playerData = null, TRules rules = null, CancellationToken ct = default) where TPlayerData : class, new() where TRules : class, new()
            => Send<MatchmakingCreateResponse>(HttpMethod.Post, Url(Endpoints.MatchmakingCreate, $"&player_token={playerToken}"),
                new MatchmakingCreateRequest(matchmakingName, maxPlayers, strictFull, joinByRequests, hostSwitch, JsonUtility.ToJson(playerData), JsonUtility.ToJson(rules)), ct);

        public Task<MatchmakingJoinRequestResponse> RequestToJoinMatchmakingAsync<T>(string playerToken, string matchmakingId, T playerData = null, CancellationToken ct = default) where T : class, new()
            => Send<MatchmakingJoinRequestResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingRequest, matchmakingId), $"&player_token={playerToken}"), playerData, ct);

        public Task<MatchmakingPermissionResponse> RespondToJoinRequestAsync(string playerToken, string requestId, MatchmakingRequestAction action, CancellationToken ct = default)
            => Send<MatchmakingPermissionResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingResponse, requestId), $"&player_token={playerToken}"),
                new MatchmakingPermissionRequest(action.ToString().ToLower()), ct);

        public Task<MatchmakingRequestStatusResponse> CheckJoinRequestStatusAsync(string playerToken, string requestId, CancellationToken ct = default)
            => Send<MatchmakingRequestStatusResponse>(HttpMethod.Get, Url(string.Format(Endpoints.MatchmakingRequestStatus, requestId), $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingCurrentResponse<T>> GetCurrentMatchmakingStatusAsync<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<MatchmakingCurrentResponse<T>>(HttpMethod.Get, Url(Endpoints.MatchmakingCurrent, $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingDirectJoinResponse> JoinMatchmakingDirectlyAsync<T>(string playerToken, string matchmakingId, T playerData = null, CancellationToken ct = default) where T : class, new()
            => Send<MatchmakingDirectJoinResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingJoin, matchmakingId), $"&player_token={playerToken}"), playerData, ct);

        public Task<MatchmakingLeaveResponse> LeaveMatchmakingAsync(string playerToken, CancellationToken ct = default)
            => Send<MatchmakingLeaveResponse>(HttpMethod.Post, Url(Endpoints.MatchmakingLeave, $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingPlayersResponse<T>> GetMatchmakingPlayersAsync<T>(string playerToken, CancellationToken ct = default) where T : class, new()
            => Send<MatchmakingPlayersResponse<T>>(HttpMethod.Get, Url(Endpoints.MatchmakingPlayers, $"&player_token={playerToken}"), null, ct);

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

    // ====================== PARAMETERS CLASSES (Unity + JsonUtility ready) ===================

    public class ActionComplete<T> where T : class, new()
    {
        private RoomCompleteActionStatus _status;
        private T _response_data;



        public RoomCompleteActionStatus Status
        {
            get
            {
                return _status;
            }

            private set
            {
                _status = value;
            }
        }

        public T ResponseData
        {
            get
            {
                return _response_data;
            }

            private set
            {
                _response_data = value;
            }
        }



        public ActionComplete(RoomCompleteActionStatus status, T response_data)
        {
            Status = status;
            ResponseData = response_data;
        }
    }

    public class UpdatePlayers<T> where T : class, new()
    {
        private RoomTargetPlayers _target_players;
        private int[] _target_players_ids;
        private string _type;
        private T _data;



        public RoomTargetPlayers TargetPlayers
        {
            get
            {
                return _target_players;
            }

            private set
            {
                _target_players = value;
            }
        }

        public int[] TargetPlayersIds
        {
            get
            {
                return _target_players_ids;
            }

            private set
            {
                _target_players_ids = value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }

            private set
            {
                _type = value;
            }
        }

        public T Data
        {
            get
            {
                return _data;
            }

            private set
            {
                _data = value;
            }
        }



        public UpdatePlayers(RoomTargetPlayers targetPlayers, string type, T data = null, int[] targetPlayersIds = null)
        {
            TargetPlayers = targetPlayers;
            TargetPlayersIds = targetPlayersIds;
            Type = type;
            Data = data;
        }
    }

    // ====================== REQUEST CLASSES (Unity + JsonUtility ready) ======================
    [System.Serializable]
    public class PlayerRegisterRequest
    {
        public string player_name;
        public string player_data_json;



        public PlayerRegisterRequest(string playerName, string playerDataJson)
        {
            this.player_name = playerName;
            this.player_data_json = playerDataJson;
        }
    }

    // ====================== GAME ROOMS ======================

    [System.Serializable]
    public class RoomCreateRequest
    {
        public string room_name;
        public string password;
        public int max_players;
        public bool host_switch;
        public string player_data_json;   // Unity mode
        public string rules_json;   // Unity mode



        public RoomCreateRequest(string roomName, string password, int maxPlayers,
            bool hostSwitch, string playerData, string rulesJson)
        {
            this.room_name = roomName;
            this.password = password;
            this.max_players = maxPlayers;
            this.host_switch = hostSwitch;
            this.player_data_json = playerData;
            this.rules_json = rulesJson;
        }
    }

    [System.Serializable]
    public class RoomJoinRequest
    {
        public string password;
        public string player_data_json;    // Unity mode



        public RoomJoinRequest(string password, string playerData)
        {
            this.password = password;
            this.player_data_json = playerData;
        }
    }

    [System.Serializable]
    public class ActionSubmitRequest
    {
        public string action_type;
        public string request_data_json;    // Unity mode



        public ActionSubmitRequest(string actionType, string requestDataJson)
        {
            this.action_type = actionType;
            this.request_data_json = requestDataJson;
        }
    }

    [System.Serializable]
    public class ActionCompleteRequest
    {
        public string status = RoomCompleteActionStatus.Completed.ToString().ToLower();
        public string response_data_json;



        public ActionCompleteRequest(string status, string responseDataJson)
        {
            this.status = status;
            this.response_data_json = responseDataJson;
        }
    }

    [System.Serializable]
    public class UpdatePlayersRequest
    {
        public string target_players = RoomTargetPlayers.All.ToString().ToLower();
        public int[] target_players_ids;
        public string type;
        public string data_json;          // must be a JSON string for Unity



        public UpdatePlayersRequest(RoomTargetPlayers targetPlayers, string type, string dataJson = null, int[] targetPlayerIds = null)
        {
            this.target_players = targetPlayers.ToString().ToLower();
            this.target_players_ids = targetPlayerIds;
            this.type = type;
            this.data_json = dataJson;
        }
    }

    // ====================== MATCHMAKING ======================

    [System.Serializable]
    public class MatchmakingCreateRequest
    {
        public string matchmaking_name;
        public int max_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool host_switch;
        public string player_data_json;   //Unity mode
        public string rules_json;   //Unity mode



        public MatchmakingCreateRequest(string matchmakingName, int maxPlayers, bool strictFull,
            bool joinByRequests, bool hostSwitch, string playerData, string rulesJson)
        {
            this.matchmaking_name = matchmakingName;
            this.max_players = maxPlayers;
            this.strict_full = strictFull;
            this.join_by_requests = joinByRequests;
            this.host_switch = hostSwitch;
            this.player_data_json = playerData;
            this.rules_json = rulesJson;
        }
    }

    [System.Serializable]
    public class MatchmakingPermissionRequest
    {
        public string action = MatchmakingRequestAction.Approve.ToString().ToLower();



        public MatchmakingPermissionRequest(string action)
        {
            this.action = action;
        }
    }

    // ====================== LEADERBOARD ======================

    [System.Serializable]
    public class LeaderboardRequest
    {
        public string[] sort_by;
        public int limit;
    }

    // ====================== RESPONSE CLASSES (Unity + JsonUtility ready) =====================

    [System.Serializable]
    public class PlayerRegisterResponse : ApiResponse
    {
        public int player_id;
        public string private_key;
        public string player_name;
        public int game_id;
    }

    [System.Serializable]
    public class PlayerAuthResponse<T> : ApiResponse where T : class, new()
    {
        public PlayerInfo<T> player;
    }

    [System.Serializable]
    public class PlayerInfo<T> where T : class, new()
    {
        [SerializeField]
        private string player_data_json;           // Unity mode



        public int id;
        public int game_id;
        public string player_name;
        public bool is_online;
        public string last_login;
        public string last_logout;
        public string last_heartbeat;
        public string created_at;



        public T PlayerData
        {
            get
            {
                return JsonUtility.FromJson<T>(player_data_json);
            }
        }
    }

    [System.Serializable]
    public class PlayerListResponse : ApiResponse
    {
        public int count;
        public List<PlayerShort> players = new();
    }

    [System.Serializable]
    public class PlayerShort
    {
        [SerializeField]
        private string last_login;
        [SerializeField]
        private string created_at;



        public int id;
        public string player_name;
        public bool is_online;



        public DateTimeOffset? LastLogin
        {
            get
            {
                return GameSDK.ParseUtc(last_login);
            }
        }

        public DateTimeOffset? CreatedAt
        {
            get
            {
                return GameSDK.ParseUtc(created_at);
            }
        }
    }

    [System.Serializable]
    public class PlayerHeartbeatResponse : ApiResponse
    {
        [SerializeField]
        private string last_heartbeat;



        public string message;



        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return GameSDK.ParseUtc(last_heartbeat);
            }
        }
    }

    [System.Serializable]
    public class PlayerLogoutResponse : ApiResponse
    {
        [SerializeField]
        private string last_logout;



        public string message;



        public DateTimeOffset? LastLogout
        {
            get
            {
                return GameSDK.ParseUtc(last_logout);
            }
        }
    }

    [System.Serializable]
    public class GameDataResponse<T> : ApiResponse where T : class, new()
    {
        [SerializeField]
        private string data_json;                  // Unity mode



        public string type;
        public int game_id;



        public T Data
        {
            get
            {
                return JsonUtility.FromJson<T>(data_json);
            }
        }
    }

    [System.Serializable]
    public class PlayerDataResponse<T> : ApiResponse where T : class, new()
    {
        [SerializeField]
        private string data_json;                  // Unity mode



        public string type;
        public int player_id;
        public string player_name;



        public T Data
        {
            get
            {
                return JsonUtility.FromJson<T>(data_json);
            }
        }
    }

    [System.Serializable]
    public class SuccessResponse : ApiResponse
    {
        [SerializeField]
        private string updated_at;



        public string message;



        public DateTimeOffset? UpdatedAt
        {
            get
            {
                return GameSDK.ParseUtc(updated_at);
            }
        }
    }

    [System.Serializable]
    public class ServerTimeResponse : ApiResponse
    {
        [SerializeField]
        private string utc;



        public long timestamp;
        public string readable;



        public DateTimeOffset? Utc
        {
            get
            {
                return GameSDK.ParseUtc(utc);
            }
        }
    }

    [System.Serializable]
    public class ServerTimeWithOffsetResponse : ServerTimeResponse
    {
        public TimeOffset offset;
    }

    [System.Serializable]
    public class TimeOffset
    {
        [SerializeField]
        private string original_utc;



        public int offset_hours;
        public string offset_string;
        public long original_timestamp;



        public DateTimeOffset? OriginalUtc
        {
            get
            {
                return GameSDK.ParseUtc(original_utc);
            }
        }
    }

    // ====================== GAME ROOMS ======================

    [System.Serializable]
    public class RoomCreateResponse : ApiResponse
    {
        public string room_id;
        public string room_name;
        public bool is_host;
    }

    [System.Serializable]
    public class RoomListResponse<T> : ApiResponse where T : class, new()
    {
        public List<RoomShort<T>> rooms = new();
    }

    [System.Serializable]
    public class RoomShort<T> where T : class, new()
    {
        [SerializeField]
        private string rules_json;   // Unity mode



        public string room_id;
        public string room_name;
        public int max_players;
        public int current_players;
        public bool has_password;
        public bool host_switch;



        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }
    }

    [System.Serializable]
    public class RoomJoinResponse : ApiResponse
    {
        public string room_id;
        public string message;
    }

    [System.Serializable]
    public class RoomPlayersResponse<T> : ApiResponse where T : class, new()
    {
        public List<RoomPlayer<T>> players = new();
        public string last_updated;
    }

    [System.Serializable]
    public class RoomPlayer<T> where T : class, new()
    {
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string player_data_json;    //Unity mode



        public int player_id;
        public string player_name;
        public bool is_host;
        public bool is_online;



        public T PlayerData
        {
            get
            {
                return JsonUtility.FromJson<T>(player_data_json);
            }
        }



        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return GameSDK.ParseUtc(last_heartbeat);
            }
        }
    }

    [System.Serializable]
    public class RoomLeaveResponse : ApiResponse
    {
        public string message;
    }

    [System.Serializable]
    public class HeartbeatResponse : ApiResponse
    {
        public string status;
    }

    [System.Serializable]
    public class ActionSubmitResponse : ApiResponse
    {
        public string action_id;
        public string status;
    }

    [System.Serializable]
    public class ActionPollResponse : ApiResponse
    {
        public List<ActionInfo> actions = new();
    }

    [System.Serializable]
    public class ActionInfo
    {
        [SerializeField]
        private string status;
        [SerializeField]
        private string response_data_json;   // Unity mode



        public string action_id;
        public string action_type;



        public RoomActionStatus Status
        {
            get
            {
                switch (status)
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
                        throw new System.ArgumentException($"Unknown action status: {status}");
                }
            }
        }
    }

    [System.Serializable]
    public class ActionPendingResponse<T> : ApiResponse where T : class, new()
    {
        public List<PendingAction<T>> actions = new();
    }

    [System.Serializable]
    public class PendingAction<T> where T : class, new()
    {
        [SerializeField]
        private string request_data_json;    // Unity mode
        [SerializeField]
        private string created_at;


        public string action_id;
        public int player_id;
        public string action_type;
        public string player_name;



        public T RequestData
        {
            get
            {
                return JsonUtility.FromJson<T>(request_data_json);
            }
        }

        public DateTimeOffset? CreatedAt
        {
            get
            {
                return GameSDK.ParseUtc(created_at);
            }
        }
    }

    [System.Serializable]
    public class ActionCompleteResponse : ApiResponse
    {
        public string message;
    }

    [System.Serializable]
    public class UpdatePlayersResponse : ApiResponse
    {
        public int updates_sent;
        public List<string> update_ids = new();
        public List<int> target_players_ids = new();
    }

    [System.Serializable]
    public class PollUpdatesResponse : ApiResponse
    {
        public List<PlayerUpdate> updates = new();
        public string last_update_id;
    }

    [System.Serializable]
    public class PlayerUpdate
    {
        [SerializeField]
        private string created_at;



        public string update_id;
        public int from_player_id;
        public string type;
        public string data_json;           // Unity mode



        public DateTimeOffset? CreatedAt
        {
            get
            {
                return GameSDK.ParseUtc(created_at);
            }
        }
    }

    [System.Serializable]
    public class CurrentRoomResponse<T> : ApiResponse where T : class, new()
    {
        public bool in_room;
        public CurrentRoomInfo<T> room;
        public List<string> pending_actions_json;   // raw JSON strings
        public List<string> pending_updates_json;
    }

    [System.Serializable]
    public class CurrentRoomInfo<T> where T : class, new()
    {
        [SerializeField]
        private string joined_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string room_created_at;
        [SerializeField]
        private string room_last_activity;
        [SerializeField]
        private string rules_json;   // Unity mode



        public string room_id;
        public string room_name;
        public bool is_host;
        public bool is_online;
        public int max_players;
        public int current_players;
        public bool has_password;
        public bool host_switch;
        public bool is_active;
        public string player_name;



        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }



        public DateTimeOffset? JoinedAt
        {
            get
            {
                return GameSDK.ParseUtc(joined_at);
            }
        }

        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return GameSDK.ParseUtc(last_heartbeat);
            }
        }

        public DateTimeOffset? RoomCreatedAt
        {
            get
            {
                return GameSDK.ParseUtc(room_created_at);
            }
        }

        public DateTimeOffset? RoomLastActivity
        {
            get
            {
                return GameSDK.ParseUtc(room_last_activity);
            }
        }
    }

    // ====================== MATCHMAKING ======================

    [System.Serializable]
    public class MatchmakingListResponse<T> : ApiResponse where T : class, new()
    {
        public List<MatchmakingLobby<T>> lobbies = new();
    }

    [System.Serializable]
    public class MatchmakingLobby<T> where T : class, new()
    {
        [SerializeField]
        private string created_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string rules_json;           // Unity mode



        public string matchmaking_id;
        public string matchmaking_name;
        public int host_player_id;
        public int max_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool host_switch;
        public int current_players;
        public string host_name;



        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }



        public DateTimeOffset? CreatedAt
        {
            get
            {
                return GameSDK.ParseUtc(created_at);
            }
        }

        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return GameSDK.ParseUtc(last_heartbeat);
            }
        }
    }

    [System.Serializable]
    public class MatchmakingCreateResponse : ApiResponse
    {
        public string matchmaking_id;
        public string matchmaking_name;
        public int max_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool host_switch;
        public bool is_host;
    }

    [System.Serializable]
    public class MatchmakingJoinRequestResponse : ApiResponse
    {
        public string request_id;
        public string message;
    }

    [System.Serializable]
    public class MatchmakingPermissionResponse : ApiResponse
    {
        public string message;
        public string request_id;
        public string action;
    }

    [System.Serializable]
    public class MatchmakingRequestStatusResponse : ApiResponse
    {
        public MatchmakingRequestInfo request;
    }

    [System.Serializable]
    public class MatchmakingRequestInfo : MatchmakingRequestBase
    {
        public int responded_by;
        public string responder_name;
        public bool join_by_requests;
    }

    [System.Serializable]
    public class MatchmakingCurrentResponse<T> : ApiResponse where T : class, new()
    {
        public bool in_matchmaking;
        public MatchmakingInfo<T> matchmaking;
        public List<MatchmakingRequestBase> pending_requests;
    }

    [System.Serializable]
    public class MatchmakingRequestBase
    {
        [SerializeField]
        private string requested_at;
        [SerializeField]
        private string responded_at;



        public string request_id;
        public string matchmaking_id;
        public string status;



        public DateTimeOffset? RequestedAt
        {
            get
            {
                return GameSDK.ParseUtc(requested_at);
            }
        }

        public DateTimeOffset? RespondedAt
        {
            get
            {
                return GameSDK.ParseUtc(responded_at);
            }
        }
    }


    [System.Serializable]
    public class MatchmakingInfo<T> where T : class, new()
    {
        [SerializeField]
        private string rules_json;           // Unity mode
        [SerializeField]
        private string joined_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string lobby_heartbeat;
        [SerializeField]
        private string started_at;



        public string matchmaking_id;
        public string matchmaking_name;
        public bool is_host;
        public int max_players;
        public int current_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool host_switch;
        public bool is_online;
        public bool is_started;



        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }

        public DateTimeOffset? JoinedAt
        {
            get
            {
                return GameSDK.ParseUtc(joined_at);
            }
        }

        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return GameSDK.ParseUtc(last_heartbeat);
            }
        }

        public DateTimeOffset? LobbyHeartbeat
        {
            get
            {
                return GameSDK.ParseUtc(lobby_heartbeat);
            }
        }

        public DateTimeOffset? StartedAt
        {
            get
            {
                return GameSDK.ParseUtc(started_at);
            }
        }
    }

    [System.Serializable]
    public class MatchmakingDirectJoinResponse : ApiResponse
    {
        public string message;
        public string matchmaking_id;
    }

    [System.Serializable]
    public class MatchmakingLeaveResponse : ApiResponse
    {
        public string message;
    }

    [System.Serializable]
    public class MatchmakingPlayersResponse<T> : ApiResponse where T : class, new()
    {
        public List<MatchmakingPlayer<T>> players = new();
        public string last_updated;
    }

    [System.Serializable]
    public class MatchmakingPlayer<T> where T : class, new()
    {
        [SerializeField]
        private string joined_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string player_data_json;     // Unity mode



        public int player_id;
        public bool is_online;
        public string player_name;
        public int seconds_since_heartbeat;
        public bool is_host;



        public T PlayerData
        {
            get
            {
                return JsonUtility.FromJson<T>(player_data_json);
            }
        }



        public DateTimeOffset? JoinedAt
        {
            get
            {
                return GameSDK.ParseUtc(joined_at);
            }
        }

        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return GameSDK.ParseUtc(last_heartbeat);
            }
        }
    }

    [System.Serializable]
    public class MatchmakingHeartbeatResponse : ApiResponse
    {
        public string status;
    }

    [System.Serializable]
    public class MatchmakingRemoveResponse : ApiResponse
    {
        public string message;
    }

    [System.Serializable]
    public class MatchmakingStartResponse : ApiResponse
    {
        public string room_id;
        public string room_name;
        public int players_transferred;
        public string message;
    }

    // ====================== LEADERBOARD ======================

    [System.Serializable]
    public class LeaderboardResponse<T> : ApiResponse where T : class, new()
    {
        public List<LeaderboardPlayer<T>> leaderboard = new();
        public int total;
        public string[] sort_by;
        public int limit;
    }

    [System.Serializable]
    public class LeaderboardPlayer<T> where T : class, new()
    {
        [SerializeField]
        private string player_data_json;     // Unity mode



        public int rank;
        public int player_id;
        public string player_name;



        public T PlayerData
        {
            get
            {
                return JsonUtility.FromJson<T>(player_data_json);
            }
        }
    }

    // ====================== EXCEPTION ======================

    public class ApiException : Exception
    {
        public string RawResponse { get; }
        public string ApiError { get; }

        public ApiException(string message, string rawResponse = null) : base(message)
        {
            RawResponse = rawResponse;
            ApiError = message;
        }

        public ApiException(string message, string rawResponse, Exception inner) : base(message, inner)
        {
            RawResponse = rawResponse;
            ApiError = message;
        }
    }
}