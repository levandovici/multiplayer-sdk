using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Text;

// Unity Multiplayer API SDK
// Compatible with Unity JsonUtility - follows all Unity serialization rules
// No Dictionary, no object types, fields only (no properties), [Serializable] required

namespace MultiplayerAPI
{
    // ────────────────────────────────────────────────
    //      Base Response Classes
    // ────────────────────────────────────────────────

    [Serializable]
    public class BaseResponse
    {
        public bool success;
        public string error;
    }

    [Serializable]
    public class ApiResponse : BaseResponse
    {
        // Base class for all API responses
    }

    // ────────────────────────────────────────────────
    //      Player Management
    // ────────────────────────────────────────────────

    [Serializable]
    public class RegisterPlayerRequest
    {
        public string player_name;
        public string player_data_json; // Unity compatibility: JSON string instead of object
    }

    [Serializable]
    public class RegisterPlayerResponse : BaseResponse
    {
        public string player_id;
        public string private_key;
        public string player_name;
        public int game_id;
    }

    [Serializable]
    public class LoginResponse : BaseResponse
    {
        public PlayerInfo player;
    }

    [Serializable]
    public class PlayerInfo
    {
        public int id;
        public int game_id;
        public string player_name;
        public string player_data_json; // Unity compatibility: JSON string
        public int is_active;
        public string last_login;
        public string last_heartbeat;
        public string last_logout;
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class HeartbeatResponse : BaseResponse
    {
        public string message;
        public string last_heartbeat;
    }

    [Serializable]
    public class LogoutResponse : BaseResponse
    {
        public string message;
        public string last_logout;
    }

    [Serializable]
    public class ListPlayersResponse : BaseResponse
    {
        public int count;
        public PlayerInfo[] players; // Unity compatibility: Array instead of List
    }

    // ────────────────────────────────────────────────
    //      Game Data Management
    // ────────────────────────────────────────────────

    [Serializable]
    public class GameDataResponse : BaseResponse
    {
        public string type;
        public int game_id;
        public string data_json; // Unity compatibility: JSON string instead of object
    }

    [Serializable]
    public class PlayerDataResponse : BaseResponse
    {
        public string type;
        public int player_id;
        public string player_name;
        public string data_json; // Unity compatibility: JSON string instead of object
    }

    [Serializable]
    public class UpdateDataRequest
    {
        // Dynamic fields will be serialized as JSON string
        public string data_json;
    }

    [Serializable]
    public class UpdateDataResponse : BaseResponse
    {
        public string message;
        public string updated_at;
    }

    // ────────────────────────────────────────────────
    //      Time Management
    // ────────────────────────────────────────────────

    [Serializable]
    public class TimeResponse : BaseResponse
    {
        public string utc;
        public int timestamp;
        public string readable;
        public TimeOffset offset; // Can be null
    }

    [Serializable]
    public class TimeOffset
    {
        public int offset_hours;
        public string offset_string;
        public string original_utc;
        public int original_timestamp;
    }

    // ────────────────────────────────────────────────
    //      Game Room Management
    // ────────────────────────────────────────────────

    [Serializable]
    public class CreateRoomRequest
    {
        public string room_name;
        public string password;
        public int max_players;
    }

    [Serializable]
    public class CreateRoomResponse : BaseResponse
    {
        public string room_id;
        public string room_name;
        public bool is_host;
    }

    [Serializable]
    public class RoomInfo
    {
        public string room_id;
        public string room_name;
        public int max_players;
        public int current_players;
        public bool has_password;
    }

    [Serializable]
    public class ListRoomsResponse : BaseResponse
    {
        public RoomInfo[] rooms; // Unity compatibility: Array instead of List
    }

    [Serializable]
    public class JoinRoomRequest
    {
        public string password;
    }

    [Serializable]
    public class JoinRoomResponse : BaseResponse
    {
        public string room_id;
        public string message;
    }

    [Serializable]
    public class RoomPlayer
    {
        public string player_id;
        public string player_name;
        public bool is_host;
        public bool is_online;
        public string last_heartbeat;
    }

    [Serializable]
    public class ListRoomPlayersResponse : BaseResponse
    {
        public RoomPlayer[] players;
        public string last_updated;
    }

    [Serializable]
    public class CurrentRoomStatusResponse : BaseResponse
    {
        public bool in_room;
        public CurrentRoomInfo room;
        public PendingAction[] pending_actions;
        public PendingUpdate[] pending_updates;
    }

    [Serializable]
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

    [Serializable]
    public class PendingAction
    {
        public string action_id;
        public string action_type;
        public string status;
        public string created_at;
        public string processed_at;
    }

    [Serializable]
    public class PendingUpdate
    {
        public string update_id;
        public string from_player_id;
        public string type;
        public string created_at;
        public string status;
    }

    // ────────────────────────────────────────────────
    //      Room Actions
    // ────────────────────────────────────────────────

    [Serializable]
    public class SubmitActionRequest
    {
        public string action_type;
        public string request_data_json; // Unity compatibility: JSON string instead of object
    }

    [Serializable]
    public class SubmitActionResponse : BaseResponse
    {
        public string action_id;
        public string status;
    }

    [Serializable]
    public class ActionResponse
    {
        public string action_id;
        public string action_type;
        public string response_data_json; // Unity compatibility: JSON string instead of object
        public string status;
    }

    [Serializable]
    public class PollActionsResponse : BaseResponse
    {
        public ActionResponse[] actions;
    }

    [Serializable]
    public class PendingActionInfo
    {
        public string action_id;
        public string player_id;
        public string action_type;
        public string request_data_json; // Unity compatibility: JSON string instead of object
        public string created_at;
        public string player_name;
    }

    [Serializable]
    public class GetPendingActionsResponse : BaseResponse
    {
        public PendingActionInfo[] actions;
    }

    [Serializable]
    public class CompleteActionRequest
    {
        public string status;
        public string response_data_json; // Unity compatibility: JSON string instead of object
    }

    [Serializable]
    public class CompleteActionResponse : BaseResponse
    {
        public string message;
    }

    // ────────────────────────────────────────────────
    //      Room Updates
    // ────────────────────────────────────────────────

    [Serializable]
    public class SendUpdateRequest
    {
        public string targetPlayerIds; // "all" or JSON array string
        public string type;
        public string dataJson; // Already JSON string
    }

    [Serializable]
    public class SendUpdateResponse : BaseResponse
    {
        public int updates_sent;
        public string[] update_ids;
        public string[] target_players;
    }

    [Serializable]
    public class RoomUpdate
    {
        public string update_id;
        public string from_player_id;
        public string type;
        public string data_json; // Unity compatibility: JSON string
        public string created_at;
    }

    [Serializable]
    public class PollUpdatesResponse : BaseResponse
    {
        public RoomUpdate[] updates;
        public string last_update_id;
    }

    // ────────────────────────────────────────────────
    //      Matchmaking
    // ────────────────────────────────────────────────

    [Serializable]
    public class MatchmakingLobby
    {
        public string matchmaking_id;
        public string host_player_id;
        public int max_players;
        public bool strict_full;
        public string extra_json_string; // Unity compatibility: JSON string
        public string created_at;
        public string last_heartbeat;
        public int current_players;
        public string host_name;
    }

    [Serializable]
    public class ListMatchmakingResponse : BaseResponse
    {
        public MatchmakingLobby[] lobbies;
    }

    [Serializable]
    public class CreateMatchmakingRequest
    {
        public int maxPlayers;
        public bool strictFull;
        public bool joinByRequests;
        public string extraJsonString; // JSON string
    }

    [Serializable]
    public class CreateMatchmakingResponse : BaseResponse
    {
        public string matchmaking_id;
        public int max_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool is_host;
    }

    [Serializable]
    public class JoinRequestResponse : BaseResponse
    {
        public string request_id;
        public string message;
    }

    [Serializable]
    public class JoinMatchmakingResponse : BaseResponse
    {
        public string matchmaking_id;
        public string message;
    }

    [Serializable]
    public class LeaveMatchmakingResponse : BaseResponse
    {
        public string message;
    }

    [Serializable]
    public class MatchmakingPlayer
    {
        public string player_id;
        public string joined_at;
        public string last_heartbeat;
        public string status;
        public string player_name;
        public int seconds_since_heartbeat;
        public bool is_host;
    }

    [Serializable]
    public class GetMatchmakingPlayersResponse : BaseResponse
    {
        public MatchmakingPlayer[] players;
        public string last_updated;
    }

    [Serializable]
    public class CurrentMatchmakingStatusResponse : BaseResponse
    {
        public bool in_matchmaking;
        public CurrentMatchmakingInfo matchmaking;
        public MatchmakingRequest[] pending_requests;
    }

    [Serializable]
    public class CurrentMatchmakingInfo
    {
        public string matchmaking_id;
        public bool is_host;
        public int max_players;
        public int current_players;
        public bool strict_full;
        public bool join_by_requests;
        public string extra_json_string; // Unity compatibility: JSON string
        public string joined_at;
        public string player_status;
        public string last_heartbeat;
        public string lobby_heartbeat;
        public bool is_started;
        public string started_at;
    }

    [Serializable]
    public class MatchmakingRequest
    {
        public string request_id;
        public string matchmaking_id;
        public string status;
        public string requested_at;
        public string responded_at;
    }

    [Serializable]
    public class CheckRequestStatusResponse : BaseResponse
    {
        public RequestInfo request;
    }

    [Serializable]
    public class RequestInfo
    {
        public string request_id;
        public string matchmaking_id;
        public string status;
        public string requested_at;
        public string responded_at;
        public string responded_by;
        public string responder_name;
        public bool join_by_requests;
    }

    [Serializable]
    public class RespondToRequestRequest
    {
        public string action; // "approve" or "reject"
    }

    [Serializable]
    public class RespondToRequestResponse : BaseResponse
    {
        public string message;
        public string request_id;
        public string action;
    }

    [Serializable]
    public class StartMatchmakingResponse : BaseResponse
    {
        public string room_id;
        public string room_name;
        public int players_transferred;
        public string message;
    }

    // ────────────────────────────────────────────────
    //      Leaderboard
    // ────────────────────────────────────────────────

    [Serializable]
    public class LeaderboardEntry
    {
        public int rank;
        public int player_id;
        public string player_name;
        public string player_data_json; // Unity compatibility: JSON string
    }

    [Serializable]
    public class LeaderboardRequest
    {
        public string[] sortBy;
        public int limit;
    }

    [Serializable]
    public class LeaderboardResponse : BaseResponse
    {
        public LeaderboardEntry[] leaderboard;
        public int total;
        public string[] sort_by;
        public int limit;
    }

    // ────────────────────────────────────────────────
    //      Main SDK Class
    // ────────────────────────────────────────────────

    public class MultiplayerSDK : MonoBehaviour
    {
        [Header("API Configuration")]
        public string baseUrl = "https://api.michitai.com";
        public string apiToken = "";
        public string gamePlayerToken = "";
        public float requestTimeout = 10f;

        [Header("Debug")]
        public bool enableDebugLogs = true;

        private string UnityEndpoint => $"{baseUrl}/unity";

        #region Player Management

        public void RegisterPlayer(string playerName, string playerDataJson, Action<RegisterPlayerResponse> callback)
        {
            var request = new RegisterPlayerRequest
            {
                player_name = playerName,
                player_data_json = playerDataJson
            };

            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/game_players.php/register", "POST", json, callback);
        }

        public void LoginPlayer(Action<LoginResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_players.php/login", "PUT", "", callback);
        }

        public void SendPlayerHeartbeat(Action<HeartbeatResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_players.php/heartbeat", "POST", "", callback);
        }

        public void LogoutPlayer(Action<LogoutResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_players.php/logout", "POST", "", callback);
        }

        public void ListPlayers(Action<ListPlayersResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_players.php/list", "GET", "", callback);
        }

        #endregion

        #region Game Data Management

        public void GetGameData(Action<GameDataResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_data.php/game/get", "GET", "", callback);
        }

        public void GetPlayerData(Action<PlayerDataResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_data.php/player/get", "GET", "", callback);
        }

        public void UpdateGameData(string dataJson, Action<UpdateDataResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_data.php/game/update", "PUT", dataJson, callback);
        }

        public void UpdatePlayerData(string dataJson, Action<UpdateDataResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_data.php/player/update", "PUT", dataJson, callback);
        }

        #endregion

        #region Time Management

        public void GetServerTime(Action<TimeResponse> callback, int utcOffset = 0)
        {
            string url = $"{UnityEndpoint}/time.php";
            if (utcOffset != 0)
            {
                url += $"?utc={utcOffset}";
            }
            SendRequest(url, "GET", "", callback);
        }

        #endregion

        #region Game Room Management

        public void CreateRoom(string roomName, string password, int maxPlayers, Action<CreateRoomResponse> callback)
        {
            var request = new CreateRoomRequest
            {
                room_name = roomName,
                password = password,
                max_players = maxPlayers
            };

            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/game_room.php/rooms", "POST", json, callback);
        }

        public void ListRooms(Action<ListRoomsResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/rooms", "GET", "", callback);
        }

        public void JoinRoom(string roomId, string password, Action<JoinRoomResponse> callback)
        {
            var request = new JoinRoomRequest { password = password };
            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/game_room.php/rooms/{roomId}/join", "POST", json, callback);
        }

        public void ListRoomPlayers(Action<ListRoomPlayersResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/players", "GET", "", callback);
        }

        public void LeaveRoom(Action<BaseResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/leave", "POST", "", callback);
        }

        public void SendRoomHeartbeat(Action<BaseResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/heartbeat", "POST", "", callback);
        }

        public void GetCurrentRoomStatus(Action<CurrentRoomStatusResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/current", "GET", "", callback);
        }

        #endregion

        #region Room Actions

        public void SubmitAction(string actionType, string requestDataJson, Action<SubmitActionResponse> callback)
        {
            var request = new SubmitActionRequest
            {
                action_type = actionType,
                request_data_json = requestDataJson
            };

            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/game_room.php/actions", "POST", json, callback);
        }

        public void PollActions(Action<PollActionsResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/actions/poll", "GET", "", callback);
        }

        public void GetPendingActions(Action<GetPendingActionsResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/actions/pending", "GET", "", callback);
        }

        public void CompleteAction(string actionId, string status, string responseDataJson, Action<CompleteActionResponse> callback)
        {
            var request = new CompleteActionRequest
            {
                status = status,
                response_data_json = responseDataJson
            };

            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/game_room.php/actions/{actionId}/complete", "POST", json, callback);
        }

        #endregion

        #region Room Updates

        public void SendUpdate(string targetPlayerIds, string type, string dataJson, Action<SendUpdateResponse> callback)
        {
            var request = new SendUpdateRequest
            {
                targetPlayerIds = targetPlayerIds,
                type = type,
                dataJson = dataJson
            };

            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/game_room.php/updates", "POST", json, callback);
        }

        public void PollUpdates(string lastUpdateId, Action<PollUpdatesResponse> callback)
        {
            string url = $"{UnityEndpoint}/game_room.php/updates/poll";
            if (!string.IsNullOrEmpty(lastUpdateId))
            {
                url += $"?lastUpdateId={lastUpdateId}";
            }
            SendRequest(url, "GET", "", callback);
        }

        #endregion

        #region Matchmaking

        public void ListMatchmaking(Action<ListMatchmakingResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/list", "GET", "", callback);
        }

        public void CreateMatchmaking(int maxPlayers, bool strictFull, bool joinByRequests, string extraJsonString, Action<CreateMatchmakingResponse> callback)
        {
            var request = new CreateMatchmakingRequest
            {
                maxPlayers = maxPlayers,
                strictFull = strictFull,
                joinByRequests = joinByRequests,
                extraJsonString = extraJsonString
            };

            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/matchmaking.php/create", "POST", json, callback);
        }

        public void RequestJoinMatchmaking(string matchmakingId, Action<JoinRequestResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/{matchmakingId}/request", "POST", "", callback);
        }

        public void JoinMatchmaking(string matchmakingId, Action<JoinMatchmakingResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/{matchmakingId}/join", "POST", "", callback);
        }

        public void LeaveMatchmaking(Action<LeaveMatchmakingResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/leave", "POST", "", callback);
        }

        public void GetMatchmakingPlayers(Action<GetMatchmakingPlayersResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/players", "GET", "", callback);
        }

        public void SendMatchmakingHeartbeat(Action<BaseResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/heartbeat", "POST", "", callback);
        }

        public void RemoveMatchmaking(Action<BaseResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/remove", "POST", "", callback);
        }

        public void StartMatchmaking(Action<StartMatchmakingResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/start", "POST", "", callback);
        }

        public void GetCurrentMatchmakingStatus(Action<CurrentMatchmakingStatusResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/current", "GET", "", callback);
        }

        public void CheckRequestStatus(string requestId, Action<CheckRequestStatusResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/{requestId}/status", "GET", "", callback);
        }

        public void RespondToRequest(string requestId, string action, Action<RespondToRequestResponse> callback)
        {
            var request = new RespondToRequestRequest { action = action };
            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/matchmaking.php/{requestId}/response", "POST", json, callback);
        }

        #endregion

        #region Leaderboard

        public void GetLeaderboard(string[] sortBy, int limit, Action<LeaderboardResponse> callback)
        {
            var request = new LeaderboardRequest
            {
                sortBy = sortBy,
                limit = limit
            };

            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/leaderboard.php", "POST", json, callback);
        }

        #endregion

        #region HTTP Communication

        private void SendRequest<T>(string url, string method, string bodyJson, Action<T> callback) where T : BaseResponse
        {
            StartCoroutine(SendRequestCoroutine(url, method, bodyJson, callback));
        }

        private IEnumerator SendRequestCoroutine<T>(string url, string method, string bodyJson, Action<T> callback) where T : BaseResponse
        {
            using (UnityWebRequest request = new UnityWebRequest(url, method))
            {
                request.timeout = (int)requestTimeout;

                // Set headers
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accept", "application/json");

                // Add authentication
                if (!string.IsNullOrEmpty(apiToken))
                {
                    request.SetRequestHeader("X-API-Token", apiToken);
                }

                if (!string.IsNullOrEmpty(gamePlayerToken))
                {
                    request.SetRequestHeader("X-Player-Token", gamePlayerToken);
                }

                // Add body for POST/PUT requests
                if (!string.IsNullOrEmpty(bodyJson) && (method == "POST" || method == "PUT"))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                request.downloadHandler = new DownloadHandlerBuffer();

                if (enableDebugLogs)
                {
                    Debug.Log($"[MultiplayerSDK] {method} {url}");
                    if (!string.IsNullOrEmpty(bodyJson))
                    {
                        Debug.Log($"[MultiplayerSDK] Body: {bodyJson}");
                    }
                }

                yield return request.SendWebRequest();

                string responseText = request.downloadHandler.text;

                if (enableDebugLogs)
                {
                    Debug.Log($"[MultiplayerSDK] Response: {responseText}");
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        T response = JsonUtility.FromJson<T>(responseText);
                        callback?.Invoke(response);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[MultiplayerSDK] JSON Parse Error: {e.Message}");
                        Debug.LogError($"[MultiplayerSDK] Response: {responseText}");
                        
                        // Create error response
                        var errorResponse = Activator.CreateInstance<T>();
                        errorResponse.success = false;
                        errorResponse.error = "JSON parse error";
                        callback?.Invoke(errorResponse);
                    }
                }
                else
                {
                    Debug.LogError($"[MultiplayerSDK] Request Error: {request.error}");
                    
                    // Create error response
                    var errorResponse = Activator.CreateInstance<T>();
                    errorResponse.success = false;
                    errorResponse.error = request.error;
                    callback?.Invoke(errorResponse);
                }
            }
        }

        #endregion

        #region Utility Methods

        public void SetApiToken(string token)
        {
            apiToken = token;
        }

        public void SetGamePlayerToken(string token)
        {
            gamePlayerToken = token;
        }

        public string SerializeToJson<T>(T obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public T DeserializeFromJson<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        #endregion
    }
}
