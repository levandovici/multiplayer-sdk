using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Text;

/*
 * =================================================================================================
 * Unity Multiplayer API SDK - Production Ready Version
 * =================================================================================================
 * 
 * DESCRIPTION:
 * A comprehensive Unity SDK for the Multiplayer API that provides full integration
 * with all multiplayer features including player management, game rooms, matchmaking,
 * real-time actions, and leaderboards.
 * 
 * UNITY COMPATIBILITY:
 * - Uses Unity's JsonUtility for serialization (no Dictionary, no object types)
 * - All complex data stored as JSON strings for Unity compatibility
 * - Follows Unity's [Serializable] attribute requirements
 * - Fields only (no properties) for proper serialization
 * 
 * PRODUCTION FEATURES:
 * - Comprehensive error handling with fallback responses
 * - Configurable timeout and debug logging
 * - Thread-safe callback system
 * - Memory-efficient coroutine-based HTTP requests
 * - Automatic authentication header management
 * - Production-ready logging and error reporting
 * 
 * USAGE INSTRUCTIONS:
 * 1. Attach MultiplayerSDK component to a GameObject
 * 2. Configure API token in Inspector or via SetApiToken()
 * 3. Call methods with callbacks for async responses
 * 4. Handle responses in your game logic
 * 
 * API ENDPOINTS:
 * Base URL: https://api.michitai.com/unity/
 * Authentication: API Token + Player Token (where applicable)
 * Response Format: JSON with consistent success/error structure
 * 
 * VERSION: 1.0.0 (Production Ready)
 * AUTHOR: Nichita Levandovici
 * LICENSE: All rights reserved
 * =================================================================================================
 */

namespace MultiplayerAPI
{
    // ────────────────────────────────────────────────
    //      Base Response Classes
    // ────────────────────────────────────────────────

    /// <summary>
    /// Base response class for all API calls
    /// Contains success status and error message
    /// </summary>
    /// <remarks>
    /// This is the base class that all API response classes inherit from.
    /// Provides consistent structure for success/error handling across all API endpoints.
    /// All response classes should inherit from this base class.
    /// </remarks>
    [Serializable]
    public class BaseResponse
    {
        /// <summary>Whether the API call was successful</summary>
        public bool success;
        
        /// <summary>Error message if success is false, null otherwise</summary>
        public string error;
    }

    /// <summary>
    /// Generic API response class
    /// Base class for all specific response types
    /// </summary>
    /// <remarks>
    /// This class serves as the base for all specific API response types.
    /// Inherits from BaseResponse and can be extended with additional fields.
    /// Use this as the return type for generic API methods.
    /// </remarks>
    [Serializable]
    public class ApiResponse : BaseResponse
    {
        // Base class for all API responses
        // Additional fields can be added by derived classes
    }

    // ────────────────────────────────────────────────
    //      Player Management
    // ────────────────────────────────────────────────

    /// <summary>
    /// Request data for player registration
    /// Creates a new player account in the multiplayer system
    /// </summary>
    /// <remarks>
    /// This class contains the data needed to register a new player.
    /// The player_name should be unique and player_data_json should contain
    /// any initial player data as a JSON string for Unity compatibility.
    /// The response will contain player_id and private_key for authentication.
    /// </remarks>
    [Serializable]
    public class RegisterPlayerRequest
    {
        /// <summary>Display name for the player (must be unique)</summary>
        public string player_name;
        
        /// <summary>Player data as JSON string (Unity compatibility requirement)</summary>
        public string player_data_json;
    }

    /// <summary>
    /// Response from player registration
    /// Returns player credentials and account information
    /// </summary>
    /// <remarks>
    /// This class contains the response from successful player registration.
    /// Includes player_id, private_key, player_name, and game_id.
    /// Store the private_key securely for future API calls.
    /// </remarks>
    [Serializable]
    public class RegisterPlayerResponse : BaseResponse
    {
        /// <summary>Unique player identifier assigned by the system</summary>
        public string player_id;
        
        /// <summary>Private authentication key (keep secure!)</summary>
        public string private_key;
        
        /// <summary>Player display name</summary>
        public string player_name;
        
        /// <summary>Game instance identifier</summary>
        public int game_id;
    }

    /// <summary>
    /// Response from player login
    /// Returns complete player information and session data
    /// </summary>
    /// <remarks>
    /// This class contains the response from successful player authentication.
    /// Includes complete PlayerInfo object with player details, status,
    /// last activity timestamps, and player data. Use this to get
    /// current player state after successful login.
    /// </remarks>
    [Serializable]
    public class LoginResponse : BaseResponse
    {
        /// <summary>Complete player information and current status</summary>
        public PlayerInfo player;
    }

    /// <summary>
    /// Player information container
    /// Contains player details, status, and activity data
    /// </summary>
    /// <remarks>
    /// This class contains comprehensive player information including
    /// ID, name, status, timestamps, and data. Used in
    /// login responses and other player-related API calls.
    /// The player_data_json field contains all player-specific data
    /// as a JSON string for Unity compatibility.
    /// </remarks>
    [Serializable]
    public class PlayerInfo
    {
        /// <summary>Unique player identifier</summary>
        public int id;
        public int game_id;
        
        /// <summary>Player display name</summary>
        public string player_name;
        
        /// <summary>Current player status (active, inactive, etc.)</summary>
        public int is_active;
        
        /// <summary>Timestamp of last login</summary>
        public string last_login;
        
        /// <summary>Timestamp of last activity</summary>
        public string last_heartbeat;
        
        /// <summary>Timestamp of last logout</summary>
        public string last_logout;
        
        /// <summary>Player-specific data as JSON string (Unity compatibility)</summary>
        public string player_data_json;
        
        /// <summary>Whether player is currently online</summary>
        public string created_at;
        
        public string updated_at;
    }

    /// <summary>
    /// Response from player heartbeat
    /// Returns current heartbeat timestamp
    /// </summary>
    /// <remarks>
    /// This class contains the response from heartbeat requests.
    /// The last_heartbeat field indicates when the server
    /// received the heartbeat. Use this to verify that
    /// heartbeat was processed successfully.
    /// </remarks>
    [Serializable]
    public class HeartbeatResponse : BaseResponse
    {
        public string message;
        /// <summary>Server timestamp of last heartbeat received</summary>
        public string last_heartbeat;
    }

    /// <summary>
    /// Response from player logout
    /// Returns logout timestamp
    /// </summary>
    /// <remarks>
    /// This class contains the response from player logout requests.
    /// The last_logout field indicates when the server processed
    /// the logout request. Use this to confirm successful
    /// session termination.
    /// </remarks>
    [Serializable]
    public class LogoutResponse : BaseResponse
    {
        public string message;
        /// <summary>Server timestamp of logout processing</summary>
        public string last_logout;
    }

    /// <summary>
    /// Response from list players request
    /// Returns all registered players with count
    /// </summary>
    /// <remarks>
    /// This class contains the response from listing all players.
    /// Includes total count and array of PlayerInfo objects.
    /// Useful for admin dashboards or player discovery.
    /// The players array uses Unity-compatible array type.
    /// </remarks>
    [Serializable]
    public class ListPlayersResponse : BaseResponse
    {
        /// <summary>Number of players in the response</summary>
        public int count;
        
        /// <summary>Array of player information objects</summary>
        public PlayerInfo[] players; // Unity compatibility: Array instead of List
    }

    // ────────────────────────────────────────────────
    //      Game Data Management
    // ────────────────────────────────────────────────

    /// <summary>
    /// Response from game data request
    /// Returns global game data and settings
    /// </summary>
    /// <remarks>
    /// This class contains the response from retrieving global game data.
    /// Includes game type, ID, and data as JSON string.
    /// The data_json field contains all game-wide settings
    /// and configuration as a JSON string for Unity compatibility.
    /// </remarks>
    [Serializable]
    public class GameDataResponse : BaseResponse
    {
        /// <summary>Type of game data (e.g., "settings", "config")</summary>
        public string type;
        
        /// <summary>Game instance identifier</summary>
        public int game_id;
        
        /// <summary>Game data as JSON string (Unity compatibility requirement)</summary>
        public string data_json; // Unity compatibility: JSON string instead of object
    }

    /// <summary>
    /// Response from player data request
    /// Returns individual player's stored data
    /// </summary>
    /// <remarks>
    /// This class contains the response from retrieving player-specific data.
    /// Includes data type, player information, and data as JSON string.
    /// The data_json field contains player-specific information
    /// as a JSON string for Unity compatibility.
    /// </remarks>
    [Serializable]
    public class PlayerDataResponse : BaseResponse
    {
        /// <summary>Type of player data (e.g., "inventory", "stats")</summary>
        public string type;
        
        /// <summary>Player identifier</summary>
        public int player_id;
        
        /// <summary>Player display name</summary>
        public string player_name;
        
        /// <summary>Player data as JSON string (Unity compatibility requirement)</summary>
        public string data_json; // Unity compatibility: JSON string instead of object
    }

    /// <summary>
    /// Request data for updating game or player data
    /// Contains dynamic data as JSON string
    /// </summary>
    /// <remarks>
    /// This class contains data for updating game or player information.
    /// The data_json field allows for dynamic field updates.
    /// All data is serialized as a JSON string for Unity compatibility.
    /// </remarks>
    [Serializable]
    public class UpdateDataRequest
    {
        /// <summary>Dynamic fields serialized as JSON string</summary>
        public string data_json;
    }

    /// <summary>
    /// Response from data update requests
    /// Returns update confirmation and timestamp
    /// </summary>
    /// <remarks>
    /// This class contains the response from data update operations.
    /// Includes confirmation message and update timestamp.
    /// Use this to verify that data updates were processed successfully.
    /// </remarks>
    [Serializable]
    public class UpdateDataResponse : BaseResponse
    {
        /// <summary>Confirmation message for the update operation</summary>
        public string message;
        
        /// <summary>Server timestamp when update was processed</summary>
        public string updated_at;
    }

    // ────────────────────────────────────────────────
    //      Time Management
    // ────────────────────────────────────────────────

    /// <summary>
    /// Response from server time request
    /// Returns server time and timezone information
    /// </summary>
    /// <remarks>
    /// This class contains the response from server time requests.
    /// Includes UTC timestamp, human-readable time, and timezone offset.
    /// Useful for time synchronization between client and server.
    /// </remarks>
    [Serializable]
    public class TimeResponse : BaseResponse
    {
        /// <summary>Server time in UTC format</summary>
        public string utc;
        
        /// <summary>Unix timestamp of server time</summary>
        public int timestamp;
        
        /// <summary>Human-readable server time</summary>
        public string readable;
        
        /// <summary>Timezone offset information (can be null)</summary>
        public TimeOffset offset; // Can be null
    }

    /// <summary>
    /// Timezone offset information
    /// Contains UTC offset in hours
    /// </summary>
    /// <remarks>
    /// This class contains timezone offset information.
    /// The offset_hours field represents the difference from UTC.
    /// Useful for calculating local server time.
    /// </remarks>
    [Serializable]
    public class TimeOffset
    {
        /// <summary>UTC offset in hours (e.g., -5 for EST)</summary>
        public int offset_hours;
        
        /// <summary>Offset string representation (e.g., "-05:00")</summary>
        public string offset_string;
        
        /// <summary>Original UTC timestamp before offset</summary>
        public string original_utc;
        
        /// <summary>Original Unix timestamp before offset</summary>
        public int original_timestamp;
    }

    // ────────────────────────────────────────────────
    //      Game Room Management
    // ────────────────────────────────────────────────

    /// <summary>
    /// Request data for creating game room
    /// Contains room configuration settings
    /// </summary>
    /// <remarks>
    /// This class contains the data needed to create a new game room.
    /// Includes room name, password protection, and capacity settings.
    /// The response will confirm room creation and host status.
    /// </remarks>
    [Serializable]
    public class CreateRoomRequest
    {
        /// <summary>Display name for the room</summary>
        public string room_name;
        
        /// <summary>Room password (empty for public rooms)</summary>
        public string password;
        
        /// <summary>Maximum player capacity</summary>
        public int max_players;
    }

    /// <summary>
    /// Response from room creation request
    /// Returns room details and host status
    /// </summary>
    /// <remarks>
    /// This class contains the response from successful room creation.
    /// Includes room ID, name, and confirmation of host status.
    /// Use this to verify room was created successfully.
    /// </remarks>
    [Serializable]
    public class CreateRoomResponse : BaseResponse
    {
        /// <summary>Unique room identifier</summary>
        public string room_id;
        
        /// <summary>Room display name</summary>
        public string room_name;
        
        /// <summary>Whether the creating player is the host</summary>
        public bool is_host;
    }

    /// <summary>
    /// Room information container
    /// Contains room details and occupancy data
    /// </summary>
    /// <remarks>
    /// This class contains comprehensive room information.
    /// Used in room listings and room status responses.
    /// Includes room ID, name, capacity, and password protection status.
    /// </remarks>
    [Serializable]
    public class RoomInfo
    {
        /// <summary>Unique room identifier</summary>
        public string room_id;
        
        /// <summary>Room display name</summary>
        public string room_name;
        
        /// <summary>Maximum player capacity</summary>
        public int max_players;
        
        /// <summary>Current number of players in room</summary>
        public int current_players;
        
        /// <summary>Whether room is password protected</summary>
        public bool has_password;
    }

    /// <summary>
    /// Response from list rooms request
    /// Returns array of available rooms
    /// </summary>
    /// <remarks>
    /// This class contains the response from listing all available rooms.
    /// Includes array of RoomInfo objects for room browser functionality.
    /// Uses Unity-compatible array type instead of List.
    /// </remarks>
    [Serializable]
    public class ListRoomsResponse : BaseResponse
    {
        /// <summary>Array of room information objects</summary>
        public RoomInfo[] rooms; // Unity compatibility: Array instead of List
    }

    /// <summary>
    /// Request data for joining room
    /// Contains room password for private rooms
    /// </summary>
    /// <remarks>
    /// This class contains the data needed to join an existing room.
    /// Only requires password for private rooms.
    /// The response will confirm successful room entry.
    /// </remarks>
    [Serializable]
    public class JoinRoomRequest
    {
        /// <summary>Room password (empty for public rooms)</summary>
        public string password;
    }

    /// <summary>
    /// Response from room join request
    /// Returns room entry confirmation
    /// </summary>
    /// <remarks>
    /// This class contains the response from room join attempts.
    /// Includes room ID and confirmation message.
    /// Use this to verify successful room entry.
    /// </remarks>
    [Serializable]
    public class JoinRoomResponse : BaseResponse
    {
        /// <summary>Room identifier that was joined</summary>
        public string room_id;
        
        /// <summary>Join confirmation message</summary>
        public string message;
    }

    /// <summary>
    /// Player information within a room
    /// Contains player status and activity data
    /// </summary>
    /// <remarks>
    /// This class contains player information specific to room context.
    /// Used in room player listings and status updates.
    /// Includes host status, online status, and heartbeat information.
    /// </remarks>
    [Serializable]
    public class RoomPlayer
    {
        /// <summary>Player identifier</summary>
        public string player_id;
        
        /// <summary>Player display name</summary>
        public string player_name;
        
        /// <summary>Whether player is the room host</summary>
        public bool is_host;
        
        /// <summary>Whether player is currently online</summary>
        public bool is_online;
        
        /// <summary>Timestamp of last heartbeat</summary>
        public string last_heartbeat;
    }

    /// <summary>
    /// Response from list room players request
    /// Returns all players in current room
    /// </summary>
    /// <remarks>
    /// This class contains the response from listing players in a room.
    /// Includes array of RoomPlayer objects and last update timestamp.
    /// Useful for displaying player lists and checking room occupancy.
    /// </remarks>
    [Serializable]
    public class ListRoomPlayersResponse : BaseResponse
    {
        /// <summary>Array of player information objects in room</summary>
        public RoomPlayer[] players;
        
        /// <summary>Timestamp of last player list update</summary>
        public string last_updated;
    }

    /// <summary>
    /// Response from current room status request
    /// Returns complete room state and context
    /// </summary>
    /// <remarks>
    /// This class contains comprehensive information about the player's current room.
    /// Includes room details, pending actions, and pending updates.
    /// Useful for checking room state before performing actions.
    /// </remarks>
    [Serializable]
    public class CurrentRoomStatusResponse : BaseResponse
    {
        /// <summary>Whether player is currently in a room</summary>
        public bool in_room;
        
        /// <summary>Complete room information if in_room is true</summary>
        public CurrentRoomInfo room;
        
        /// <summary>Array of pending actions to process</summary>
        public PendingAction[] pending_actions;
        
        /// <summary>Array of pending updates to process</summary>
        public PendingUpdate[] pending_updates;
    }

    /// <summary>
    /// Complete room information container
    /// Contains all room details and state data
    /// </summary>
    /// <remarks>
    /// This class contains comprehensive room information.
    /// Used in current room status responses.
    /// Includes room details, player information, and activity timestamps.
    /// </remarks>
    [Serializable]
    public class CurrentRoomInfo
    {
        /// <summary>Unique room identifier</summary>
        public string room_id;
        
        /// <summary>Room display name</summary>
        public string room_name;
        
        /// <summary>Whether the current player is the host</summary>
        public bool is_host;
        public bool is_online;
        
        /// <summary>Maximum player capacity</summary>
        public int max_players;
        
        /// <summary>Current number of players in room</summary>
        public int current_players;
        
        /// <summary>Whether room is password protected</summary>
        public bool has_password;
        public bool is_active;
        public string player_name;
        public string joined_at;
        public string last_heartbeat;
        
        /// <summary>Array of players in the room</summary>
        public RoomPlayer[] players;
        
        /// <summary>Timestamp when room was created</summary>
        public string room_created_at;
        
        /// <summary>Timestamp of last room activity</summary>
        public string room_last_activity;
    }

    /// <summary>
    /// Pending action information
    /// Contains action details and processing status
    /// </summary>
    /// <remarks>
    /// This class contains information about pending actions.
    /// Used in room status responses to show actions awaiting processing.
    /// Includes action ID, type, and timestamp information.
    /// </remarks>
    [Serializable]
    public class PendingAction
    {
        /// <summary>Unique action identifier</summary>
        public string action_id;
        
        /// <summary>Type of action (e.g., "player_ready", "game_start")</summary>
        public string action_type;
        public string status;
        
        /// <summary>Player who submitted the action</summary>
        public string player_id;
        
        /// <summary>Timestamp when action was created</summary>
        public string created_at;
        
        /// <summary>Timestamp when action was processed (null if pending)</summary>
        public string processed_at;
    }

    /// <summary>
    /// Pending update information
    /// Contains update details and delivery status
    /// </summary>
    /// <remarks>
    /// This class contains information about pending updates.
    /// Used in room status responses to show updates awaiting delivery.
    /// Includes update ID, target, and timestamp information.
    /// </remarks>
    [Serializable]
    public class PendingUpdate
    {
        /// <summary>Unique update identifier</summary>
        public string update_id;
        
        /// <summary>Player who sent the update</summary>
        public string from_player_id;
        
        /// <summary>Target players for the update</summary>
        public string target_player_ids;
        
        /// <summary>Update type identifier</summary>
        public string type;
        
        /// <summary>Timestamp when update was created</summary>
        public string created_at;
        public string status;
        
        /// <summary>Timestamp when update was delivered (null if pending)</summary>
        public string delivered_at;
    }

    // ────────────────────────────────────────────────
    //      Room Actions
    // ────────────────────────────────────────────────

    /// <summary>
    /// Request data for submitting room action
    /// Contains action type and data for processing
    /// </summary>
    /// <remarks>
    /// This class contains the data needed to submit an action for processing.
    /// The action_type defines the action type and request_data_json contains
    /// action-specific data as a JSON string for Unity compatibility.
    /// </remarks>
    [Serializable]
    public class SubmitActionRequest
    {
        /// <summary>Type of action being submitted (e.g., "player_ready", "game_start")</summary>
        public string action_type;
        
        /// <summary>Action data as JSON string (Unity compatibility requirement)</summary>
        public string request_data_json; // Unity compatibility: JSON string instead of object
    }

    /// <summary>
    /// Response from action submission
    /// Returns action ID and processing status
    /// </summary>
    /// <remarks>
    /// This class contains the response from action submission requests.
    /// Includes action ID for tracking and current processing status.
    /// Use this to track action completion and handle responses.
    /// </remarks>
    [Serializable]
    public class SubmitActionResponse : BaseResponse
    {
        /// <summary>Unique action identifier for tracking</summary>
        public string action_id;
        
        /// <summary>Current processing status of the action</summary>
        public string status;
    }

    /// <summary>
    /// Action response container
    /// Contains completed action information
    /// </summary>
    /// <remarks>
    /// This class contains information about completed actions.
    /// Used in action polling responses to show processed actions.
    /// Includes action ID, type, response data, and status.
    /// </remarks>
    [Serializable]
    public class ActionResponse
    {
        /// <summary>Unique action identifier</summary>
        public string action_id;
        
        /// <summary>Type of action that was processed</summary>
        public string action_type;
        
        /// <summary>Response data as JSON string (Unity compatibility requirement)</summary>
        public string response_data_json; // Unity compatibility: JSON string instead of object
        
        /// <summary>Final processing status</summary>
        public string status;
    }

    /// <summary>
    /// Response from action polling request
    /// Returns array of completed actions
    /// </summary>
    /// <remarks>
    /// This class contains the response from polling for completed actions.
    /// Includes array of ActionResponse objects for processed actions.
    /// Use this to retrieve actions that have been completed by other players.
    /// </remarks>
    [Serializable]
    public class PollActionsResponse : BaseResponse
    {
        /// <summary>Array of completed action responses</summary>
        public ActionResponse[] actions;
    }

    /// <summary>
    /// Pending action information container
    /// Contains unprocessed action details
    /// </summary>
    /// <remarks>
    /// This class contains information about pending actions that haven't been processed yet.
    /// Used in pending actions responses to show actions awaiting processing.
    /// Includes action ID, player information, type, and creation timestamp.
    /// </remarks>
    [Serializable]
    public class PendingActionInfo
    {
        /// <summary>Unique action identifier</summary>
        public string action_id;
        
        /// <summary>Player ID who submitted the action</summary>
        public string player_id;
        
        /// <summary>Type of action being processed</summary>
        public string action_type;
        
        /// <summary>Action data as JSON string (Unity compatibility requirement)</summary>
        public string request_data_json; // Unity compatibility: JSON string instead of object
        public string created_at;
        public string player_name;
    }

    /// <summary>
    /// Response from pending actions request
    /// Returns array of unprocessed actions
    /// </summary>
    /// <remarks>
    /// This class contains the response from polling for pending actions.
    /// Includes array of PendingActionInfo objects for actions awaiting processing.
    /// Use this to retrieve actions that need to be completed by the current player.
    /// </remarks>
    [Serializable]
    public class GetPendingActionsResponse : BaseResponse
    {
        /// <summary>Array of pending action information objects</summary>
        public PendingActionInfo[] actions;
    }

    /// <summary>
    /// Request data for completing action
    /// Contains completion status and response data
    /// </summary>
    /// <remarks>
    /// This class contains the data needed to complete a pending action.
    /// The status field indicates completion result and response_data_json
    /// contains processing results as a JSON string for Unity compatibility.
    /// </remarks>
    [Serializable]
    public class CompleteActionRequest
    {
        /// <summary>Completion status ("completed", "failed", "error", etc.)</summary>
        public string status;
        
        /// <summary>Response data as JSON string (Unity compatibility requirement)</summary>
        public string response_data_json; // Unity compatibility: JSON string instead of object
    }

    /// <summary>
    /// Response from action completion request
    /// Returns completion confirmation
    /// </summary>
    /// <remarks>
    /// This class contains the response from completing an action.
    /// Includes confirmation message for successful completion.
    /// Use this to verify that actions were processed successfully.
    /// </remarks>
    [Serializable]
    public class CompleteActionResponse : BaseResponse
    {
        /// <summary>Completion confirmation message</summary>
        public string message;
    }

    // ────────────────────────────────────────────────
    //      Room Updates
    // ────────────────────────────────────────────────

    /// <summary>
    /// Request data for sending room updates
    /// Contains update target, type, and data
    /// </summary>
    /// <remarks>
    /// This class contains the data needed to send updates to room players.
    /// The targetPlayerIds field can be "all" or a JSON array of player IDs.
    /// Use this for real-time data synchronization between players.
    /// </remarks>
    [Serializable]
    public class SendUpdateRequest
    {
        /// <summary>Target players ("all" or JSON array string)</summary>
        public string targetPlayerIds; // "all" or JSON array string
        
        /// <summary>Update type identifier (e.g., "game_state", "player_action")</summary>
        public string type;
        
        /// <summary>Update data as JSON string</summary>
        public string dataJson; // Already JSON string
    }

    /// <summary>
    /// Response from update sending request
    /// Returns delivery confirmation and tracking
    /// </summary>
    /// <remarks>
    /// This class contains the response from sending room updates.
    /// Includes delivery count, update IDs, and target player information.
    /// Use this to track update delivery and confirm successful transmission.
    /// </remarks>
    [Serializable]
    public class SendUpdateResponse : BaseResponse
    {
        /// <summary>Number of updates successfully sent</summary>
        public int updates_sent;
        
        /// <summary>Array of sent update IDs</summary>
        public string[] update_ids;
        
        /// <summary>Array of target player IDs</summary>
        public string[] target_players;
    }

    /// <summary>
    /// Room update information container
    /// Contains update details and metadata
    /// </summary>
    /// <remarks>
    /// This class contains information about room updates.
    /// Used in update polling responses to show received updates.
    /// Includes update ID, sender, type, data, and timestamp information.
    /// </remarks>
    [Serializable]
    public class RoomUpdate
    {
        /// <summary>Unique update identifier</summary>
        public string update_id;
        
        /// <summary>Player who sent the update</summary>
        public string from_player_id;
        
        /// <summary>Update type identifier</summary>
        public string type;
        
        /// <summary>Update data as JSON string (Unity compatibility requirement)</summary>
        public string data_json; // Unity compatibility: JSON string
        
        /// <summary>Timestamp when update was created</summary>
        public string created_at;
    }

    /// <summary>
    /// Response from update polling request
    /// Returns array of received updates
    /// </summary>
    /// <remarks>
    /// This class contains the response from polling for room updates.
    /// Includes array of RoomUpdate objects and last update ID for incremental polling.
    /// Use this to retrieve updates sent by other players.
    /// </remarks>
    [Serializable]
    public class PollUpdatesResponse : BaseResponse
    {
        /// <summary>Array of received update objects</summary>
        public RoomUpdate[] updates;
        
        /// <summary>Last update ID for incremental polling</summary>
        public string last_update_id;
    }

    // ────────────────────────────────────────────────
    //      Matchmaking
    // ────────────────────────────────────────────────

    /// <summary>
    /// Matchmaking lobby information container
    /// Contains lobby details and state data
    /// </summary>
    /// <remarks>
    /// This class contains comprehensive matchmaking lobby information.
    /// Used in lobby listings and matchmaking status responses.
    /// Includes lobby details, host information, and activity timestamps.
    /// </remarks>
    [Serializable]
    public class MatchmakingLobby
    {
        /// <summary>Unique lobby identifier</summary>
        public string matchmaking_id;
        
        /// <summary>Host player identifier</summary>
        public string host_player_id;
        
        /// <summary>Maximum player capacity</summary>
        public int max_players;
        
        /// <summary>Whether lobby closes automatically when full</summary>
        public bool strict_full;
        
        /// <summary>Additional lobby settings as JSON string (Unity compatibility requirement)</summary>
        public string extra_json_string; // Unity compatibility: JSON string
        
        /// <summary>Timestamp when lobby was created</summary>
        public string created_at;
        
        /// <summary>Timestamp of last lobby heartbeat</summary>
        public string last_heartbeat;
        
        /// <summary>Current number of players in lobby</summary>
        public int current_players;
        
        /// <summary>Host player display name</summary>
        public string host_name;
    }

    /// <summary>
    /// Response from matchmaking lobby listing
    /// Returns array of available lobbies
    /// </summary>
    /// <remarks>
    /// This class contains the response from listing all available matchmaking lobbies.
    /// Includes array of MatchmakingLobby objects for lobby browser functionality.
    /// Useful for displaying available lobbies to players.
    /// </remarks>
    [Serializable]
    public class ListMatchmakingResponse : BaseResponse
    {
        /// <summary>Array of available matchmaking lobby objects</summary>
        public MatchmakingLobby[] lobbies;
    }

    /// <summary>
    /// Request data for creating matchmaking lobby
    /// Contains lobby configuration and settings
    /// </summary>
    /// <remarks>
    /// This class contains the data needed to create a new matchmaking lobby.
    /// Includes player capacity, fullness behavior, join requirements, and extra settings.
    /// The extraJsonString field allows for custom lobby configuration.
    /// </remarks>
    [Serializable]
    public class CreateMatchmakingRequest
    {
        /// <summary>Maximum player capacity (2-16 recommended)</summary>
        public int maxPlayers;
        
        /// <summary>Whether lobby closes automatically when full</summary>
        public bool strictFull;
        
        /// <summary>Whether players must request to join (true) or can join directly (false)</summary>
        public bool joinByRequests;
        
        /// <summary>Additional lobby settings as JSON string (e.g., {"minLevel":5,"rank":"silver"})</summary>
        public string extraJsonString; // JSON string
    }

    /// <summary>
    /// Response from matchmaking lobby creation
    /// Returns lobby details and host status
    /// </summary>
    /// <remarks>
    /// This class contains the response from successful matchmaking lobby creation.
    /// Includes lobby ID, configuration settings, and host status confirmation.
    /// Use this to verify lobby was created successfully.
    /// </remarks>
    [Serializable]
    public class CreateMatchmakingResponse : BaseResponse
    {
        /// <summary>Unique lobby identifier</summary>
        public string matchmaking_id;
        
        /// <summary>Maximum player capacity</summary>
        public int max_players;
        
        /// <summary>Whether lobby closes automatically when full</summary>
        public bool strict_full;
        
        /// <summary>Whether players must request to join</summary>
        public bool join_by_requests;
        
        /// <summary>Whether the creating player is the host</summary>
        public bool is_host;
    }

    /// <summary>
    /// Response from join request submission
    /// Returns request ID and confirmation
    /// </summary>
    /// <remarks>
    /// This class contains the response from submitting a join request.
    /// Includes request ID for tracking and confirmation message.
    /// Use this to track join request status and await host approval.
    /// </remarks>
    [Serializable]
    public class JoinRequestResponse : BaseResponse
    {
        /// <summary>Unique request identifier for tracking</summary>
        public string request_id;
        
        /// <summary>Request submission confirmation message</summary>
        public string message;
    }

    /// <summary>
    /// Response from direct matchmaking join
    /// Returns lobby entry confirmation
    /// </summary>
    /// <remarks>
    /// This class contains the response from direct matchmaking lobby entry.
    /// Includes lobby ID and confirmation message for successful join.
    /// Use this to verify successful lobby entry.
    /// </remarks>
    [Serializable]
    public class JoinMatchmakingResponse : BaseResponse
    {
        /// <summary>Lobby identifier that was joined</summary>
        public string matchmaking_id;
        
        /// <summary>Join confirmation message</summary>
        public string message;
    }

    /// <summary>
    /// Response from matchmaking lobby leave
    /// Returns leave confirmation message
    /// </summary>
    /// <remarks>
    /// This class contains the response from leaving a matchmaking lobby.
    /// Includes confirmation message for successful lobby exit.
    /// Use this to verify successful lobby departure.
    /// </remarks>
    [Serializable]
    public class LeaveMatchmakingResponse : BaseResponse
    {
        /// <summary>Leave confirmation message</summary>
        public string message;
    }

    /// <summary>
    /// Player information within matchmaking lobby
    /// Contains player status and activity data
    /// </summary>
    /// <remarks>
    /// This class contains player information specific to matchmaking context.
    /// Used in matchmaking player listings and status updates.
    /// Includes join time, heartbeat information, and status tracking.
    /// </remarks>
    [Serializable]
    public class MatchmakingPlayer
    {
        /// <summary>Player identifier</summary>
        public string player_id;
        
        /// <summary>Timestamp when player joined the lobby</summary>
        public string joined_at;
        
        /// <summary>Timestamp of last player heartbeat</summary>
        public string last_heartbeat;
        
        /// <summary>Current player status in lobby</summary>
        public string status;
        
        /// <summary>Player display name</summary>
        public string player_name;
        
        /// <summary>Seconds since last heartbeat (for timeout detection)</summary>
        public int seconds_since_heartbeat;
        
        /// <summary>Whether player is the lobby host</summary>
        public bool is_host;
    }

    /// <summary>
    /// Response from matchmaking players listing
    /// Returns all players in current lobby
    /// </summary>
    /// <remarks>
    /// This class contains the response from listing players in a matchmaking lobby.
    /// Includes array of MatchmakingPlayer objects and last update timestamp.
    /// Useful for displaying player lists and checking lobby occupancy.
    /// </remarks>
    [Serializable]
    public class GetMatchmakingPlayersResponse : BaseResponse
    {
        /// <summary>Array of player information objects in lobby</summary>
        public MatchmakingPlayer[] players;
        
        /// <summary>Timestamp of last player list update</summary>
        public string last_updated;
    }

    /// <summary>
    /// Response from current matchmaking status request
    /// Returns complete lobby state and context
    /// </summary>
    /// <remarks>
    /// This class contains comprehensive information about the player's current matchmaking lobby.
    /// Includes lobby details, player status, and pending join requests.
    /// Useful for checking lobby state before performing actions.
    /// </remarks>
    [Serializable]
    public class CurrentMatchmakingStatusResponse : BaseResponse
    {
        /// <summary>Whether player is currently in a matchmaking lobby</summary>
        public bool in_matchmaking;
        
        /// <summary>Complete lobby information if in_matchmaking is true</summary>
        public CurrentMatchmakingInfo matchmaking;
        
        /// <summary>Array of pending join requests to process</summary>
        public MatchmakingRequest[] pending_requests;
    }

    /// <summary>
    /// Complete matchmaking lobby information container
    /// Contains all lobby details and state data
    /// </summary>
    /// <remarks>
    /// This class contains comprehensive matchmaking lobby information.
    /// Used in current matchmaking status responses.
    /// Includes lobby details, player status, and activity timestamps.
    /// </remarks>
    [Serializable]
    public class CurrentMatchmakingInfo
    {
        /// <summary>Unique lobby identifier</summary>
        public string matchmaking_id;
        
        /// <summary>Whether the current player is the host</summary>
        public bool is_host;
        
        /// <summary>Maximum player capacity</summary>
        public int max_players;
        
        /// <summary>Current number of players in lobby</summary>
        public int current_players;
        
        /// <summary>Whether lobby closes automatically when full</summary>
        public bool strict_full;
        
        /// <summary>Whether players must request to join</summary>
        public bool join_by_requests;
        
        /// <summary>Additional lobby settings as JSON string (Unity compatibility requirement)</summary>
        public string extra_json_string; // Unity compatibility: JSON string
        
        /// <summary>Timestamp when player joined the lobby</summary>
        public string joined_at;
        
        /// <summary>Current player status in lobby</summary>
        public string player_status;
        
        /// <summary>Timestamp of last player heartbeat</summary>
        public string last_heartbeat;
        
        /// <summary>Timestamp of last lobby heartbeat</summary>
        public string lobby_heartbeat;
        
        /// <summary>Whether lobby has been started (game started)</summary>
        public bool is_started;
        
        /// <summary>Timestamp when lobby was started</summary>
        public string started_at;
    }

    /// <summary>
    /// Join request information container
    /// Contains request details and status
    /// </summary>
    /// <remarks>
    /// This class contains information about join requests to matchmaking lobbies.
    /// Used in matchmaking status responses to show pending requests.
    /// Includes request ID, lobby information, and status tracking.
    /// </remarks>
    [Serializable]
    public class MatchmakingRequest
    {
        /// <summary>Unique request identifier</summary>
        public string request_id;
        
        /// <summary>Lobby identifier the request is for</summary>
        public string matchmaking_id;
        
        /// <summary>Current request status</summary>
        public string status;
        
        /// <summary>Timestamp when request was submitted</summary>
        public string requested_at;
        
        /// <summary>Timestamp when request was responded to</summary>
        public string responded_at;
    }

    /// <summary>
    /// Response from join request status check
    /// Returns request status and processing information
    /// </summary>
    /// <remarks>
    /// This class contains the response from checking join request status.
    /// Includes detailed request information and current status.
    /// Use this to track whether a join request was approved or rejected.
    /// </remarks>
    [Serializable]
    public class CheckRequestStatusResponse : BaseResponse
    {
        /// <summary>Complete request information and status</summary>
        public RequestInfo request;
    }

    /// <summary>
    /// Request information container
    /// Contains complete request details and status
    /// </summary>
    /// <remarks>
    /// This class contains comprehensive information about a join request.
    /// Used in request status responses to show detailed request information.
    /// Includes request details, status, and responder information.
    /// </remarks>
    [Serializable]
    public class RequestInfo
    {
        /// <summary>Unique request identifier</summary>
        public string request_id;
        
        /// <summary>Lobby identifier the request is for</summary>
        public string matchmaking_id;
        
        /// <summary>Current request status</summary>
        public string status;
        
        /// <summary>Timestamp when request was submitted</summary>
        public string requested_at;
        
        /// <summary>Timestamp when request was responded to</summary>
        public string responded_at;
        
        /// <summary>Player who responded to the request</summary>
        public string responded_by;
        
        /// <summary>Player display name of responder</summary>
        public string responder_name;
        
        /// <summary>Whether lobby requires join requests</summary>
        public bool join_by_requests;
    }

    /// <summary>
    /// Request data for responding to join requests
    /// Contains approval or rejection action
    /// </summary>
    /// <remarks>
    /// This class contains the data needed to respond to a join request.
    /// The action field indicates whether to approve or reject the request.
    /// Only the lobby host can respond to join requests.
    /// Used in the RespondToRequest method for lobby management.
    /// </remarks>
    [Serializable]
    public class RespondToRequestRequest
    {
        /// <summary>Response action ("approve" or "reject")</summary>
        public string action; // "approve" or "reject"
    }

    /// <summary>
    /// Response from join request handling
    /// Returns request processing confirmation
    /// </summary>
    /// <remarks>
    /// This class contains the response from handling a join request.
    /// Includes confirmation message, request ID, and action taken.
    /// Use this to verify that join requests were processed successfully.
    /// </remarks>
    [Serializable]
    public class RespondToRequestResponse : BaseResponse
    {
        /// <summary>Request processing confirmation message</summary>
        public string message;
        
        /// <summary>Request identifier that was processed</summary>
        public string request_id;
        
        /// <summary>Action taken ("approve" or "reject")</summary>
        public string action;
    }

    /// <summary>
    /// Response from matchmaking game start
    /// Returns game room details and transfer information
    /// </summary>
    /// <remarks>
    /// This class contains the response from starting a game from matchmaking.
    /// Includes room ID, name, and player transfer information.
    /// Use this to verify successful game start and room creation.
    /// </remarks>
    [Serializable]
    public class StartMatchmakingResponse : BaseResponse
    {
        /// <summary>Created game room identifier</summary>
        public string room_id;
        
        /// <summary>Created game room name</summary>
        public string room_name;
        
        /// <summary>Number of players transferred to game room</summary>
        public int players_transferred;
        
        /// <summary>Game start confirmation message</summary>
        public string message;
    }

    // ────────────────────────────────────────────────
    //      Leaderboard
    // ────────────────────────────────────────────────

    /// <summary>
    /// Leaderboard entry container
    /// Contains ranked player information
    /// </summary>
    /// <remarks>
    /// This class contains information about a single leaderboard entry.
    /// Includes player rank, name, and data used for ranking.
    /// Used in leaderboard responses to display competitive rankings.
    /// </remarks>
    [Serializable]
    public class LeaderboardEntry
    {
        /// <summary>Player rank position</summary>
        public int rank;
        
        /// <summary>Player display name</summary>
        public string player_name;
        
        /// <summary>Player data used for ranking as JSON string (Unity compatibility requirement)</summary>
        public string player_data_json;
        
        /// <summary>Player identifier</summary>
        public int player_id;
    }

    /// <summary>
    /// Request data for leaderboard retrieval
    /// Contains sorting criteria and result limit
    /// </summary>
    /// <remarks>
    /// This class contains the data needed to retrieve a ranked leaderboard.
    /// The sortBy array defines ranking criteria and limit controls result size.
    /// Useful for displaying competitive rankings and player stats.
    /// </remarks>
    [Serializable]
    public class LeaderboardRequest
    {
        /// <summary>Array of fields to sort by (e.g., ["level", "score"])</summary>
        public string[] sortBy;
        
        /// <summary>Maximum number of results to return</summary>
        public int limit;
    }

    /// <summary>
    /// Response from leaderboard request
    /// Returns ranked players with sorting information
    /// </summary>
    /// <remarks>
    /// This class contains the response from leaderboard retrieval requests.
    /// Includes array of ranked players, total count, and sorting criteria.
    /// Useful for displaying competitive rankings and player achievements.
    /// </remarks>
    [Serializable]
    public class LeaderboardResponse : BaseResponse
    {
        /// <summary>Array of ranked player entries</summary>
        public LeaderboardEntry[] leaderboard;
        
        /// <summary>Total number of entries in leaderboard</summary>
        public int total;
        
        /// <summary>Sorting criteria applied to results</summary>
        public string[] sort_by;
        
        /// <summary>Maximum number of results returned</summary>
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
        public string apiPrivateToken = "";
        public string gamePlayerToken = "";
        public float requestTimeout = 30f;

        [Header("Debug")]
        public bool enableDebugLogs = true;

        private string UnityEndpoint => $"{baseUrl}/unity";

        #region PLAYER MANAGEMENT
        // Methods for player registration, authentication, and lifecycle management

        /// <summary>
        /// Registers a new player in the multiplayer system
        /// Creates player account and returns authentication credentials
        /// </summary>
        /// <remarks>
        /// This method should be called when creating a new player account.
        /// The response contains player_id and private_key that must be stored
        /// securely for future API calls. The player_data_json should contain
        /// any initial player data as a JSON string.
        /// </remarks>
        /// <param name="playerName">Display name for the player (must be unique)</param>
        /// <param name="playerDataJson">Player data as JSON string (e.g., {"level":1,"score":0})</param>
        /// <param name="callback">Response callback with RegisterPlayerResponse result</param>
        /// <example>
        /// <code>
        /// sdk.RegisterPlayer("MyPlayer", "{\"level\":1,\"score\":0}", (response) => {
        ///     if (response.success) {
        ///         Debug.Log($"Player registered with ID: {Response.player_id}");
        ///         sdk.SetGamePlayerToken(Response.private_key);
        ///     } else {
        ///         Debug.LogError($"Registration failed: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
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

        /// <summary>
        /// Authenticates a player with existing credentials
        /// Returns complete player information and session data
        /// </summary>
        /// <remarks>
        /// Call this method after player registration to authenticate into player.
        /// Uses the previously stored player token for authentication.
        /// The response contains complete player information including current status,
        /// last activity timestamps, and player data. Store the player_id
        /// and private_key for subsequent authenticated API calls.
        /// </remarks>
        /// <param name="callback">Response callback with LoginResponse result</param>
        /// <example>
        /// <code>
        /// sdk.LoginPlayer((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Logged in as: {Response.player.player_name}");
        ///         Debug.Log($"Player ID: {Response.player.id}");
        ///         Debug.Log($"Current level: {Response.player.player_data_json}");
        ///     } else {
        ///         Debug.LogError($"Login failed: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void LoginPlayer(Action<LoginResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_players.php/login", "PUT", "", callback);
        }
        /// Call this method every 30-60 seconds while the player is active.
        /// This prevents automatic timeout and maintains the player's online status.
        /// The response contains the current heartbeat timestamp.
        /// </remarks>
        /// <param name="callback">Response callback with HeartbeatResponse result</param>
        /// <example>
        /// <code>
        /// // Call every 30 seconds
        /// InvokeRepeating(nameof(SendPlayerHeartbeat), 30f);
        /// 
        /// sdk.SendPlayerHeartbeat((response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Heartbeat sent at: {Response.last_heartbeat}");
        ///     } else {
        ///         Debug.LogError($"Heartbeat failed: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void SendPlayerHeartbeat(Action<HeartbeatResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_players.php/heartbeat", "POST", "", callback);
        }

        /// <summary>
        /// Logs out player and ends current session
        /// Invalidates player token and updates status
        /// </summary>
        /// <remarks>
        /// Call this method when the player logs out or exits the game.
        /// This will invalidate the current player token and set the player
        /// status to inactive. The response contains the logout timestamp.
        /// </remarks>
        /// <param name="callback">Response callback with LogoutResponse result</param>
        /// <example>
        /// <code>
        /// sdk.LogoutPlayer((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Player logged out at: {Response.last_logout}");
        ///         sdk.SetGamePlayerToken(""); // Clear token
        ///     } else {
        ///         Debug.LogError($"Logout failed: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void LogoutPlayer(Action<LogoutResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_players.php/logout", "POST", "", callback);
        }

        /// <summary>
        /// Retrieves list of all registered players
        /// Useful for admin functions and player discovery
        /// </summary>
        /// <remarks>
        /// This method returns all players in the game regardless of online status.
        /// Useful for admin dashboards, player lists, or debugging multiplayer issues.
        /// The response includes player count and array of PlayerInfo objects.
        /// </remarks>
        /// <param name="callback">Response callback with ListPlayersResponse result</param>
        /// <example>
        /// <code>
        /// sdk.ListPlayers((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Found {Response.count} players");
        ///         foreach (var player in Response.players) {
        ///             Debug.Log($"Player: {player.player_name} (ID: {player.id})");
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to list players: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void ListPlayers(Action<ListPlayersResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_players.php/list", "GET", "", callback, usePrivateToken: true);
        }

        #endregion

        #region Game Data Management

        /// <summary>
        /// Retrieves global game data and settings
        /// Returns game-wide configuration and state
        /// </summary>
        /// <remarks>
        /// This method retrieves game-wide data that affects all players.
        /// Useful for getting game settings, configuration, or global state.
        /// The response contains game data as a JSON string for Unity compatibility.
        /// </remarks>
        /// <param name="callback">Response callback with GameDataResponse result</param>
        /// <example>
        /// <code>
        /// sdk.GetGameData((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Game data: {Response.data_json}");
        ///         // Parse the JSON string for use in game
        ///         var gameData = sdk.DeserializeFromJson&lt;GameData&gt;(Response.data_json);
        ///     } else {
        ///         Debug.LogError($"Failed to get game data: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void GetGameData(Action<GameDataResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_data.php/game/get", "GET", "", callback);
        }

        /// <summary>
        /// Retrieves player-specific game data
        /// Returns individual player's stored data
        /// </summary>
        /// <remarks>
        /// This method retrieves data specific to the authenticated player.
        /// Useful for loading player progress, inventory, stats, or preferences.
        /// The response contains player data as a JSON string for Unity compatibility.
        /// </remarks>
        /// <param name="callback">Response callback with PlayerDataResponse result</param>
        /// <example>
        /// <code>
        /// sdk.GetPlayerData((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Player data: {Response.data_json}");
        ///         // Parse the JSON string for use in game
        ///         var playerData = sdk.DeserializeFromJson&lt;PlayerData&gt;(Response.data_json);
        ///     } else {
        ///         Debug.LogError($"Failed to get player data: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void GetPlayerData(Action<PlayerDataResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_data.php/player/get", "GET", "", callback);
        }

        /// <summary>
        /// Updates global game data and settings
        /// Requires API private token for security
        /// </summary>
        /// <remarks>
        /// This method updates game-wide data that affects all players.
        /// Requires API private token for security. Use for game settings,
        /// global configuration, or system-wide state changes.
        /// The dataJson should contain any fields you want to update.
        /// </remarks>
        /// <param name="dataJson">Game data as JSON string (e.g., {"difficulty":"hard","max_players":10})</param>
        /// <param name="callback">Response callback with UpdateDataResponse result</param>
        /// <example>
        /// <code>
        /// sdk.UpdateGameData("{\"difficulty\":\"hard\",\"max_players\":10}", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Game data updated at: {Response.updated_at}");
        ///     } else {
        ///         Debug.LogError($"Failed to update game data: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void UpdateGameData(string dataJson, Action<UpdateDataResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_data.php/game/update", "PUT", dataJson, callback, usePrivateToken: true);
        }

        /// <summary>
        /// Updates player-specific game data
        /// Modifies individual player's stored data
        /// </summary>
        /// <remarks>
        /// This method updates data specific to the authenticated player only.
        /// Use for player progress, inventory, stats, or preferences.
        /// The dataJson should contain fields specific to the player.
        /// </remarks>
        /// <param name="dataJson">Player data as JSON string (e.g., {"level":5,"score":1500})</param>
        /// <param name="callback">Response callback with UpdateDataResponse result</param>
        /// <example>
        /// <code>
        /// sdk.UpdatePlayerData("{\"level\":5,\"score\":1500}", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Player data updated at: {Response.updated_at}");
        ///     } else {
        ///         Debug.LogError($"Failed to update player data: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void UpdatePlayerData(string dataJson, Action<UpdateDataResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_data.php/player/update", "PUT", dataJson, callback);
        }

        #endregion

        #region Time Management

        /// <summary>
        /// Retrieves current server time
        /// Useful for time synchronization between client and server
        /// </summary>
        /// <remarks>
        /// This method retrieves the current server time in UTC format.
        /// Useful for synchronizing game time across all clients or calculating
        /// time-based game events. The optional utcOffset parameter allows getting
        /// time in different timezones.
        /// </remarks>
        /// <param name="callback">Response callback with TimeResponse result</param>
        /// <param name="utcOffset">Optional UTC offset in hours (default: 0)</param>
        /// <example>
        /// <code>
        /// // Get current server time
        /// sdk.GetServerTime((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Server time: {Response.utc} ({Response.readable})");
        ///     } else {
        ///         Debug.LogError($"Failed to get server time: {Response.error}");
        ///     }
        /// });
        /// 
        /// // Get server time +2 hours
        /// sdk.GetServerTime((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Server time +2h: {Response.utc} ({Response.readable})");
        ///     }
        /// });
        /// </code>
        /// </example>
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

        /// <summary>
        /// Creates a new multiplayer game room
        /// Configures room settings and makes creating player the host
        /// </summary>
        /// <remarks>
        /// This method creates a new game room with the calling player as host.
        /// The room settings include name, password protection, and maximum capacity.
        /// The response contains the room ID and confirms host status.
        /// </remarks>
        /// <param name="roomName">Display name for the room</param>
        /// <param name="password">Optional password for private rooms (empty for public)</param>
        /// <param name="maxPlayers">Maximum player capacity (2-16 recommended)</param>
        /// <param name="callback">Response callback with CreateRoomResponse result</param>
        /// <example>
        /// <code>
        /// sdk.CreateRoom("My Room", "password123", 4, (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Room created: {Response.room_name} (ID: {Response.room_id})");
        ///     } else {
        ///         Debug.LogError($"Failed to create room: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
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

        /// <summary>
        /// Retrieves list of all available game rooms
        /// Returns public rooms and room information
        /// </summary>
        /// <remarks>
        /// This method returns all game rooms that are currently available.
        /// Useful for a server browser or lobby system. The response includes
        /// room details like player count, password protection, and room status.
        /// </remarks>
        /// <param name="callback">Response callback with ListRoomsResponse result</param>
        /// <example>
        /// <code>
        /// sdk.ListRooms((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Found {Response.rooms.Length} rooms");
        ///         foreach (var room in Response.rooms) {
        ///             Debug.Log($"Room: {room.room_name} ({room.current_players}/{room.max_players})");
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to list rooms: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void ListRooms(Action<ListRoomsResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/rooms", "GET", "", callback);
        }

        /// <summary>
        /// Joins an existing multiplayer game room
        /// Attempts to enter room with optional password
        /// </summary>
        /// <remarks>
        /// This method attempts to join an existing room by ID.
        /// If the room is password-protected, the correct password must be provided.
        /// The response confirms successful room entry and provides room details.
        /// </remarks>
        /// <param name="roomId">Unique room identifier</param>
        /// <param name="password">Room password (empty for public rooms)</param>
        /// <param name="callback">Response callback with JoinRoomResponse result</param>
        /// <example>
        /// <code>
        /// sdk.JoinRoom("room123", "password123", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Joined room: {Response.room_id}");
        ///     } else {
        ///         Debug.LogError($"Failed to join room: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void JoinRoom(string roomId, string password, Action<JoinRoomResponse> callback)
        {
            var request = new JoinRoomRequest { password = password };
            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/game_room.php/rooms/{roomId}/join", "POST", json, callback);
        }

        /// <summary>
        /// Retrieves list of players in current room
        /// Returns all players in the caller's room
        /// </summary>
        /// <remarks>
        /// This method retrieves all players currently in the authenticated player's room.
        /// Useful for displaying player lists, checking room occupancy, or managing
        /// multiplayer game state. The response includes player status and activity.
        /// </remarks>
        /// <param name="callback">Response callback with ListRoomPlayersResponse result</param>
        /// <example>
        /// <code>
        /// sdk.ListRoomPlayers((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Found {Response.players.Length} players in room");
        ///         foreach (var player in Response.players) {
        ///             Debug.Log($"Player: {player.player_name} (Host: {player.is_host})");
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to list room players: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void ListRoomPlayers(Action<ListRoomPlayersResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/players", "GET", "", callback);
        }

        /// <summary>
        /// Leaves current multiplayer game room
        /// Exits room and updates player status
        /// </summary>
        /// <remarks>
        /// This method removes the authenticated player from their current room.
        /// Call this when player wants to leave a room or when game ends.
        /// The response confirms successful room exit.
        /// </remarks>
        /// <param name="callback">Response callback with BaseResponse result</param>
        /// <example>
        /// <code>
        /// sdk.LeaveRoom((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log("Successfully left room");
        ///     } else {
        ///         Debug.LogError($"Failed to leave room: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void LeaveRoom(Action<BaseResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/leave", "POST", "", callback);
        }

        /// <summary>
        /// Sends heartbeat to maintain room connection
        /// Should be called periodically while in room
        /// </summary>
        /// <remarks>
        /// Call this method every 30-60 seconds while player is in a room.
        /// This prevents automatic timeout and maintains player's room connection.
        /// The response confirms heartbeat was received.
        /// </remarks>
        /// <param name="callback">Response callback with BaseResponse result</param>
        /// <example>
        /// <code>
        /// // Call every 30 seconds while in room
        /// InvokeRepeating(nameof(SendRoomHeartbeat), 30f);
        /// 
        /// sdk.SendRoomHeartbeat((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log("Room heartbeat sent successfully");
        ///     } else {
        ///         Debug.LogError($"Room heartbeat failed: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void SendRoomHeartbeat(Action<BaseResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/heartbeat", "POST", "", callback);
        }

        /// <summary>
        /// Retrieves current room status and player information
        /// Returns complete room state and context
        /// </summary>
        /// <remarks>
        /// This method retrieves comprehensive information about the player's current room.
        /// Includes room details, player status, pending actions, and updates.
        /// Useful for checking room state before performing actions.
        /// </remarks>
        /// <param name="callback">Response callback with CurrentRoomStatusResponse result</param>
        /// <example>
        /// <code>
        /// sdk.GetCurrentRoomStatus((Response) => {
        ///     if (Response.success) {
        ///         if (Response.in_room) {
        ///             Debug.Log($"In room: {Response.room.room_name}");
        ///             Debug.Log($"Player count: {Response.room.current_players}/{Response.room.max_players}");
        ///         } else {
        ///             Debug.Log("Not in any room");
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to get room status: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void GetCurrentRoomStatus(Action<CurrentRoomStatusResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/current", "GET", "", callback);
        }

        #endregion

        #region Room Actions

        /// <summary>
        /// Submits an action for processing by other players
        /// Used for game events and player interactions
        /// </summary>
        /// <remarks>
        /// This method submits an action to be processed by other players in the room.
        /// Actions can be game events, player moves, or any multiplayer interaction.
        /// The response returns an action ID for tracking completion status.
        /// </remarks>
        /// <param name="actionType">Type of action being submitted (e.g., "player_ready", "game_start")</param>
        /// <param name="requestDataJson">Action data as JSON string</param>
        /// <param name="callback">Response callback with SubmitActionResponse result</param>
        /// <example>
        /// <code>
        /// sdk.SubmitAction("player_ready", "{\"ready\":true,\"timestamp\":\"2023-01-01T12:00:00Z\"}", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Action submitted: {Response.action_id}");
        ///     } else {
        ///         Debug.LogError($"Failed to submit action: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
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

        /// <summary>
        /// Polls for completed actions from other players
        /// Retrieves processed actions for current player
        /// </summary>
        /// <remarks>
        /// This method retrieves actions that have been processed and are ready for the current player.
        /// Call this periodically to check for completed actions from other players.
        /// The response includes action data and completion status.
        /// </remarks>
        /// <param name="callback">Response callback with PollActionsResponse result</param>
        /// <example>
        /// <code>
        /// // Call every 5 seconds while in room
        /// InvokeRepeating(nameof(PollActions), 5f);
        /// 
        /// sdk.PollActions((Response) => {
        ///     if (Response.success) {
        ///         foreach (var action in Response.actions) {
        ///             Debug.Log($"Completed action: {action.action_type} from {action.action_id}");
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to poll actions: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void PollActions(Action<PollActionsResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/actions/poll", "GET", "", callback);
        }

        /// <summary>
        /// Retrieves pending actions awaiting processing
        /// Returns actions that need to be completed
        /// </summary>
        /// <remarks>
        /// This method retrieves actions that other players have submitted and await processing.
        /// These actions need to be completed using CompleteAction method.
        /// Call this periodically to check for new actions to process.
        /// </remarks>
        /// <param name="callback">Response callback with GetPendingActionsResponse result</param>
        /// <example>
        /// <code>
        /// // Call every 5 seconds while in room
        /// InvokeRepeating(nameof(GetPendingActions), 5f);
        /// 
        /// sdk.GetPendingActions((Response) => {
        ///     if (Response.success) {
        ///         foreach (var action in Response.actions) {
        ///             Debug.Log($"Pending action: {action.action_type} from {action.player_name}");
        ///             // Process the action
        ///             sdk.CompleteAction(action.action_id, "completed", "{\"result\":\"success\"}", (completeResponse) => {
        ///                 // Handle completion
        ///             });
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to get pending actions: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void GetPendingActions(Action<GetPendingActionsResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/game_room.php/actions/pending", "GET", "", callback);
        }

        /// <summary>
        /// Marks an action as completed with response data
        /// Used to process and respond to player actions
        /// </summary>
        /// <remarks>
        /// This method completes a pending action with processing results.
        /// Call this after processing an action from GetPendingActions.
        /// The response data should contain the result of the action processing.
        /// </remarks>
        /// <param name="actionId">Unique action identifier to complete</param>
        /// <param name="status">Completion status ("completed", "failed", "error", etc.)</param>
        /// <param name="responseDataJson">Response data as JSON string with processing results</param>
        /// <param name="callback">Response callback with CompleteActionResponse result</param>
        /// <example>
        /// <code>
        /// sdk.CompleteAction("action123", "completed", "{\"result\":\"success\",\"processed_at\":\"2023-01-01T12:00:00Z\"}", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log("Action completed successfully");
        ///     } else {
        ///         Debug.LogError($"Failed to complete action: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
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

        /// <summary>
        /// Sends updates to specific players or all players in room
        /// Used for real-time data synchronization
        /// </summary>
        /// <remarks>
        /// This method sends updates to specific players or all players in the room.
        /// Useful for real-time game state synchronization, notifications, or events.
        /// Target players can be "all" or a JSON array of player IDs.
        /// The response confirms delivery and provides update tracking.
        /// </remarks>
        /// <param name="targetPlayerIds">Target players ("all" or JSON array string)</param>
        /// <param name="type">Update type identifier (e.g., "game_state", "player_action")</param>
        /// <param name="dataJson">Update data as JSON string</param>
        /// <param name="callback">Response callback with SendUpdateResponse result</param>
        /// <example>
        /// <code>
        /// // Send to all players
        /// sdk.SendUpdate("all", "game_state", "{\"phase\":\"started\",\"timer\":300}", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Update sent to {Response.updates_sent} players");
        ///     } else {
        ///         Debug.LogError($"Failed to send update: {Response.error}");
        ///     }
        /// });
        /// 
        /// // Send to specific players
        /// sdk.SendUpdate("[\"123\",\"456\"]", "player_action", "{\"action\":\"move\",\"position\":{\"x\":10,\"y\":5}}", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Update sent to specific players");
        ///     }
        /// });
        /// </code>
        /// </example>
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

        /// <summary>
        /// Polls for updates from other players in room
        /// Retrieves new updates since last poll
        /// </summary>
        /// <remarks>
        /// This method retrieves updates sent by other players in the room.
        /// Call this periodically to check for new updates and game state changes.
        /// The optional lastUpdateId parameter enables incremental polling.
        /// The response includes update data and sender information.
        /// </remarks>
        /// <param name="lastUpdateId">Last update ID for incremental polling (null for all)</param>
        /// <param name="callback">Response callback with PollUpdatesResponse result</param>
        /// <example>
        /// <code>
        /// // Call every 5 seconds while in room
        /// InvokeRepeating(nameof(PollUpdates), 5f);
        /// 
        /// string lastUpdateId = null;
        /// 
        /// sdk.PollUpdates(lastUpdateId, (Response) => {
        ///     if (Response.success) {
        ///         foreach (var update in Response.updates) {
        ///             Debug.Log($"Update from {update.from_player_id}: {update.type}");
        ///             // Process update data
        ///             var updateData = sdk.DeserializeFromJson&lt;UpdateData&gt;(update.data_json);
        ///         }
        ///         // Store last update ID for incremental polling
        ///         lastUpdateId = Response.last_update_id;
        ///     } else {
        ///         Debug.LogError($"Failed to poll updates: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
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

        /// <summary>
        /// Retrieves list of all available matchmaking lobbies
        /// Returns lobbies that are not full and not started
        /// </summary>
        /// <remarks>
        /// This method returns all matchmaking lobbies that are currently available.
        /// Useful for a lobby browser or server discovery system.
        /// The response includes lobby details like player count and settings.
        /// </remarks>
        /// <param name="callback">Response callback with ListMatchmakingResponse result</param>
        /// <example>
        /// <code>
        /// sdk.ListMatchmaking((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Found {Response.lobbies.Length} lobbies");
        ///         foreach (var lobby in Response.lobbies) {
        ///             Debug.Log($"Lobby: {lobby.host_name} ({lobby.current_players}/{lobby.max_players})");
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to list lobbies: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void ListMatchmaking(Action<ListMatchmakingResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/list", "GET", "", callback);
        }

        /// <summary>
        /// Creates a new matchmaking lobby
        /// Configures lobby settings and makes creating player the host
        /// </summary>
        /// <remarks>
        /// This method creates a new matchmaking lobby with the calling player as host.
        /// The lobby settings include player capacity, fullness behavior, and join requirements.
        /// The response confirms lobby creation and host status.
        /// </remarks>
        /// <param name="maxPlayers">Maximum player capacity (2-16 recommended)</param>
        /// <param name="strictFull">Whether lobby closes automatically when full</param>
        /// <param name="joinByRequests">Whether players must request to join (true) or can join directly (false)</param>
        /// <param name="extraJsonString">Additional lobby settings as JSON string (e.g., {"minLevel":5,"rank":"silver"})</param>
        /// <param name="callback">Response callback with CreateMatchmakingResponse result</param>
        /// <example>
        /// <code>
        /// sdk.CreateMatchmaking(4, true, false, "{\"minLevel\":5,\"rank\":\"silver\",\"gameMode\":\"competitive\"}", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Lobby created: {Response.matchmaking_id}");
        ///         Debug.Log($"You are host: {Response.is_host}");
        ///     } else {
        ///         Debug.LogError($"Failed to create lobby: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
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

        /// <summary>
        /// Requests to join a matchmaking lobby
        /// Sends join request to lobby host for approval
        /// </summary>
        /// <remarks>
        /// This method sends a join request to a lobby that requires approval.
        /// The lobby host must approve or reject the request.
        /// Use this for lobbies with joinByRequests set to true.
        /// The response returns a request ID for status tracking.
        /// </remarks>
        /// <param name="matchmakingId">Unique lobby identifier</param>
        /// <param name="callback">Response callback with JoinRequestResponse result</param>
        /// <example>
        /// <code>
        /// sdk.RequestJoinMatchmaking("lobby123", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Join request sent: {Response.request_id}");
        ///         Debug.Log("Waiting for host approval...");
        ///     } else {
        ///         Debug.LogError($"Failed to request join: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void RequestJoinMatchmaking(string matchmakingId, Action<JoinRequestResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/{matchmakingId}/request", "POST", "", callback);
        }

        /// <summary>
        /// Joins a matchmaking lobby directly
        /// Bypasses approval process (if allowed by lobby settings)
        /// </summary>
        /// <remarks>
        /// This method attempts to join a lobby directly without host approval.
        /// Only works if the lobby has joinByRequests set to false.
        /// The response confirms successful lobby entry.
        /// </remarks>
        /// <param name="matchmakingId">Unique lobby identifier</param>
        /// <param name="callback">Response callback with JoinMatchmakingResponse result</param>
        /// <example>
        /// <code>
        /// sdk.JoinMatchmaking("lobby123", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Successfully joined lobby: {Response.matchmaking_id}");
        ///     } else {
        ///         Debug.LogError($"Failed to join lobby: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void JoinMatchmaking(string matchmakingId, Action<JoinMatchmakingResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/{matchmakingId}/join", "POST", "", callback);
        }

        /// <summary>
        /// Leaves current matchmaking lobby
        /// Exits lobby and updates player status
        /// </summary>
        /// <remarks>
        /// This method removes the authenticated player from their current matchmaking lobby.
        /// Call this when player wants to leave a lobby or cancel matchmaking.
        /// The response confirms successful lobby exit.
        /// </remarks>
        /// <param name="callback">Response callback with LeaveMatchmakingResponse result</param>
        /// <example>
        /// <code>
        /// sdk.LeaveMatchmaking((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log("Successfully left matchmaking lobby");
        ///     } else {
        ///         Debug.LogError($"Failed to leave lobby: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void LeaveMatchmaking(Action<LeaveMatchmakingResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/leave", "POST", "", callback);
        }

        /// <summary>
        /// Retrieves list of players in current matchmaking lobby
        /// Returns all players in the caller's lobby
        /// </summary>
        /// <remarks>
        /// This method retrieves all players currently in the authenticated player's matchmaking lobby.
        /// Useful for displaying player lists, checking lobby occupancy, or managing
        /// lobby state. The response includes player status and activity.
        /// </remarks>
        /// <param name="callback">Response callback with GetMatchmakingPlayersResponse result</param>
        /// <example>
        /// <code>
        /// sdk.GetMatchmakingPlayers((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Found {Response.players.Length} players in lobby");
        ///         foreach (var player in Response.players) {
        ///             Debug.Log($"Player: {player.player_name} (Host: {player.is_host})");
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to get lobby players: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void GetMatchmakingPlayers(Action<GetMatchmakingPlayersResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/players", "GET", "", callback);
        }

        /// <summary>
        /// Sends heartbeat to maintain matchmaking connection
        /// Should be called periodically while in lobby
        /// </summary>
        /// <remarks>
        /// Call this method every 30-60 seconds while player is in a matchmaking lobby.
        /// This prevents automatic timeout and maintains player's lobby connection.
        /// The response confirms heartbeat was received.
        /// </remarks>
        /// <param name="callback">Response callback with BaseResponse result</param>
        /// <example>
        /// <code>
        /// // Call every 30 seconds while in lobby
        /// InvokeRepeating(nameof(SendMatchmakingHeartbeat), 30f);
        /// 
        /// sdk.SendMatchmakingHeartbeat((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log("Matchmaking heartbeat sent successfully");
        ///     } else {
        ///         Debug.LogError($"Matchmaking heartbeat failed: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void SendMatchmakingHeartbeat(Action<BaseResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/heartbeat", "POST", "", callback);
        }

        /// <summary>
        /// Removes matchmaking lobby (host only)
        /// Deletes lobby and removes all players
        /// </summary>
        /// <remarks>
        /// This method removes the matchmaking lobby and kicks all players.
        /// Only the lobby host can call this method.
        /// Use this when canceling matchmaking or cleaning up empty lobbies.
        /// The response confirms successful lobby removal.
        /// </remarks>
        /// <param name="callback">Response callback with BaseResponse result</param>
        /// <example>
        /// <code>
        /// sdk.RemoveMatchmaking((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log("Matchmaking lobby removed successfully");
        ///     } else {
        ///         Debug.LogError($"Failed to remove lobby: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void RemoveMatchmaking(Action<BaseResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/remove", "POST", "", callback);
        }

        /// <summary>
        /// Starts game from matchmaking lobby
        /// Transfers players from lobby to game room
        /// </summary>
        /// <remarks>
        /// This method starts a game from the matchmaking lobby.
        /// All players in the lobby are transferred to a new game room.
        /// Only the lobby host can call this method.
        /// The response confirms game start and provides room details.
        /// </remarks>
        /// <param name="callback">Response callback with StartMatchmakingResponse result</param>
        /// <example>
        /// <code>
        /// sdk.StartMatchmaking((Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Game started! Room: {Response.room_name}");
        ///         Debug.Log($"Players transferred: {Response.players_transferred}");
        ///     } else {
        ///         Debug.LogError($"Failed to start game: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void StartMatchmaking(Action<StartMatchmakingResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/start", "POST", "", callback);
        }

        /// <summary>
        /// Retrieves current matchmaking status and lobby information
        /// Returns complete lobby state and player context
        /// </summary>
        /// <remarks>
        /// This method retrieves comprehensive information about the player's current matchmaking lobby.
        /// Includes lobby details, player status, and pending join requests.
        /// Useful for checking lobby state before performing actions.
        /// </remarks>
        /// <param name="callback">Response callback with CurrentMatchmakingStatusResponse result</param>
        /// <example>
        /// <code>
        /// sdk.GetCurrentMatchmakingStatus((Response) => {
        ///     if (Response.success) {
        ///         if (Response.in_matchmaking) {
        ///             Debug.Log($"In lobby: {Response.matchmaking.matchmaking_id}");
        ///             Debug.Log($"Player count: {Response.matchmaking.current_players}/{Response.matchmaking.max_players}");
        ///         } else {
        ///             Debug.Log("Not in any matchmaking lobby");
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to get matchmaking status: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void GetCurrentMatchmakingStatus(Action<CurrentMatchmakingStatusResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/current", "GET", "", callback);
        }

        /// <summary>
        /// Checks status of a join request
        /// Returns request status and processing information
        /// </summary>
        /// <remarks>
        /// This method retrieves the status of a specific join request.
        /// Useful for tracking whether a join request was approved or rejected.
        /// The response includes detailed request information and processing status.
        /// </remarks>
        /// <param name="requestId">Unique request identifier</param>
        /// <param name="callback">Response callback with CheckRequestStatusResponse result</param>
        /// <example>
        /// <code>
        /// sdk.CheckRequestStatus("request123", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Request status: {Response.request.status}");
        ///         Debug.Log($"Responded by: {Response.request.responder_name}");
        ///     } else {
        ///         Debug.LogError($"Failed to check request status: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void CheckRequestStatus(string requestId, Action<CheckRequestStatusResponse> callback)
        {
            SendRequest($"{UnityEndpoint}/matchmaking.php/{requestId}/status", "GET", "", callback);
        }

        /// <summary>
        /// Responds to a join request (host function)
        /// Approves or rejects player join requests
        /// </summary>
        /// <remarks>
        /// This method allows the lobby host to approve or reject join requests.
        /// Only the lobby host can call this method.
        /// Use this to manage player access to your lobby.
        /// The response confirms the request was processed.
        /// </remarks>
        /// <param name="requestId">Unique request identifier</param>
        /// <param name="action">Response action ("approve" or "reject")</param>
        /// <param name="callback">Response callback with RespondToRequestResponse result</param>
        /// <example>
        /// <code>
        /// // Approve a request
        /// sdk.RespondToRequest("request123", "approve", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log("Join request approved");
        ///     } else {
        ///         Debug.LogError($"Failed to approve request: {Response.error}");
        ///     }
        /// });
        /// 
        /// // Reject a request
        /// sdk.RespondToRequest("request456", "reject", (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log("Join request rejected");
        ///     } else {
        ///         Debug.LogError($"Failed to reject request: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void RespondToRequest(string requestId, string action, Action<RespondToRequestResponse> callback)
        {
            var request = new RespondToRequestRequest { action = action };
            var json = JsonUtility.ToJson(request);
            SendRequest($"{UnityEndpoint}/matchmaking.php/{requestId}/response", "POST", json, callback);
        }

        #endregion

        #region Leaderboard

        /// <summary>
        /// Retrieves leaderboard with configurable sorting
        /// Returns ranked players based on specified criteria
        /// </summary>
        /// <remarks>
        /// This method retrieves a ranked list of players based on specified criteria.
        /// Useful for displaying leaderboards, rankings, or competitive stats.
        /// The response includes player ranks and sorting information.
        /// </remarks>
        /// <param name="sortBy">Array of fields to sort by (e.g., ["level", "score"])</param>
        /// <param name="limit">Maximum number of results to return</param>
        /// <param name="callback">Response callback with LeaderboardResponse result</param>
        /// <example>
        /// <code>
        /// sdk.GetLeaderboard(new string[] { "level", "score" }, 10, (Response) => {
        ///     if (Response.success) {
        ///         Debug.Log($"Retrieved {Response.leaderboard.Length} ranked players");
        ///         foreach (var entry in Response.leaderboard) {
        ///             Debug.Log($"#{entry.rank} - {entry.player_name} (Level: {entry.player_data_json})");
        ///         }
        ///     } else {
        ///         Debug.LogError($"Failed to get leaderboard: {Response.error}");
        ///     }
        /// });
        /// </code>
        /// </example>
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

        /// <summary>
        /// Sends HTTP request to API server with authentication and error handling
        /// Handles all communication with multiplayer API
        /// </summary>
        /// <remarks>
        /// This method handles all HTTP communication with the multiplayer API.
        /// Includes automatic authentication headers, timeout handling, and comprehensive error management.
        /// Uses coroutines for async operations and proper response parsing.
        /// </remarks>
        /// <typeparam name="T">Expected response type (must inherit from BaseResponse)</typeparam>
        /// <param name="url">API endpoint URL</param>
        /// <param name="method">HTTP method (GET, POST, PUT)</param>
        /// <param name="bodyJson">Request body as JSON string (empty for GET)</param>
        /// <param name="callback">Response callback with typed result</param>
        /// <example>
        /// <code>
        /// // This method is called internally by all SDK methods
        /// // Example usage:
        /// SendRequest&lt;LoginResponse&gt;("/login", "POST", json, (response) => {
        ///     // Handle response
        /// });
        /// </code>
        /// </example>
        private void SendRequest<T>(string url, string method, string bodyJson, Action<T> callback, bool usePrivateToken = false) where T : BaseResponse
        {
            StartCoroutine(SendRequestCoroutine(BuildUrlWithAuth(url, usePrivateToken), method, bodyJson, callback));
        }

        /// <summary>
        /// Builds URL with authentication query parameters
        /// Matches .NET SDK authentication pattern
        /// </summary>
        /// <param name="url">Base URL</param>
        /// <param name="usePrivateToken">Whether to use private token for admin operations</param>
        /// <returns>URL with authentication parameters</returns>
        private string BuildUrlWithAuth(string url, bool usePrivateToken = false)
        {
            string separator = url.Contains("?") ? "&" : "?";
            
            // Add api_token parameter
            if (!string.IsNullOrEmpty(apiToken))
            {
                url += $"{separator}api_token={UnityWebRequest.EscapeURL(apiToken)}";
                separator = "&";
            }
            
            // Add game_player_token parameter
            if (!string.IsNullOrEmpty(gamePlayerToken))
            {
                url += $"{separator}game_player_token={UnityWebRequest.EscapeURL(gamePlayerToken)}";
                separator = "&";
            }
            
            // Add api_private_token parameter for admin operations
            if (usePrivateToken && !string.IsNullOrEmpty(apiPrivateToken))
            {
                url += $"{separator}api_private_token={UnityWebRequest.EscapeURL(apiPrivateToken)}";
            }
            
            return url;
        }

        /// <summary>
        /// Coroutine for async HTTP requests with comprehensive error handling
        /// Manages UnityWebRequest with proper authentication and response processing
        /// </summary>
        /// <remarks>
        /// This coroutine manages the complete HTTP request lifecycle.
        /// Includes timeout configuration, authentication headers, request body handling,
        /// debug logging, JSON parsing, and error response creation.
        /// Designed for production use with robust error handling.
        /// </remarks>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <param name="url">API endpoint URL</param>
        /// <param name="method">HTTP method</param>
        /// <param name="bodyJson">Request body as JSON string</param>
        /// <param name="callback">Response callback</param>
        /// <returns>IEnumerator for Unity coroutine</returns>
        /// <example>
        /// <code>
        /// // This method is called internally by SendRequest
        /// // Handles UnityWebRequest lifecycle:
        /// // 1. Configure timeout and headers
        /// // 2. Add authentication tokens
        /// // 3. Set request body
        /// // 4. Send request and wait for response
        /// // 5. Parse JSON response or create error response
        /// </code>
        /// </example>
        private IEnumerator SendRequestCoroutine<T>(string url, string method, string bodyJson, Action<T> callback) where T : BaseResponse
        {
            using (UnityWebRequest request = new UnityWebRequest(url, method))
            {
                request.timeout = (int)requestTimeout;

                // Set headers
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accept", "application/json");

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

        /// <summary>
        /// Sets API authentication token
        /// Must be called before making authenticated requests
        /// </summary>
        /// <remarks>
        /// Call this method to set the API token for authentication.
        /// The token is required for all API calls and should be stored
        /// securely. Use this method after obtaining a valid API token.
        /// </remarks>
        /// <param name="token">Valid API token from multiplayer service</param>
        /// <example>
        /// <code>
        /// // Set API token (typically done once at startup)
        /// sdk.SetApiToken("your_api_token_here");
        /// 
        /// // Now all subsequent API calls will be authenticated
        /// sdk.LoginPlayer((response) => {
        ///     // This will work with the set token
        /// });
        /// </code>
        /// </example>
        public void SetApiToken(string token)
        {
            apiToken = token;
        }

        /// <summary>
        /// Sets player authentication token
        /// Required for player-specific API calls
        /// </summary>
        /// <remarks>
        /// Call this method to set the player token after successful login.
        /// The player token is required for player-specific operations like
        /// room management, matchmaking, and data updates.
        /// Store this token securely for subsequent API calls.
        /// </remarks>
        /// <param name="token">Player token from login response</param>
        /// <example>
        /// <code>
        /// // After successful login, store the player token
        /// sdk.LoginPlayer((loginResponse) => {
        ///     if (loginResponse.success) {
        ///         // Store player token for future calls
        ///         sdk.SetGamePlayerToken(loginResponse.player.private_key);
        ///         Debug.Log("Player token set successfully");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void SetGamePlayerToken(string token)
        {
            gamePlayerToken = token;
        }

        /// <summary>
        /// Sets the private API token for admin operations
        /// Required for admin-only operations like listing all players
        /// </summary>
        /// <remarks>
        /// This method sets the private API token used for privileged operations.
        /// The private token should be kept secure and only used server-side
        /// or in trusted admin interfaces. This token is required for
        /// operations like listing all players or updating global game data.
        /// </remarks>
        /// <param name="token">Private API token for admin operations</param>
        /// <example>
        /// <code>
        /// // Set private API token for admin operations
        /// sdk.SetApiPrivateToken("your_private_api_token_here");
        /// 
        /// // Now admin operations will work
        /// sdk.ListPlayers((response) => {
        ///     if (response.success) {
        ///         Debug.Log($"Found {response.count} players");
        ///     }
        /// });
        /// </code>
        /// </example>
        public void SetApiPrivateToken(string token)
        {
            apiPrivateToken = token;
        }

        /// <summary>
        /// Serializes object to JSON using Unity's JsonUtility
        /// Helper method for consistent JSON serialization
        /// </summary>
        /// <remarks>
        /// This method serializes any object to a JSON string using Unity's JsonUtility.
        /// Use this method to ensure Unity compatibility with all JSON operations.
        /// The serialization follows Unity's requirements (no Dictionary, fields only).
        /// </remarks>
        /// <typeparam name="T">Object type to serialize</typeparam>
        /// <param name="obj">Object to serialize to JSON</param>
        /// <returns>JSON string representation of the object</returns>
        /// <example>
        /// <code>
        /// // Serialize player data
        /// var playerData = new { level = 5, score = 1000 };
        /// string json = sdk.SerializeToJson(playerData);
        /// Debug.Log($"Serialized: {json}");
        /// 
        /// // Use with API calls
        /// sdk.UpdatePlayerData(json, (response) => {
        ///     // Handle response
        /// });
        /// </code>
        /// </example>
        public string SerializeToJson<T>(T obj)
        {
            return JsonUtility.ToJson(obj);
        }

        /// <summary>
        /// Deserializes JSON to object using Unity's JsonUtility
        /// Helper method for consistent JSON deserialization
        /// </summary>
        /// <remarks>
        /// This method deserializes a JSON string to an object using Unity's JsonUtility.
        /// Use this method to parse JSON responses from the API.
        /// The deserialization follows Unity's requirements (no Dictionary, fields only).
        /// </remarks>
        /// <typeparam name="T">Target object type for deserialization</typeparam>
        /// <param name="json">JSON string to deserialize to object</param>
        /// <returns>Deserialized object of type T</returns>
        /// <example>
        /// <code>
        /// // Parse player data from API response
        /// sdk.GetPlayerData((response) => {
        ///     if (response.success) {
        ///         var playerData = sdk.DeserializeFromJson&lt;PlayerData&gt;(response.data_json);
        ///         Debug.Log($"Parsed level: {playerData.level}");
        ///     }
        /// });
        /// </code>
        /// </example>
        public T DeserializeFromJson<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        #endregion
    }
}
