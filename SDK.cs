using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace michitai
{
    /// <summary>
    /// Provides a client for interacting with the MICHITAI Game API.
    /// Handles authentication, player management, game rooms, and actions.
    /// </summary>
    public class GameSDK
    {
        private readonly string _apiToken;
        private readonly string _apiPrivateToken;
        private readonly string _baseUrl;
        private static readonly HttpClient _http = new HttpClient();
        private ILogger? _logger;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Initializes a new instance of the GameSDK class.
        /// </summary>
        /// <param name="apiToken">The public API token for authentication.</param>
        /// <param name="apiPrivateToken">The private API token for privileged operations.</param>
        /// <param name="baseUrl">Base URL of the API (default: https://api.michitai.com/v1/php/).</param>
        public GameSDK(string apiToken, string apiPrivateToken, string baseUrl = "https://api.michitai.com/v1/php/", ILogger? logger = null)
        {
            _apiToken = apiToken;
            _apiPrivateToken = apiPrivateToken;
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _logger = logger;
        }

        /// <summary>
        /// Constructs a URL for API requests with the base URL, endpoint, and authentication token.
        /// </summary>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="extra">Additional query string parameters (optional).</param>
        /// <returns>Fully constructed URL string with authentication.</returns>
        private string Url(string endpoint, string extra = "")
        {
            return $"{_baseUrl}{endpoint}?api_token={_apiToken}{extra}";
        }

        /// <summary>
        /// Sends an HTTP request to the API and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into.</typeparam>
        /// <param name="method">The HTTP method (GET, POST, PUT, etc.).</param>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="body">The request body (optional). Will be serialized to JSON if provided.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized response.</returns>
        private async Task<T> Send<T>(HttpMethod method, string url, object? body = null) where T : class
        {
            var req = new HttpRequestMessage(method, url);

            if (body != null)
            {
                string json = JsonSerializer.Serialize(body, _jsonOptions);
                req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var res = await _http.SendAsync(req);
            string str = await res.Content.ReadAsStringAsync();

            T response;

            try
            {
                response = JsonSerializer.Deserialize<T>(str, _jsonOptions)!;
            }
            catch (JsonException)
            {
                _logger?.Warn(str);

                throw;
            }

            return response;
        }

        /// <summary>
        /// Registers a new player in the game.
        /// </summary>
        /// <param name="name">Display name for the player.</param>
        /// <param name="playerData">Additional player data as an object (will be serialized to JSON).</param>
        /// <returns>Task containing player registration response with player ID and private key.</returns>
        public Task<PlayerRegisterResponse> RegisterPlayer(string name, object playerData)
        {
            return Send<PlayerRegisterResponse>(
                HttpMethod.Post,
                Url("game_players.php"),
                new { player_name = name, player_data = playerData }
            );
        }

        /// <summary>
        /// Authenticates a player using their player token.
        /// </summary>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <returns>Task containing player authentication response with player information.</returns>
        public Task<PlayerAuthResponse> AuthenticatePlayer(string playerToken)
        {
            return Send<PlayerAuthResponse>(
                HttpMethod.Put,
                Url("game_players.php", $"&game_player_token={playerToken}")
            );
        }

        /// <summary>
        /// Retrieves a list of all players (requires private API token).
        /// </summary>
        /// <returns>Task containing list of players and total count.</returns>
        public Task<PlayerListResponse> GetAllPlayers()
        {
            return Send<PlayerListResponse>(HttpMethod.Get, Url("game_players.php", $"&api_private_token={_apiPrivateToken}"));
        }

        /// <summary>
        /// Retrieves game data for the current game.
        /// </summary>
        /// <returns>Task containing the game data response.</returns>
        public Task<GameDataResponse> GetGameData()
        {
            return Send<GameDataResponse>(HttpMethod.Get, Url("game_data.php"));
        }

        /// <summary>
        /// Updates the game data (requires private API token).
        /// </summary>
        /// <param name="data">The game data to update.</param>
        /// <returns>Task indicating success or failure of the update.</returns>
        public Task<SuccessResponse> UpdateGameData(object data)
        {
            return Send<SuccessResponse>(HttpMethod.Put, Url("game_data.php", $"&api_private_token={_apiPrivateToken}"), data);
        }

        /// <summary>
        /// Retrieves data for a specific player.
        /// </summary>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <returns>Task containing the player's data.</returns>
        public Task<PlayerDataResponse> GetPlayerData(string playerToken)
        {
            return Send<PlayerDataResponse>(
                HttpMethod.Get,
                Url("game_data.php", $"&game_player_token={playerToken}")
            );
        }

        /// <summary>
        /// Updates data for a specific player.
        /// </summary>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="data">The data to update for the player.</param>
        /// <returns>Task indicating success or failure of the update.</returns>
        public Task<SuccessResponse> UpdatePlayerData(string playerToken, object data)
        {
            return Send<SuccessResponse>(
                HttpMethod.Put,
                Url("game_data.php", $"&game_player_token={playerToken}"),
                data
            );
        }

        /// <summary>
        /// Retrieves the current server time.
        /// </summary>
        /// <returns>Task containing the server time in various formats.</returns>
        public Task<ServerTimeResponse> GetServerTime()
        {
            return Send<ServerTimeResponse>(
                HttpMethod.Get,
                Url("time.php")
            );
        }

        /// <summary>
        /// Creates a new game room.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <param name="roomName">Name for the new room.</param>
        /// <param name="password">Optional password for the room.</param>
        /// <param name="maxPlayers">Maximum number of players allowed in the room (default: 4).</param>
        /// <returns>Task containing the room creation response.</returns>
        public Task<RoomCreateResponse> CreateRoomAsync(
            string gamePlayerToken,
            string roomName,
            string? password = null,
            int maxPlayers = 4)
        {
            return Send<RoomCreateResponse>(
                HttpMethod.Post,
                Url("game_room.php/rooms", $"&game_player_token={gamePlayerToken}"),
                new
                {
                    room_name = roomName,
                    password = password,
                    max_players = maxPlayers
                }
            );
        }

        /// <summary>
        /// Retrieves a list of all available game rooms.
        /// </summary>
        /// <returns>Task containing the list of rooms.</returns>
        public Task<RoomListResponse> GetRoomsAsync()
        {
            return Send<RoomListResponse>(
                HttpMethod.Get,
                Url("game_room.php/rooms")
            );
        }

        /// <summary>
        /// Joins an existing game room.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <param name="roomId">ID of the room to join.</param>
        /// <param name="password">Room password if required.</param>
        /// <returns>Task containing the join room response.</returns>
        public Task<RoomJoinResponse> JoinRoomAsync(
            string gamePlayerToken,
            string roomId,
            string? password = null)
        {
            return Send<RoomJoinResponse>(
                HttpMethod.Post,
                Url($"game_room.php/rooms/{roomId}/join", $"&game_player_token={gamePlayerToken}"),
                password != null ? new { password = password } : new { password = "" }
            );
        }

        /// <summary>
        /// Leaves the current room.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <returns>Task indicating success or failure of leaving the room.</returns>
        public Task<RoomLeaveResponse> LeaveRoomAsync(string gamePlayerToken)
        {
            return Send<RoomLeaveResponse>(
                HttpMethod.Post,
                Url("game_room.php/rooms/leave", $"&game_player_token={gamePlayerToken}")
            );
        }

        /// <summary>
        /// Retrieves a list of players in the current room.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <returns>Task containing the list of players in the room.</returns>
        public Task<RoomPlayersResponse> GetRoomPlayersAsync(string gamePlayerToken)
        {
            return Send<RoomPlayersResponse>(
                HttpMethod.Get,
                Url("game_room.php/players", $"&game_player_token={gamePlayerToken}")
            );
        }

        /// <summary>
        /// Sends a heartbeat to keep the player's session alive.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <returns>Task containing the heartbeat response.</returns>
        public Task<HeartbeatResponse> SendHeartbeatAsync(string gamePlayerToken)
        {
            return Send<HeartbeatResponse>(
                HttpMethod.Post,
                Url("game_room.php/players/heartbeat", $"&game_player_token={gamePlayerToken}")
            );
        }

        /// <summary>
        /// Submits a new game action.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <param name="actionType">Type of the action being submitted.</param>
        /// <param name="requestData">Data associated with the action.</param>
        /// <returns>Task containing the action submission response.</returns>
        public Task<ActionSubmitResponse> SubmitActionAsync(
            string gamePlayerToken,
            string actionType,
            object requestData)
        {
            return Send<ActionSubmitResponse>(
                HttpMethod.Post,
                Url("game_room.php/actions", $"&game_player_token={gamePlayerToken}"),
                new
                {
                    action_type = actionType,
                    request_data = requestData
                }
            );
        }

        /// <summary>
        /// Polls for new actions that need to be processed.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <returns>Task containing any pending actions.</returns>
        public Task<ActionPollResponse> PollActionsAsync(string gamePlayerToken)
        {
            return Send<ActionPollResponse>(
                HttpMethod.Get,
                Url("game_room.php/actions/poll", $"&game_player_token={gamePlayerToken}")
            );
        }

        /// <summary>
        /// Retrieves a list of pending actions for the player.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <returns>Task containing the list of pending actions.</returns>
        public Task<ActionPendingResponse> GetPendingActionsAsync(string gamePlayerToken)
        {
            return Send<ActionPendingResponse>(
                HttpMethod.Get,
                Url("game_room.php/actions/pending", $"&game_player_token={gamePlayerToken}")
            );
        }

        /// <summary>
        /// Marks an action as completed.
        /// </summary>
        /// <param name="actionId">ID of the action to complete.</param>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <param name="request">The completion request details.</param>
        /// <returns>Task indicating success or failure of the completion.</returns>
        public Task<ActionCompleteResponse> CompleteActionAsync(
            string actionId,
            string gamePlayerToken,
            ActionCompleteRequest request)
        {
            return Send<ActionCompleteResponse>(
                HttpMethod.Post,
                Url($"game_room.php/actions/{actionId}/complete", $"&game_player_token={gamePlayerToken}"),
                new
                {
                    status = request.Status,
                    response_data = request.Response_data
                }
            );
        }

        /// <summary>
        /// Sends updates to specific players in the room.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <param name="request">The update request containing target players and data.</param>
        /// <returns>Task containing the update response.</returns>
        public Task<UpdatePlayersResponse> UpdatePlayersAsync(
            string gamePlayerToken,
            UpdatePlayersRequest request)
        {
            return Send<UpdatePlayersResponse>(
                HttpMethod.Post,
                Url("game_room.php/updates", $"&game_player_token={gamePlayerToken}"),
                new
                {
                    targetPlayerIds = request.TargetPlayerIds,
                    type = request.Type,
                    dataJson = request.DataJson
                }
            );
        }

        /// <summary>
        /// Polls for player updates in the room.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <param name="lastUpdateId">Optional last update ID to get updates after this point.</param>
        /// <returns>Task containing the poll updates response.</returns>
        public Task<PollUpdatesResponse> PollUpdatesAsync(
            string gamePlayerToken,
            string? lastUpdateId = null)
        {
            string extra = $"&game_player_token={gamePlayerToken}";
            if (!string.IsNullOrEmpty(lastUpdateId))
            {
                extra += $"&lastUpdateId={lastUpdateId}";
            }

            return Send<PollUpdatesResponse>(
                HttpMethod.Get,
                Url("game_room.php/updates/poll", extra)
            );
        }

        /// <summary>
        /// Gets the current room information for the player.
        /// </summary>
        /// <param name="gamePlayerToken">The player's authentication token.</param>
        /// <returns>Task containing the current room response.</returns>
        public Task<CurrentRoomResponse> GetCurrentRoomAsync(string gamePlayerToken)
        {
            return Send<CurrentRoomResponse>(
                HttpMethod.Get,
                Url("game_room.php/current", $"&game_player_token={gamePlayerToken}")
            );
        }
    }



    public interface ILogger
    {
        void Log(string message);
        void Warn(string message);
        void Error(string message);
    }



    public class PlayerRegisterResponse
    {
        public bool Success { get; set; }
        public required string Player_id { get; set; }
        public required string Private_key { get; set; }
        public required string Player_name { get; set; }
        public int Game_id { get; set; }
    }

    public class PlayerAuthResponse
    {
        public bool Success { get; set; }
        public required PlayerInfo Player { get; set; }
    }

    public class PlayerListResponse
    {
        public bool Success { get; set; }
        public int Count { get; set; }
        public required List<PlayerShort> Players { get; set; }
    }

    public class PlayerShort
    {
        public int Id { get; set; }
        public required string Player_name { get; set; }
        public int Is_active { get; set; }
        public required string Last_login { get; set; }
        public required string Created_at { get; set; }
    }

    public class PlayerInfo
    {
        public int Id { get; set; }
        public int Game_id { get; set; }
        public required string Player_name { get; set; }
        public required Dictionary<string, object> Player_data { get; set; }
        public int Is_active { get; set; }
        public required string Last_login { get; set; }
        public required string Created_at { get; set; }
        public required string Updated_at { get; set; }
    }

    public class GameDataResponse
    {
        public bool Success { get; set; }
        public required string Type { get; set; }
        public int Game_id { get; set; }
        public required Dictionary<string, object> Data { get; set; }
    }

    public class PlayerDataResponse
    {
        public bool Success { get; set; }
        public required string Type { get; set; }
        public int Player_id { get; set; }
        public required string Player_name { get; set; }
        public required Dictionary<string, object> Data { get; set; }
    }

    public class SuccessResponse
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public required string Updated_at { get; set; }
    }

    public class ServerTimeResponse
    {
        public bool Success { get; set; }
        public required string Utc { get; set; }
        public long Timestamp { get; set; }
        public required string Readable { get; set; }
    }

    public class RoomCreateResponse
    {
        public bool Success { get; set; }
        public required string Room_id { get; set; }
        public required string Room_name { get; set; }
        public bool Is_host { get; set; }
    }

    public class RoomShort
    {
        public required string Room_id { get; set; }
        public required string Room_name { get; set; }
        public int Max_players { get; set; }
        public int Current_players { get; set; }
        public int Has_password { get; set; }
    }

    public class RoomListResponse
    {
        public bool Success { get; set; }
        public required List<RoomShort> Rooms { get; set; }
    }

    public class RoomJoinResponse
    {
        public bool Success { get; set; }
        public required string Room_id { get; set; }
        public required string Message { get; set; }
    }

    public class RoomPlayer
    {
        public required string Player_id { get; set; }
        public required string Player_name { get; set; }
        public int Is_host { get; set; }
        public int Is_online { get; set; }
        public required string Last_heartbeat { get; set; }
    }

    public class RoomPlayersResponse
    {
        public bool Success { get; set; }
        public required List<RoomPlayer> Players { get; set; }
        public required string Last_updated { get; set; }
    }

    public class RoomLeaveResponse
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
    }

    public class HeartbeatResponse
    {
        public bool Success { get; set; }
        public required string Status { get; set; }
    }

    public class ActionSubmitResponse
    {
        public bool Success { get; set; }
        public required string Action_id { get; set; }
        public required string Status { get; set; }
    }

    public class ActionInfo
    {
        public required string Action_id { get; set; }
        public required string Action_type { get; set; }
        public object? Response_data { get; set; }
        public required string Status { get; set; }
    }

    public class ActionPollResponse
    {
        public bool Success { get; set; }
        public required List<ActionInfo> Actions { get; set; }
    }

    public class PendingAction
    {
        public required string Action_id { get; set; }
        public required string Player_id { get; set; }
        public required string Action_type { get; set; }
        public object? Request_data { get; set; }
        public required string Created_at { get; set; }
        public required string Player_name { get; set; }
    }

    public class ActionPendingResponse
    {
        public bool Success { get; set; }
        public required List<PendingAction> Actions { get; set; }
    }

    public class ActionCompleteRequest
    {
        public ActionStatus Status { get; set; }
        public object? Response_data { get; set; }



        public ActionCompleteRequest(ActionStatus status, object? responseData = null)
        {
            Status = status;
            Response_data = responseData;
        }
    }

    public class ActionCompleteResponse
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
    }

    public enum ActionStatus
    {
        Pending,
        Processing,
        Completed,
        Failed,
        Read
    }

    public class UpdatePlayersRequest
    {
        public object? TargetPlayerIds { get; set; }
        public required string Type { get; set; }
        public required object DataJson { get; set; }

        [SetsRequiredMembers]
        public UpdatePlayersRequest(object targetPlayerIds, string type, object dataJson)
        {
            TargetPlayerIds = targetPlayerIds;
            Type = type;
            DataJson = dataJson;
        }
    }

    public class UpdatePlayersResponse
    {
        public bool Success { get; set; }
        public int Updates_sent { get; set; }
        public required List<string> Update_ids { get; set; }
        public required List<string> Target_players { get; set; }
    }

    public class PlayerUpdate
    {
        public required string Update_id { get; set; }
        public required string From_player_id { get; set; }
        public required string Type { get; set; }
        public required object Data_json { get; set; }
        public required string Created_at { get; set; }
    }

    public class PollUpdatesResponse
    {
        public bool Success { get; set; }
        public required List<PlayerUpdate> Updates { get; set; }
        public required string Last_update_id { get; set; }
    }

    public class CurrentRoomInfo
    {
        public required string Room_id { get; set; }
        public required string Room_name { get; set; }
        public bool Is_host { get; set; }
        public bool Is_online { get; set; }
        public int Max_players { get; set; }
        public int Current_players { get; set; }
        public bool Has_password { get; set; }
        public bool Is_active { get; set; }
        public required string Player_name { get; set; }
        public required string Joined_at { get; set; }
        public required string Last_heartbeat { get; set; }
        public required string Room_created_at { get; set; }
        public required string Room_last_activity { get; set; }
    }

    public class CurrentRoomResponse
    {
        public bool Success { get; set; }
        public bool In_room { get; set; }
        public CurrentRoomInfo? Room { get; set; }
        public required List<object> Pending_actions { get; set; }
        public required List<object> Pending_updates { get; set; }
    }
}