using System;
using System.Threading.Tasks;
using System.Text.Json;
using michitai;

public class Game
{
    private static GameSDK? sdk;

    public static async Task Main()
    {
        Console.WriteLine("=== MICHITAI Game SDK Usage Example ===\n");

        // 1️⃣ Initialize SDK
        sdk = new GameSDK("YOUR_API_TOKEN", "YOUR_API_PRIVATE_TOKEN");
        //sdk = new GameSDK("YOUR_API_TOKEN", "YOUR_API_PRIVATE_TOKEN", logger:new ConsoleLogger()); // for console logging
        //sdk = new GameSDK("YOUR_API_TOKEN", "YOUR_API_PRIVATE_TOKEN", logger:new UnityLogger()); // for unity logging
        Console.WriteLine("[INIT] SDK initialized\n");

        // 2️⃣ Register Player
        Console.WriteLine("[PLAYER] Registering new player...");
        var reg = await sdk.RegisterPlayer("TestPlayer", new
        {
            level = 1,
            score = 0,
            inventory = new[] { "sword", "shield" }
        });

        string playerToken = reg.Private_key;
        int playerId = int.Parse(reg.Player_id);

        Console.WriteLine($"[PLAYER] Registered: ID={playerId}, Token={playerToken}\n");

        // 3️⃣ Authenticate Player
        Console.WriteLine("[PLAYER] Authenticating player...");
        var auth = await sdk.AuthenticatePlayer(playerToken);

        if (auth.Success)
        {
            var pdata = auth.Player.Player_data;
            int level = pdata.ContainsKey("level") ? ((JsonElement)pdata["level"]).GetInt32() : 0;

            Console.WriteLine($"[PLAYER] Authenticated: {auth.Player.Player_name} (Level={level})\n");
        }
        else
        {
            Console.WriteLine("[PLAYER] Authentication failed\n");
        }

        // 4️⃣ List all players
        Console.WriteLine("[ADMIN] Fetching all players...");
        var allPlayers = await sdk.GetAllPlayers();
        Console.WriteLine($"[ADMIN] Total players: {allPlayers.Count}");
        foreach (var p in allPlayers.Players)
        {
            Console.WriteLine($" - ID={p.Id}, Name={p.Player_name}, Active={p.Is_active}");
        }
        Console.WriteLine();

        // 5️⃣ Get global game data
        Console.WriteLine("[GAME] Loading game data...");
        var gameData = await sdk.GetGameData();
        Console.WriteLine($"[GAME] Game ID={gameData.Game_id}, Settings={gameData.Data.Count}\n");

        // 6️⃣ Update global game data
        Console.WriteLine("[GAME] Updating game settings...");
        var updateGame = await sdk.UpdateGameData(new
        {
            game_settings = new { difficulty = "hard", max_players = 10 },
            last_updated = DateTime.UtcNow.ToString("o")
        });
        Console.WriteLine($"[GAME] {updateGame.Message} at {updateGame.Updated_at}\n");

        // 7️⃣ Get player-specific data
        Console.WriteLine("[PLAYER] Loading player data...");
        var playerData = await sdk.GetPlayerData(playerToken);

        var pDataDict = playerData.Data;
        int playerLevel = pDataDict.ContainsKey("level") ? ((JsonElement)pDataDict["level"]).GetInt32() : 0;
        int playerScore = pDataDict.ContainsKey("score") ? ((JsonElement)pDataDict["score"]).GetInt32() : 0;
        string[] inventory = pDataDict.ContainsKey("inventory")
            ? JsonSerializer.Deserialize<string[]>(((JsonElement)pDataDict["inventory"]).GetRawText())!
            : new string[0];

        Console.WriteLine($"[PLAYER] Level={playerLevel}, Score={playerScore}, Inventory=[{string.Join(", ", inventory)}]\n");

        // 8️⃣ Update player data
        Console.WriteLine("[PLAYER] Updating player progress...");
        var updatedPlayer = await sdk.UpdatePlayerData(playerToken, new
        {
            level = 2,
            score = 100,
            inventory = new[] { "sword", "shield", "potion" },
            last_played = DateTime.UtcNow.ToString("o")
        });
        Console.WriteLine($"[PLAYER] {updatedPlayer.Message} at {updatedPlayer.Updated_at}\n");

        // 9️⃣ Get server time
        Console.WriteLine("[SERVER] Getting server time...");
        var serverTime = await sdk.GetServerTime();
        if (serverTime.Success)
        {
            Console.WriteLine($"[SERVER] Server time (UTC): {serverTime.Utc}");
            Console.WriteLine($"[SERVER] Timestamp: {serverTime.Timestamp}");
            Console.WriteLine($"[SERVER] Readable: {serverTime.Readable}\n");
        }
        else
        {
            Console.WriteLine("[SERVER] Failed to get server time\n");
        }

        // 🔟 Create a new room
        Console.WriteLine("[ROOM] Creating a new game room...");
        var roomCreate = await sdk.CreateRoomAsync(
            playerToken,
            "Test Room",
            "test123",
            4
        );
        string roomId = roomCreate.Room_id;
        Console.WriteLine($"[ROOM] Created room: ID={roomId}, Name={roomCreate.Room_name}, Is Host={roomCreate.Is_host}\n");

        // 1️⃣1️⃣ List all available rooms
        Console.WriteLine("[ROOM] Fetching available rooms...");
        var rooms = await sdk.GetRoomsAsync();
        Console.WriteLine($"[ROOM] Found {rooms.Rooms.Count} room(s):");
        foreach (var room in rooms.Rooms)
        {
            Console.WriteLine($" - ID: {room.Room_id}, Name: {room.Room_name}, Players: {room.Current_players}/{room.Max_players}");
        }
        Console.WriteLine();

        // 1️⃣2️⃣ Join the room (as another player would)
        Console.WriteLine("[ROOM] Joining the room...");
        var joinRoom = await sdk.JoinRoomAsync(
            playerToken,
            roomId,
            "test123"
        );
        Console.WriteLine($"[ROOM] {joinRoom.Message}\n");

        // 1️⃣3️⃣ List players in the room
        Console.WriteLine("[ROOM] Fetching room players...");
        var roomPlayers = await sdk.GetRoomPlayersAsync(playerToken);
        Console.WriteLine($"[ROOM] Players in room ({roomPlayers.Players.Count}):");
        foreach (var player in roomPlayers.Players)
        {
            Console.WriteLine($" - {player.Player_name} (ID: {player.Player_id}, Host: {player.Is_host == 1}, Online: {player.Is_online}, Last Heartbeat: {player.Last_heartbeat})");
        }
        Console.WriteLine();

        // 1️⃣4️⃣ Send a heartbeat
        Console.WriteLine("[ROOM] Sending heartbeat...");
        var heartbeat = await sdk.SendHeartbeatAsync(playerToken);
        Console.WriteLine($"[ROOM] Heartbeat status: {heartbeat.Status}\n");

        // 1️⃣5️⃣ Submit an action
        Console.WriteLine("[ACTION] Submitting move action...");
        var action = await sdk.SubmitActionAsync(
            playerToken,
            "move",
            new { x = 10, y = 20 }
        );
        string actionId = action.Action_id;
        Console.WriteLine($"[ACTION] Action submitted: ID={actionId}, Status={action.Status}\n");

        // 1️⃣6️⃣ Check pending actions
        Console.WriteLine("[ACTION] Checking for pending actions...");
        var pendingActions = await sdk.GetPendingActionsAsync(playerToken);
        if (pendingActions.Actions != null && pendingActions.Actions.Count > 0)
        {
            foreach (var pendingAction in pendingActions.Actions)
            {
                string requestDataStr = pendingAction.Request_data != null ?
                    JsonSerializer.Serialize(pendingAction.Request_data) : "null";
                Console.WriteLine($"[ACTION] Pending: {pendingAction.Action_type}, " +
                               $"Request: {requestDataStr}");
            }
        }
        else
        {
            Console.WriteLine("[ACTION] No pending actions found\n");
        }

        // 1️⃣7️⃣ Complete an action (simulating server response)
        if (!string.IsNullOrEmpty(actionId))
        {
            Console.WriteLine($"[ACTION] Completing action {actionId}...");
            var completeAction = await sdk.CompleteActionAsync(
                actionId,
                playerToken,
                new ActionCompleteRequest(ActionStatus.Completed,
                new { success = true, message = "Moved successfully" })
            );
            Console.WriteLine($"[ACTION] {completeAction.Message}\n");
        }

        // 1️⃣8️⃣ Poll for completed actions
        Console.WriteLine("[ACTION] Polling for completed actions...");
        var completedActions = await sdk.PollActionsAsync(playerToken);
        if (completedActions.Actions != null && completedActions.Actions.Count > 0)
        {
            foreach (var completedAction in completedActions.Actions)
            {
                string responseDataStr = completedAction.Response_data != null ?
                    JsonSerializer.Serialize(completedAction.Response_data) : "null";
                Console.WriteLine($"[ACTION] Completed: {completedAction.Action_type}, " +
                               $"Result: {responseDataStr}");
            }
        }
        else
        {
            Console.WriteLine("[ACTION] No completed actions found\n");
        }

        // 1️⃣9️⃣ Get current room information
        Console.WriteLine("[ROOM] Getting current room info...");
        var currentRoom = await sdk.GetCurrentRoomAsync(playerToken);
        if (currentRoom.Success && currentRoom.In_room && currentRoom.Room != null)
        {
            var room = currentRoom.Room;
            Console.WriteLine($"[ROOM] Current Room: {room.Room_name} (ID: {room.Room_id})");
            Console.WriteLine($"[ROOM] Players: {room.Current_players}/{room.Max_players}, Host: {room.Is_host}");
            Console.WriteLine($"[ROOM] Joined: {room.Joined_at}, Last Activity: {room.Room_last_activity}\n");
        }
        else
        {
            Console.WriteLine("[ROOM] Not currently in any room\n");
        }

        // 2️⃣0️⃣ Send update to all players
        Console.WriteLine("[UPDATE] Sending update to all players...");
        var updateAll = await sdk.UpdatePlayersAsync(
            playerToken,
            new UpdatePlayersRequest(
                "all",
                "play_animation",
                new { animation = "victory", duration = 2.0 }
            )
        );
        Console.WriteLine($"[UPDATE] Sent to {updateAll.Updates_sent} players, Update IDs: [{string.Join(", ", updateAll.Update_ids)}]\n");

        // 2️⃣1️⃣ Send update to specific player
        Console.WriteLine("[UPDATE] Sending update to specific player...");
        var updateSpecific = await sdk.UpdatePlayersAsync(
            playerToken,
            new UpdatePlayersRequest(
                new string[] { $"{roomPlayers.Players[0].Player_id}" },
                "spawn_effect",
                new { effect = "explosion", position = new { x = 10, y = 20 } }
            )
        );
        Console.WriteLine($"[UPDATE] Sent to {updateSpecific.Updates_sent} players, Target: [{string.Join(", ", updateSpecific.Target_players)}]\n");

        // 2️⃣2️⃣ Poll for player updates
        Console.WriteLine("[UPDATE] Polling for player updates...");
        var pollUpdates = await sdk.PollUpdatesAsync(playerToken);
        if (pollUpdates.Updates.Count > 0)
        {
            Console.WriteLine($"[UPDATE] Found {pollUpdates.Updates.Count} updates (Last ID: {pollUpdates.Last_update_id}):");
            foreach (var update in pollUpdates.Updates)
            {
                string dataStr = JsonSerializer.Serialize(update.Data_json);
                Console.WriteLine($" - From {update.From_player_id}: {update.Type} -> {dataStr} at {update.Created_at}");
            }
        }
        else
        {
            Console.WriteLine("[UPDATE] No new updates found\n");
        }

        // 2️⃣3️⃣ Poll for updates after specific ID
        if (!string.IsNullOrEmpty(pollUpdates.Last_update_id))
        {
            Console.WriteLine("[UPDATE] Polling for updates after last ID...");
            var pollAfter = await sdk.PollUpdatesAsync(playerToken, pollUpdates.Last_update_id);
            Console.WriteLine($"[UPDATE] Found {pollAfter.Updates.Count} new updates since last poll\n");
        }

        // 2️⃣4️⃣ Leave the room
        Console.WriteLine("[ROOM] Leaving the room...");
        var leaveRoom = await sdk.LeaveRoomAsync(playerToken);
        Console.WriteLine($"[ROOM] {leaveRoom.Message}\n");

        Console.WriteLine("=== Demo Complete ===");
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

#if UNITY_EDITOR
    public class UnityLogger : ILogger
    {
        public virtual void Error(string message)
        {
            Debug.Log($"[Error] {message}");
        }

        public virtual void Log(string message)
        {
            Debug.LogWarning($"[Log] {message}");
        }

        public virtual void Warn(string message)
        {
            Debug.LogError($"[Warning] {message}");
        }
    }
#endif
}
