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

        // ==================== PLAYER ====================
        public Task<PlayerRegisterResponse> RegisterPlayer(string name, string playerDataJson = "", CancellationToken ct = default)
            => Send<PlayerRegisterResponse>(HttpMethod.Post, Url(Endpoints.GamePlayersRegister), new PlayerRegisterRequest { player_name = name, player_data_json = playerDataJson }, ct);

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
            => Send<LeaderboardResponse<T>>(HttpMethod.Post, Url(Endpoints.Leaderboard), new LeaderboardRequest { sort_by = sortBy, limit = limit }, ct);

        // ==================== TIME ====================
        public Task<ServerTimeResponse> GetServerTime(CancellationToken ct = default)
            => Send<ServerTimeResponse>(HttpMethod.Get, Url(Endpoints.Time), null, ct);

        public Task<ServerTimeWithOffsetResponse> GetServerTimeWithOffset(int utcOffset, CancellationToken ct = default)
            => Send<ServerTimeWithOffsetResponse>(HttpMethod.Get, Url(Endpoints.Time, $"&utc={utcOffset:+#;-#}"), null, ct);

        // ==================== GAME ROOMS ====================
        public Task<RoomCreateResponse> CreateRoomAsync(string playerToken, string roomName, string password = null, int maxPlayers = 4, CancellationToken ct = default)
            => Send<RoomCreateResponse>(HttpMethod.Post, Url(Endpoints.GameRoomCreate, $"&player_token={playerToken}"),
                new { room_name = roomName, password, max_players = maxPlayers }, ct);

        public Task<RoomListResponse> GetRoomsAsync(CancellationToken ct = default)
            => Send<RoomListResponse>(HttpMethod.Get, Url(Endpoints.GameRoomList), null, ct);

        public Task<RoomJoinResponse> JoinRoomAsync(string playerToken, string roomId, string password = null, CancellationToken ct = default)
            => Send<RoomJoinResponse>(HttpMethod.Post, Url(string.Format(Endpoints.GameRoomJoin, roomId), $"&player_token={playerToken}"),
                password != null ? new { password } : null, ct);

        public Task<RoomLeaveResponse> LeaveRoomAsync(string playerToken, CancellationToken ct = default)
            => Send<RoomLeaveResponse>(HttpMethod.Post, Url(Endpoints.GameRoomLeave, $"&player_token={playerToken}"), null, ct);

        public Task<RoomPlayersResponse> GetRoomPlayersAsync(string playerToken, CancellationToken ct = default)
            => Send<RoomPlayersResponse>(HttpMethod.Get, Url(Endpoints.GameRoomPlayers, $"&player_token={playerToken}"), null, ct);

        public Task<HeartbeatResponse> SendRoomHeartbeatAsync(string playerToken, CancellationToken ct = default)
            => Send<HeartbeatResponse>(HttpMethod.Post, Url(Endpoints.GameRoomHeartbeat, $"&player_token={playerToken}"), null, ct);

        public Task<ActionSubmitResponse> SubmitActionAsync(string playerToken, string actionType, object requestData, CancellationToken ct = default)
            => Send<ActionSubmitResponse>(HttpMethod.Post, Url(Endpoints.GameRoomActions, $"&player_token={playerToken}"),
                new { action_type = actionType, request_data = requestData }, ct);

        public Task<ActionPollResponse> PollActionsAsync(string playerToken, CancellationToken ct = default)
            => Send<ActionPollResponse>(HttpMethod.Get, Url(Endpoints.GameRoomActionsPoll, $"&player_token={playerToken}"), null, ct);

        public Task<ActionPendingResponse> GetPendingActionsAsync(string playerToken, CancellationToken ct = default)
            => Send<ActionPendingResponse>(HttpMethod.Get, Url(Endpoints.GameRoomActionsPending, $"&player_token={playerToken}"), null, ct);

        public Task<ActionCompleteResponse> CompleteActionAsync(string actionId, string playerToken, ActionCompleteRequest request, CancellationToken ct = default)
            => Send<ActionCompleteResponse>(HttpMethod.Post, Url(string.Format(Endpoints.GameRoomActionComplete, actionId), $"&player_token={playerToken}"), request, ct);

        public Task<UpdatePlayersResponse> UpdatePlayersAsync(string playerToken, UpdatePlayersRequest request, CancellationToken ct = default)
            => Send<UpdatePlayersResponse>(HttpMethod.Post, Url(Endpoints.GameRoomUpdates, $"&player_token={playerToken}"), request, ct);

        public Task<PollUpdatesResponse> PollUpdatesAsync(string playerToken, string lastUpdateId = null, CancellationToken ct = default)
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
            bool joinByRequests = false, string rules = "", CancellationToken ct = default)
            => Send<MatchmakingCreateResponse>(HttpMethod.Post, Url(Endpoints.MatchmakingCreate, $"&player_token={playerToken}"),
                new MatchmakingCreateRequest { max_players = maxPlayers, strict_full = strictFull, join_by_requests = joinByRequests, rules_json = rules }, ct);

        public Task<MatchmakingJoinRequestResponse> RequestToJoinMatchmakingAsync(string playerToken, string matchmakingId, CancellationToken ct = default)
            => Send<MatchmakingJoinRequestResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingRequest, matchmakingId), $"&player_token={playerToken}"), null, ct);

        public Task<MatchmakingRequestResponse> RespondToJoinRequestAsync(string playerToken, string requestId, MatchmakingRequestAction action, CancellationToken ct = default)
            => Send<MatchmakingRequestResponse>(HttpMethod.Post, Url(string.Format(Endpoints.MatchmakingResponse, requestId), $"&player_token={playerToken}"),
                new MatchmakingRequest { action = action.ToString().ToLower() }, ct);

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

    public enum MatchmakingRequestAction { Approve, Reject }

    // ====================== REQUEST CLASSES (Unity + JsonUtility ready) ======================

    [System.Serializable]
    public class PlayerRegisterRequest
    {
        public string player_name;
        public string player_data_json;
    }

    // ====================== RESPONSE CLASSES (Unity + JsonUtility ready) ======================

    [System.Serializable]
    public class PlayerRegisterResponse : ApiResponse
    {
        public string player_id;
        public string private_key;
        public string player_name;
        public int game_id;
    }

    [System.Serializable]
    public class PlayerAuthResponse : ApiResponse
    {
        public PlayerInfo player;
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public int id;
        public int game_id;
        public string player_name;
        public string player_data_json;           // Unity mode
        public bool is_active;
        public string last_login;
        public string last_logout;
        public string last_heartbeat;
        public string created_at;
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
        public int id;
        public string player_name;
        public int is_active;
        public string last_login;
        public string created_at;
    }

    [System.Serializable]
    public class PlayerHeartbeatResponse : ApiResponse
    {
        public string message;
        public string last_heartbeat;
    }

    [System.Serializable]
    public class PlayerLogoutResponse : ApiResponse
    {
        public string message;
        public string last_logout;
    }

    [System.Serializable]
    public class GameDataResponse<T> : ApiResponse where T : class, new()
    {
        public string type;
        public int game_id;
        public string data_json;                  // Unity mode



        public T GetData
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
        public string type;
        public int player_id;
        public string player_name;
        public string data_json;                  // Unity mode



        public T GetData
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
        public string message;
        public string updated_at;
    }

    [System.Serializable]
    public class ServerTimeResponse : ApiResponse
    {
        public string utc;
        public long timestamp;
        public string readable;
    }

    [System.Serializable]
    public class ServerTimeWithOffsetResponse : ApiResponse
    {
        public string utc;
        public long timestamp;
        public string readable;
        public TimeOffset offset;
    }

    [System.Serializable]
    public class TimeOffset
    {
        public int offset_hours;
        public string offset_string;
        public string original_utc;
        public long original_timestamp;
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
    public class RoomListResponse : ApiResponse
    {
        public List<RoomShort> rooms = new();
    }

    [System.Serializable]
    public class RoomShort
    {
        public string room_id;
        public string room_name;
        public int max_players;
        public int current_players;
        public int has_password;
    }

    [System.Serializable]
    public class RoomJoinResponse : ApiResponse
    {
        public string room_id;
        public string message;
    }

    [System.Serializable]
    public class RoomPlayersResponse : ApiResponse
    {
        public List<RoomPlayer> players = new();
        public string last_updated;
    }

    [System.Serializable]
    public class RoomPlayer
    {
        public string player_id;
        public string player_name;
        public int is_host;
        public int is_online;
        public string last_heartbeat;
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
        public string action_id;
        public string action_type;
        public string response_data_json;   // Unity mode
        public string status;
    }

    [System.Serializable]
    public class ActionPendingResponse : ApiResponse
    {
        public List<PendingAction> actions = new();
    }

    [System.Serializable]
    public class PendingAction
    {
        public string action_id;
        public string player_id;
        public string action_type;
        public string request_data_json;    // Unity mode
        public string created_at;
        public string player_name;
    }

    [System.Serializable]
    public class ActionCompleteRequest
    {
        public string status = "completed";
        public object response_data;
    }

    [System.Serializable]
    public class ActionCompleteResponse : ApiResponse
    {
        public string message;
    }

    [System.Serializable]
    public class UpdatePlayersRequest
    {
        public object targetPlayerIds;   // "all" or string[]
        public string type;
        public string dataJson;          // must be a JSON string for Unity

        public UpdatePlayersRequest(object targetPlayerIds, string type, string dataJson)
        {
            this.targetPlayerIds = targetPlayerIds;
            this.type = type;
            this.dataJson = dataJson;
        }
    }

    [System.Serializable]
    public class UpdatePlayersResponse : ApiResponse
    {
        public int updates_sent;
        public List<string> update_ids = new();
        public List<string> target_players = new();
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
        public string update_id;
        public string from_player_id;
        public string type;
        public string data_json;           // Unity mode
        public string created_at;
    }

    [System.Serializable]
    public class CurrentRoomResponse : ApiResponse
    {
        public bool in_room;
        public CurrentRoomInfo room;
        public List<string> pending_actions_json;   // raw JSON strings
        public List<string> pending_updates_json;
    }

    [System.Serializable]
    public class CurrentRoomInfo
    {
        public string room_id;
        public string room_name;
        public bool is_host;
        public bool is_online;
        public int max_players;
        public int current_players;
        public bool has_password;
        public bool is_active;
        public string player_name;
        public string joined_at;
        public string last_heartbeat;
        public string room_created_at;
        public string room_last_activity;
    }

    // ====================== MATCHMAKING ======================
    [System.Serializable]
    public class MatchmakingListResponse : ApiResponse
    {
        public List<MatchmakingLobby> lobbies = new();
    }

    [System.Serializable]
    public class MatchmakingLobby
    {
        public string matchmaking_id;
        public int host_player_id;
        public int max_players;
        public int strict_full;
        public string rules_json;           // Unity mode
        public string created_at;
        public string last_heartbeat;
        public int current_players;
        public string host_name;
    }

    [System.Serializable]
    public class MatchmakingCreateRequest
    {
        public int max_players;
        public bool strict_full;
        public bool join_by_requests;
        public string rules_json;           //Unity mode
    }

    [System.Serializable]
    public class MatchmakingCreateResponse : ApiResponse
    {
        public string matchmaking_id;
        public int max_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool is_host;
    }

    [System.Serializable]
    public class MatchmakingJoinRequestResponse : ApiResponse
    {
        public string request_id;
        public string message;
    }

    [System.Serializable]
    public class MatchmakingRequest
    {
        public string action;
    }

    [System.Serializable]
    public class MatchmakingRequestResponse : ApiResponse
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
    public class MatchmakingRequestInfo
    {
        public string request_id;
        public string matchmaking_id;
        public string status;
        public string requested_at;
        public string responded_at;
        public int responded_by;
        public string responder_name;
        public bool join_by_requests;
    }

    [System.Serializable]
    public class MatchmakingCurrentResponse : ApiResponse
    {
        public bool in_matchmaking;
        public MatchmakingInfo matchmaking;
        public List<string> pending_requests_json;
    }

    [System.Serializable]
    public class MatchmakingInfo
    {
        public string matchmaking_id;
        public bool is_host;
        public int max_players;
        public int current_players;
        public bool strict_full;
        public bool join_by_requests;
        public string rules_json;           // Unity mode
        public string joined_at;
        public string player_status;
        public string last_heartbeat;
        public string lobby_heartbeat;
        public bool is_started;
        public string started_at;
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
    public class MatchmakingPlayersResponse : ApiResponse
    {
        public List<MatchmakingPlayer> players = new();
        public string last_updated;
    }

    [System.Serializable]
    public class MatchmakingPlayer
    {
        public int player_id;
        public string joined_at;
        public string last_heartbeat;
        public string status;
        public string player_name;
        public int seconds_since_heartbeat;
        public int is_host;
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
    public class LeaderboardRequest
    {
        public string[] sort_by;
        public int limit;
    }

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
        public int rank;
        public int player_id;
        public string player_name;
        public string player_data_json;     // Unity mode



        public T GetData
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