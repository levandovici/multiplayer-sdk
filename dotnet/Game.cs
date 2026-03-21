using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using michitai;

public class Game
{
    private static GameSDK? sdk;
    private static Dictionary<string, PlayerInfo> players = new Dictionary<string, PlayerInfo>();

    public static async Task Main()
    {
        Console.WriteLine("=== MICHITAI Game SDK Usage Example ===\n");

        // 1️⃣ Initialize SDK
        sdk = new GameSDK("YOUR_API_TOKEN", "YOUR_API_PRIVATE_TOKEN");
        Console.WriteLine("[INIT] SDK initialized\n");

        // 2️⃣ Register Multiple Players for Matchmaking Demo
        Console.WriteLine("[PLAYERS] Registering multiple players for matchmaking demo...");
        
        // Register Host Player
        var hostReg = await RegisterPlayer("GameHost", new { level = 10, rank = "gold", role = "host" });
        string hostToken = hostReg.Private_key;
        string hostId = hostReg.Player_id;
        Console.WriteLine($"[HOST] Registered: ID={hostId}, Token={hostToken.Substring(0, 8)}...\n");

        // Register Multiple Players
        var player1Reg = await RegisterPlayer("Player1", new { level = 8, rank = "silver", role = "player" });
        var player2Reg = await RegisterPlayer("Player2", new { level = 12, rank = "gold", role = "player" });
        var player3Reg = await RegisterPlayer("Player3", new { level = 6, rank = "bronze", role = "player" });
        
        players["host"] = new PlayerInfo { Id = hostId, Token = hostToken, Name = "GameHost" };
        players["player1"] = new PlayerInfo { Id = player1Reg.Player_id, Token = player1Reg.Private_key, Name = "Player1" };
        players["player2"] = new PlayerInfo { Id = player2Reg.Player_id, Token = player2Reg.Private_key, Name = "Player2" };
        players["player3"] = new PlayerInfo { Id = player3Reg.Player_id, Token = player3Reg.Private_key, Name = "Player3" };

        Console.WriteLine($"[PLAYERS] Total registered: {players.Count} players\n");

        // 3️⃣ Authenticate All Players
        Console.WriteLine("[AUTH] Authenticating all players...");
        foreach (var kvp in players)
        {
            var auth = await sdk.AuthenticatePlayer(kvp.Value.Token);
            if (auth.Success)
            {
                Console.WriteLine($"[AUTH] {kvp.Value.Name} authenticated successfully");
            }
        }
        Console.WriteLine();

        // 4️⃣ List all players (admin view)
        Console.WriteLine("[ADMIN] Fetching all players...");
        var allPlayers = await sdk.GetAllPlayers();
        Console.WriteLine($"[ADMIN] Total players in database: {allPlayers.Count}");
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
            game_settings = new { difficulty = "hard", max_players = 10, matchmaking_enabled = true },
            last_updated = DateTime.UtcNow.ToString("o")
        });
        Console.WriteLine($"[GAME] {updateGame.Message} at {updateGame.Updated_at}\n");

        // 7️⃣ Get player-specific data for host
        Console.WriteLine("[HOST] Loading host player data...");
        var hostData = await sdk.GetPlayerData(hostToken);
        Console.WriteLine($"[HOST] {hostData.Player_name} - Level: {GetJsonValue(hostData.Data, "level")}, Rank: {GetJsonValue(hostData.Data, "rank")}\n");

        // 8️⃣ Update host player data
        Console.WriteLine("[HOST] Updating host progress...");
        var updatedHost = await sdk.UpdatePlayerData(hostToken, new
        {
            level = 15,
            rank = "platinum",
            last_played = DateTime.UtcNow.ToString("o"),
            matchmaking_status = "ready"
        });
        Console.WriteLine($"[HOST] {updatedHost.Message} at {updatedHost.Updated_at}\n");

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

        // 9️⃣1️⃣ Get server time with +1 hour offset
        Console.WriteLine("[SERVER] Getting server time with +1 hour offset...");
        var serverTimePlus1 = await sdk.GetServerTimeWithOffset(1);
        if (serverTimePlus1.Success)
        {
            Console.WriteLine($"[SERVER] Server time (+1h): {serverTimePlus1.Utc}");
            Console.WriteLine($"[SERVER] Timestamp: {serverTimePlus1.Timestamp}");
            Console.WriteLine($"[SERVER] Readable: {serverTimePlus1.Readable}");
            Console.WriteLine($"[SERVER] Offset: {serverTimePlus1.Offset.Offset_hours}h ({serverTimePlus1.Offset.Offset_string})");
            Console.WriteLine($"[SERVER] Original UTC: {serverTimePlus1.Offset.Original_utc}\n");
        }
        else
        {
            Console.WriteLine("[SERVER] Failed to get server time with +1 offset\n");
        }

        // 9️⃣2️⃣ Get server time with -2 hours offset
        Console.WriteLine("[SERVER] Getting server time with -2 hours offset...");
        var serverTimeMinus2 = await sdk.GetServerTimeWithOffset(-2);
        if (serverTimeMinus2.Success)
        {
            Console.WriteLine($"[SERVER] Server time (-2h): {serverTimeMinus2.Utc}");
            Console.WriteLine($"[SERVER] Timestamp: {serverTimeMinus2.Timestamp}");
            Console.WriteLine($"[SERVER] Readable: {serverTimeMinus2.Readable}");
            Console.WriteLine($"[SERVER] Offset: {serverTimeMinus2.Offset.Offset_hours}h ({serverTimeMinus2.Offset.Offset_string})");
            Console.WriteLine($"[SERVER] Original UTC: {serverTimeMinus2.Offset.Original_utc}\n");
        }
        else
        {
            Console.WriteLine("[SERVER] Failed to get server time with -2 offset\n");
        }

        // 🔟 Create a traditional room (for comparison)
        Console.WriteLine("[ROOM] Creating a traditional game room...");
        var roomCreate = await sdk.CreateRoomAsync(
            hostToken,
            "Traditional Room",
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

        // 1️⃣2️⃣ Leave the traditional room (cleanup)
        Console.WriteLine("[ROOM] Leaving the traditional game room...");
        try
        {
            // Send heartbeat before leaving room
            Console.WriteLine("[ROOM] Sending heartbeat before leaving room...");
            var heartbeat = await sdk.SendPlayerHeartbeatAsync(hostToken);
            if (heartbeat.Success)
            {
                Console.WriteLine($"[ROOM] Heartbeat sent: {heartbeat.Message}");
            }
            
            var leaveRoom = await sdk.LeaveRoomAsync(hostToken);
            Console.WriteLine($"[ROOM] {leaveRoom.Message}");
            
            // Logout after leaving room
            Console.WriteLine("[ROOM] Logging out after leaving room...");
            var logout = await sdk.LogoutPlayerAsync(hostToken);
            if (logout.Success)
            {
                Console.WriteLine($"[ROOM] {logout.Message}");
            }
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"[ROOM] Leave error: {ex.ApiError}");
        }

        // ==================== MATCHMAKING DEMO ====================

        // 2️⃣4️⃣ List Matchmaking Lobbies
        Console.WriteLine("[MATCHMAKING] Listing available matchmaking lobbies...");
        var lobbies = await sdk.GetMatchmakingLobbiesAsync();
        Console.WriteLine($"[MATCHMAKING] Found {lobbies.Lobbies.Count} lobby(ies):");
        foreach (var lobby in lobbies.Lobbies)
        {
            Console.WriteLine($" - ID: {lobby.Matchmaking_id}, Host: {lobby.Host_name}, Players: {lobby.Current_players}/{lobby.Max_players}, Strict: {lobby.Strict_full == 1}");
        }
        Console.WriteLine();

        // 2️⃣5️⃣ Create Matchmaking Lobby
        Console.WriteLine("[MATCHMAKING] Host creating new matchmaking lobby...");
        var matchmakingCreate = await sdk.CreateMatchmakingLobbyAsync(
            hostToken,
            maxPlayers: 4,
            strictFull: true,
            joinByRequests: false,
            extraJsonString: new { minLevel = 5, rank = "silver", gameMode = "competitive" }
        );
        string matchmakingId = matchmakingCreate.Matchmaking_id;
        Console.WriteLine($"[MATCHMAKING] Lobby created: ID={matchmakingId}, Max Players={matchmakingCreate.Max_players}, Host={matchmakingCreate.Is_host}");
        Console.WriteLine($"[MATCHMAKING] Settings: Strict Full={matchmakingCreate.Strict_full}, Join by Requests={matchmakingCreate.Join_by_requests}\n");

        // 2️⃣6️⃣ Request to Join Matchmaking (Player 1)
        Console.WriteLine("[MATCHMAKING] Player1 requesting to join matchmaking...");
        var joinRequest = await sdk.RequestToJoinMatchmakingAsync(players["player1"].Token, matchmakingId);
        Console.WriteLine($"[MATCHMAKING] Join request sent: ID={joinRequest.Request_id}, Message={joinRequest.Message}\n");

        // 2️⃣7️⃣ Respond to Join Request (Host approves)
        Console.WriteLine("[MATCHMAKING] Host responding to join request...");
        try
        {
            var approveRequest = await sdk.RespondToJoinRequestAsync(hostToken, matchmakingId, GameSDK.MatchmakingRequestAction.Approve);
            Console.WriteLine($"[MATCHMAKING] Response: {approveRequest.Message}, Action={approveRequest.Action}, Request ID={approveRequest.Request_id}\n");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"[MATCHMAKING] Error responding to request: {ex.ApiError}");
            Console.WriteLine($"[MATCHMAKING] Server response: {ex.ErrorResponse}\n");
        }

        // 2️⃣8️⃣ Check Join Request Status (Player 1)
        Console.WriteLine("[MATCHMAKING] Player1 checking join request status...");
        var requestStatus = await sdk.CheckJoinRequestStatusAsync(players["player1"].Token, joinRequest.Request_id);
        if (requestStatus.Success)
        {
            var req = requestStatus.Request;
            Console.WriteLine($"[MATCHMAKING] Request Status: {req.Status}, Responded by: {req.Responder_name} at {req.Responded_at}");
        }
        Console.WriteLine();

        // 2️⃣9️⃣ Get Current Matchmaking Status (Host)
        Console.WriteLine("[MATCHMAKING] Host checking current matchmaking status...");
        var currentMatchmaking = await sdk.GetCurrentMatchmakingStatusAsync(hostToken);
        if (currentMatchmaking.Success && currentMatchmaking.In_matchmaking)
        {
            var mm = currentMatchmaking.Matchmaking;
            Console.WriteLine($"[MATCHMAKING] Host Status: In Lobby={currentMatchmaking.In_matchmaking}, Is Host={mm.Is_host}");
            Console.WriteLine($"[MATCHMAKING] Lobby: {mm.Matchmaking_id}, Players: {mm.Current_players}/{mm.Max_players}");
            Console.WriteLine($"[MATCHMAKING] Settings: Strict={mm.Strict_full}, Approval={mm.Join_by_requests}, Started={mm.Is_started}");
        }
        Console.WriteLine();

        // 3️⃣0️⃣ Join Matchmaking Directly (Player 2 - lobby allows direct join)
        Console.WriteLine("[MATCHMAKING] Player2 joining directly (no approval required for this demo)...");
        var directJoin = await sdk.JoinMatchmakingDirectlyAsync(players["player2"].Token, matchmakingId);
        Console.WriteLine($"[MATCHMAKING] {directJoin.Message}, Lobby ID={directJoin.Matchmaking_id}\n");

        // 3️⃣1️⃣ Leave Matchmaking (Player 1 leaves to rejoin)
        Console.WriteLine("[MATCHMAKING] Player1 leaving matchmaking to test rejoin...");
        try
        {
            var leaveMatchmaking = await sdk.LeaveMatchmakingAsync(players["player1"].Token);
            Console.WriteLine($"[MATCHMAKING] {leaveMatchmaking.Message}\n");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"[MATCHMAKING] Leave error: {ex.ApiError}");
            Console.WriteLine($"[MATCHMAKING] Note: Player1 was not in matchmaking lobby\n");
        }

        // 3️⃣2️⃣ List Matchmaking Players (Host view)
        Console.WriteLine("[MATCHMAKING] Host listing players in matchmaking lobby...");
        var matchmakingPlayers = await sdk.GetMatchmakingPlayersAsync(hostToken);
        Console.WriteLine($"[MATCHMAKING] Players in lobby ({matchmakingPlayers.Players.Count}):");
        foreach (var player in matchmakingPlayers.Players)
        {
            Console.WriteLine($" - {player.Player_name} (ID: {player.Player_id}, Host: {player.Is_host == 1}, Status: {player.Status}, Heartbeat: {player.Seconds_since_heartbeat}s ago)");
        }
        Console.WriteLine();

        // 3️⃣3️⃣ Send Matchmaking Heartbeat (All players)
        Console.WriteLine("[MATCHMAKING] All players sending heartbeats...");
        foreach (var kvp in players)
        {
            if (kvp.Key != "player1") // Player1 left
            {
                try
                {
                    var heartbeat = await sdk.SendMatchmakingHeartbeatAsync(kvp.Value.Token);
                    Console.WriteLine($"[MATCHMAKING] {kvp.Value.Name} heartbeat: {heartbeat.Status ?? "N/A"}");
                }
                catch (ApiException ex)
                {
                    Console.WriteLine($"[MATCHMAKING] {kvp.Value.Name} heartbeat error: {ex.ApiError}");
                }
            }
        }
        Console.WriteLine();

        // Player1 rejoins
        Console.WriteLine("[MATCHMAKING] Player1 rejoining matchmaking...");
        var rejoin = await sdk.JoinMatchmakingDirectlyAsync(players["player1"].Token, matchmakingId);
        Console.WriteLine($"[MATCHMAKING] {rejoin.Message}\n");

        // 3️⃣4️⃣ Remove Matchmaking Lobby (Host removes - cleanup)
        Console.WriteLine("[MATCHMAKING] Host removing matchmaking lobby (cleanup test)...");
        var removeLobby = await sdk.RemoveMatchmakingLobbyAsync(hostToken);
        Console.WriteLine($"[MATCHMAKING] {removeLobby.Message}\n");

        // Create new lobby for game start demo
        Console.WriteLine("[MATCHMAKING] Creating new lobby for game start demo...");
        var newLobby = await sdk.CreateMatchmakingLobbyAsync(
            hostToken,
            maxPlayers: 3,
            strictFull: false,
            joinByRequests: false
        );
        string newMatchmakingId = newLobby.Matchmaking_id;
        Console.WriteLine($"[MATCHMAKING] New lobby created: {newMatchmakingId}\n");

        // All players join the new lobby
        Console.WriteLine("[MATCHMAKING] All players joining new lobby...");
        foreach (var kvp in players)
        {
            if (kvp.Key != "host") // Host is already in
            {
                try
                {
                    var join = await sdk.JoinMatchmakingDirectlyAsync(kvp.Value.Token, newMatchmakingId);
                    Console.WriteLine($"[MATCHMAKING] {kvp.Value.Name}: {join.Message}");
                }
                catch (ApiException ex)
                {
                    Console.WriteLine($"[MATCHMAKING] {kvp.Value.Name}: Join error - {ex.ApiError}");
                }
            }
        }
        Console.WriteLine();

        // 3️⃣5️⃣ Start Game from Matchmaking (Host starts the game)
        Console.WriteLine("[MATCHMAKING] Host starting game from matchmaking lobby...");
        var startGame = await sdk.StartGameFromMatchmakingAsync(hostToken);
        Console.WriteLine($"[MATCHMAKING] {startGame.Message}");
        Console.WriteLine($"[MATCHMAKING] Game Room Created: ID={startGame.Room_id}, Name={startGame.Room_name}");
        Console.WriteLine($"[MATCHMAKING] Players Transferred: {startGame.Players_transferred}\n");

        // ==================== POST-MATCHMAKING GAME ROOM DEMO ====================

        // Now the host is also a room host - demonstrate room functionality
        Console.WriteLine("[POST-MATCHMAKING] Demonstrating room functionality with matchmaking host...");

        // 1️⃣2️⃣ List all available rooms (should include the new game room)
        Console.WriteLine("[ROOM] Fetching available rooms after matchmaking...");
        var postRooms = await sdk.GetRoomsAsync();
        Console.WriteLine($"[ROOM] Found {postRooms.Rooms.Count} room(s):");
        foreach (var room in postRooms.Rooms)
        {
            Console.WriteLine($" - ID: {room.Room_id}, Name: {room.Room_name}, Players: {room.Current_players}/{room.Max_players}");
        }
        Console.WriteLine();

        // 1️⃣3️⃣ List players in the new game room
        Console.WriteLine("[ROOM] Fetching players in the new game room...");
        var newRoomPlayers = await sdk.GetRoomPlayersAsync(hostToken);
        Console.WriteLine($"[ROOM] Players in new room ({newRoomPlayers.Players.Count}):");
        foreach (var player in newRoomPlayers.Players)
        {
            Console.WriteLine($" - {player.Player_name} (ID: {player.Player_id}, Host: {player.Is_host == 1}, Online: {player.Is_online})");
        }
        Console.WriteLine();

        // 1️⃣5️⃣ Submit actions from different players
        Console.WriteLine("[ACTION] Players submitting actions in the new game room...");
        foreach (var kvp in players)
        {
            try
            {
                var action = await sdk.SubmitActionAsync(
                        kvp.Value.Token,
                        "player_ready",
                        new { player_id = kvp.Value.Id, ready = true, timestamp = DateTime.UtcNow.ToString("o") }
                    );
                Console.WriteLine($"[ACTION] {kvp.Value.Name} submitted ready action: {action.Action_id}");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"[ACTION] {kvp.Value.Name}: Action submit error - {ex.ApiError}");
            }
        }
        Console.WriteLine();

        // 1️⃣6️⃣ Check pending actions
        Console.WriteLine("[ACTION] Checking for pending actions...");
        var pendingActions = await sdk.GetPendingActionsAsync(hostToken);
        if (pendingActions.Actions != null && pendingActions.Actions.Count > 0)
        {
            Console.WriteLine($"[ACTION] Found {pendingActions.Actions.Count} pending actions:");
            foreach (var pendingAction in pendingActions.Actions)
            {
                Console.WriteLine($"[ACTION] - {pendingAction.Player_name}: {pendingAction.Action_type} at {pendingAction.Created_at}");
            }
        }
        Console.WriteLine();

        // 2️⃣0️⃣ Send game start update to all players
        Console.WriteLine("[UPDATE] Host sending game start update to all players...");
        var gameStartUpdate = await sdk.UpdatePlayersAsync(
            hostToken,
            new UpdatePlayersRequest(
                "all",
                "game_start",
                new { game_mode = "competitive", start_time = DateTime.UtcNow.ToString("o") }
            )
        );
        Console.WriteLine($"[UPDATE] Game start update sent to {gameStartUpdate.Updates_sent} players\n");

        // 2️⃣2️⃣ Poll for game updates
        Console.WriteLine("[UPDATE] Players polling for game updates...");
        foreach (var kvp in players)
        {
            try
            {
                var pollUpdates = await sdk.PollUpdatesAsync(kvp.Value.Token);
                if (pollUpdates.Updates.Count > 0)
                {
                    Console.WriteLine($"[UPDATE] {kvp.Value.Name} received {pollUpdates.Updates.Count} update(s):");
                    foreach (var update in pollUpdates.Updates)
                    {
                        Console.WriteLine($"[UPDATE]   - {update.Type} from {update.From_player_id} at {update.Created_at}");
                    }
                }
                else
                {
                    Console.WriteLine($"[UPDATE] {kvp.Value.Name}: No new updates");
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"[UPDATE] {kvp.Value.Name}: Poll error - {ex.ApiError}");
            }
        }
        Console.WriteLine();

        // 2️⃣3️⃣ Get current room information for all players
        Console.WriteLine("[ROOM] All players checking current room status...");
        foreach (var kvp in players)
        {
            try
            {
                var currentRoom = await sdk.GetCurrentRoomAsync(kvp.Value.Token);
                if (currentRoom.Success && currentRoom.In_room && currentRoom.Room != null)
                {
                    var room = currentRoom.Room;
                    Console.WriteLine($"[ROOM] {kvp.Value.Name}: In '{room.Room_name}' (Players: {room.Current_players}/{room.Max_players}, Host: {room.Is_host})");
                }
                else
                {
                    Console.WriteLine($"[ROOM] {kvp.Value.Name}: Not in any room");
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"[ROOM] {kvp.Value.Name}: Room status error - {ex.ApiError}");
            }
        }
        Console.WriteLine();

        // 2️⃣4️⃣ Leave the room (all players)
        Console.WriteLine("[ROOM] All players leaving the game room...");
        foreach (var kvp in players)
        {
            try
            {
                // Send heartbeat before leaving room
                Console.WriteLine($"[ROOM] {kvp.Value.Name}: Sending heartbeat before leaving room...");
                var heartbeat = await sdk.SendPlayerHeartbeatAsync(kvp.Value.Token);
                if (heartbeat.Success)
                {
                    Console.WriteLine($"[ROOM] {kvp.Value.Name}: Heartbeat sent: {heartbeat.Message}");
                }
                
                var leaveRoom = await sdk.LeaveRoomAsync(kvp.Value.Token);
                Console.WriteLine($"[ROOM] {kvp.Value.Name}: {leaveRoom.Message}");
                
                // Logout after leaving room
                Console.WriteLine($"[ROOM] {kvp.Value.Name}: Logging out after leaving room...");
                var logout = await sdk.LogoutPlayerAsync(kvp.Value.Token);
                if (logout.Success)
                {
                    Console.WriteLine($"[ROOM] {kvp.Value.Name}: {logout.Message}");
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"[ROOM] {kvp.Value.Name}: Leave error - {ex.ApiError}");
            }
        }
        Console.WriteLine();

        // 2️⃣5️⃣ Leaderboards
        Console.WriteLine("[LEADERBOARD] Testing leaderboard functionality...");
        
        try
        {
            // Sort by level only
            Console.WriteLine("[LEADERBOARD] Getting leaderboard sorted by level...");
            var leaderboardLevel = await sdk.GetLeaderboardAsync(new[] { "level" }, 10);
            if (leaderboardLevel.Success)
            {
                Console.WriteLine($"[LEADERBOARD] Success! Total players: {leaderboardLevel.Total}");
                Console.WriteLine($"[LEADERBOARD] Sorted by: {string.Join(", ", leaderboardLevel.Sort_by)}");
                Console.WriteLine("[LEADERBOARD] Top players:");
                foreach (var player in leaderboardLevel.Leaderboard.Take(5))
                {
                    Console.WriteLine($"[LEADERBOARD]   Rank {player.Rank}: {player.Player_name} (ID: {player.Player_id})");
                    if (player.Player_data != null)
                    {
                        Console.WriteLine($"[LEADERBOARD]     Level: {player.Player_data.GetValueOrDefault("level", "N/A")}");
                        Console.WriteLine($"[LEADERBOARD]     Rank: {player.Player_data.GetValueOrDefault("rank", "N/A")}");
                    }
                }
            }
            else
            {
                Console.WriteLine("[LEADERBOARD] Failed to get leaderboard by level");
            }
            Console.WriteLine();

            // Sort by level then score
            Console.WriteLine("[LEADERBOARD] Getting leaderboard sorted by level, then score...");
            var leaderboardLevelScore = await sdk.GetLeaderboardAsync(new[] { "level", "score" }, 10);
            if (leaderboardLevelScore.Success)
            {
                Console.WriteLine($"[LEADERBOARD] Success! Total players: {leaderboardLevelScore.Total}");
                Console.WriteLine($"[LEADERBOARD] Sorted by: {string.Join(", ", leaderboardLevelScore.Sort_by)}");
                Console.WriteLine("[LEADERBOARD] Top players:");
                foreach (var player in leaderboardLevelScore.Leaderboard.Take(5))
                {
                    Console.WriteLine($"[LEADERBOARD]   Rank {player.Rank}: {player.Player_name} (ID: {player.Player_id})");
                    if (player.Player_data != null)
                    {
                        Console.WriteLine($"[LEADERBOARD]     Level: {player.Player_data.GetValueOrDefault("level", "N/A")}");
                        Console.WriteLine($"[LEADERBOARD]     Score: {player.Player_data.GetValueOrDefault("score", "N/A")}");
                        var inventory = player.Player_data.GetValueOrDefault("inventory", null);
                        if (inventory != null && inventory.ToString() != "")
                        {
                            Console.WriteLine($"[LEADERBOARD]     Inventory: {inventory}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("[LEADERBOARD] Failed to get leaderboard by level and score");
            }
            Console.WriteLine();

            // Sort by score then level
            Console.WriteLine("[LEADERBOARD] Getting leaderboard sorted by score, then level...");
            var leaderboardScoreLevel = await sdk.GetLeaderboardAsync(new[] { "score", "level" }, 10);
            if (leaderboardScoreLevel.Success)
            {
                Console.WriteLine($"[LEADERBOARD] Success! Total players: {leaderboardScoreLevel.Total}");
                Console.WriteLine($"[LEADERBOARD] Sorted by: {string.Join(", ", leaderboardScoreLevel.Sort_by)}");
                Console.WriteLine("[LEADERBOARD] Top players:");
                foreach (var player in leaderboardScoreLevel.Leaderboard.Take(5))
                {
                    Console.WriteLine($"[LEADERBOARD]   Rank {player.Rank}: {player.Player_name} (ID: {player.Player_id})");
                    if (player.Player_data != null)
                    {
                        Console.WriteLine($"[LEADERBOARD]     Score: {player.Player_data.GetValueOrDefault("score", "N/A")}");
                        Console.WriteLine($"[LEADERBOARD]     Level: {player.Player_data.GetValueOrDefault("level", "N/A")}");
                        var inventory = player.Player_data.GetValueOrDefault("inventory", null);
                        if (inventory != null && inventory.ToString() != "")
                        {
                            Console.WriteLine($"[LEADERBOARD]     Inventory: {inventory}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("[LEADERBOARD] Failed to get leaderboard by score and level");
            }
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"[LEADERBOARD] Error: {ex.ApiError}");
        }
        Console.WriteLine();

        Console.WriteLine("=== Complete Matchmaking & Game Demo ===");
        Console.WriteLine("✅ Multiple players registered and authenticated");
        Console.WriteLine("✅ Matchmaking lobby created with custom settings");
        Console.WriteLine("✅ Join requests and approval system demonstrated");
        Console.WriteLine("✅ Direct join functionality tested");
        Console.WriteLine("✅ Player management and heartbeats working");
        Console.WriteLine("✅ Game started from matchmaking lobby");
        Console.WriteLine("✅ Host transitioned from matchmaking to room host");
        Console.WriteLine("✅ Full game room functionality demonstrated");
        Console.WriteLine("✅ Leaderboard system with multiple sorting options tested");
    }

    private static async Task<PlayerRegisterResponse> RegisterPlayer(string name, object playerData)
    {
        Console.WriteLine($"[PLAYER] Registering {name}...");
        var reg = await sdk.RegisterPlayer(name, playerData);
        Console.WriteLine($"[PLAYER] {name} registered: ID={reg.Player_id}");
        return reg;
    }

    private static string GetJsonValue(Dictionary<string, object> data, string key)
    {
        if (data.ContainsKey(key))
        {
            return data[key].ToString();
        }
        return "N/A";
    }

    private class PlayerInfo
    {
        public required string Id { get; set; }
        public required string Token { get; set; }
        public required string Name { get; set; }
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
