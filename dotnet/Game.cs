using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using michitai;

public class Game
{
    private static GameSDK? sdk;
    private static readonly Dictionary<string, PlayerInfo> players = new();

    public static async Task Main()
    {
        Console.WriteLine("=== MICHITAI Game SDK Usage Example ===\n");

        var logger = new ConsoleLogger();
        sdk = new GameSDK("YOUR_API_TOKEN", "YOUR_PRIVATE_TOKEN", logger: logger);

        Console.WriteLine("[INIT] SDK initialized\n");

        try
        {
            await RunFullDemo();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] Unexpected error: {ex.Message}");
        }

        Console.WriteLine("\n=== Demo Finished ===");
    }

    private static async Task RunFullDemo()
    {
        // ==================== REGISTER PLAYERS ====================
        Console.WriteLine("[PLAYERS] Registering players...");

        var hostReg = await RegisterPlayer("GameHost", new { level = 10, rank = "gold", role = "host" });
        var p1Reg = await RegisterPlayer("Player1", new { level = 8, rank = "silver", role = "player" });
        var p2Reg = await RegisterPlayer("Player2", new { level = 12, rank = "gold", role = "player" });
        var p3Reg = await RegisterPlayer("Player3", new { level = 6, rank = "bronze", role = "player" });

        players["host"] = new PlayerInfo { Id = hostReg.Player_id, Token = hostReg.Private_key, Name = "GameHost" };
        players["player1"] = new PlayerInfo { Id = p1Reg.Player_id, Token = p1Reg.Private_key, Name = "Player1" };
        players["player2"] = new PlayerInfo { Id = p2Reg.Player_id, Token = p2Reg.Private_key, Name = "Player2" };
        players["player3"] = new PlayerInfo { Id = p3Reg.Player_id, Token = p3Reg.Private_key, Name = "Player3" };

        Console.WriteLine($"[PLAYERS] {players.Count} players registered successfully\n");

        // ==================== AUTHENTICATE ====================
        Console.WriteLine("[AUTH] Authenticating players...");
        foreach (var kvp in players)
        {
            await SafeExecute(async () =>
            {
                await sdk!.AuthenticatePlayer(kvp.Value.Token);
                Console.WriteLine($"[AUTH] {kvp.Value.Name} authenticated");
            }, $"Authenticate {kvp.Value.Name}");
        }
        Console.WriteLine();

        // ==================== MATCHMAKING DEMO ====================
        Console.WriteLine("=== MATCHMAKING DEMO ===");

        string matchmakingId = "";
        string requestId = "";

        // Create lobby
        await SafeExecute(async () =>
        {
            var create = await sdk!.CreateMatchmakingLobbyAsync(
                players["host"].Token, maxPlayers: 4, strictFull: true, joinByRequests: false,
                extraJsonString: new { minLevel = 5, rank = "silver", gameMode = "competitive" });

            matchmakingId = create.Matchmaking_id;
            Console.WriteLine($"[MATCHMAKING] Lobby created: {matchmakingId}");
        }, "Create matchmaking lobby");

        // Player1 requests to join
        await SafeExecute(async () =>
        {
            var req = await sdk!.RequestToJoinMatchmakingAsync(players["player1"].Token, matchmakingId);
            requestId = req.Request_id;
            Console.WriteLine($"[MATCHMAKING] Join request sent: {requestId}");
        }, "Player1 join request");

        // Host approves (FIXED: use requestId)
        await SafeExecute(async () =>
        {
            var approve = await sdk!.RespondToJoinRequestAsync(
                players["host"].Token, requestId, MatchmakingRequestAction.Approve);

            Console.WriteLine($"[MATCHMAKING] Host approved: {approve.Message}");
        }, "Host approve request");

        // Other players join directly
        await SafeExecute(async () => await sdk!.JoinMatchmakingDirectlyAsync(players["player2"].Token, matchmakingId), "Player2 direct join");
        await SafeExecute(async () => await sdk!.JoinMatchmakingDirectlyAsync(players["player3"].Token, matchmakingId), "Player3 direct join");

        // Show lobby players
        await SafeExecute(async () =>
        {
            var list = await sdk!.GetMatchmakingPlayersAsync(players["host"].Token);
            Console.WriteLine($"[MATCHMAKING] {list.Players.Count} players in lobby:");
            foreach (var p in list.Players)
                Console.WriteLine($"   - {p.Player_name} (Host: {p.Is_host == 1})");
        }, "List matchmaking players");

        // Start game from matchmaking
        string? roomId = null;
        await SafeExecute(async () =>
        {
            var start = await sdk!.StartGameFromMatchmakingAsync(players["host"].Token);
            roomId = start.Room_id;
            Console.WriteLine($"[MATCHMAKING] Game started! Room: {roomId}");
        }, "Start game from matchmaking");

        // ==================== GAME ROOM AFTER MATCHMAKING ====================
        if (roomId != null)
        {
            Console.WriteLine("\n=== GAME ROOM DEMO ===");

            // Submit actions
            foreach (var kvp in players)
            {
                await SafeExecute(async () =>
                {
                    var action = await sdk!.SubmitActionAsync(kvp.Value.Token, "player_ready",
                        new { player_id = kvp.Value.Id, ready = true });

                    Console.WriteLine($"[ACTION] {kvp.Value.Name} submitted action");
                }, $"Submit action {kvp.Value.Name}");
            }

            // Host sends update
            await SafeExecute(async () =>
            {
                var update = await sdk!.UpdatePlayersAsync(players["host"].Token,
                    new UpdatePlayersRequest("all", "game_start", new { message = "Game has started!" }));

                Console.WriteLine($"[UPDATE] Sent update to {update.Updates_sent} players");
            }, "Send game start update");

            // Poll updates
            foreach (var kvp in players)
            {
                await SafeExecute(async () =>
                {
                    var updates = await sdk!.PollUpdatesAsync(kvp.Value.Token);
                    Console.WriteLine($"[UPDATE] {kvp.Value.Name} received {updates.Updates.Count} updates");
                }, $"Poll updates for {kvp.Value.Name}");
            }

            // Cleanup - leave room
            foreach (var kvp in players)
            {
                await SafeExecute(async () => await CleanupRoom(kvp.Value.Token), $"Leave room for {kvp.Value.Name}");
            }
        }

        // Leaderboard
        await SafeExecute(async () =>
        {
            Console.WriteLine("\n[LEADERBOARD] Testing...");
            var lb = await sdk!.GetLeaderboardAsync(new[] { "level" }, 10);
            Console.WriteLine($"[LEADERBOARD] Top player: {lb.Leaderboard[0].Player_name} (Rank {lb.Leaderboard[0].Rank})");
        }, "Leaderboard test");
    }

    // ====================== HELPERS ======================

    private static async Task<PlayerRegisterResponse> RegisterPlayer(string name, object playerData)
    {
        Console.WriteLine($"[PLAYER] Registering {name}...");
        var reg = await sdk!.RegisterPlayer(name, playerData);
        Console.WriteLine($"[PLAYER] {name} registered → ID = {reg.Player_id}");
        return reg;
    }

    private static async Task CleanupRoom(string playerToken)
    {
        await sdk!.SendPlayerHeartbeatAsync(playerToken);
        var leave = await sdk.LeaveRoomAsync(playerToken);
        Console.WriteLine($"[ROOM] Left room: {leave.Message}");

        var logout = await sdk.LogoutPlayerAsync(playerToken);
        Console.WriteLine($"[LOGOUT] {logout.Message}");
    }

    /// <summary>
    /// Safe wrapper to prevent one failure from stopping the entire demo
    /// </summary>
    private static async Task SafeExecute(Func<Task> action, string operationName)
    {
        try
        {
            await action();
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"[ERROR] {operationName} failed: {ex.ApiError}");
            if (!string.IsNullOrEmpty(ex.RawResponse))
                Console.WriteLine($"       Raw response: {ex.RawResponse.Substring(0, Math.Min(300, ex.RawResponse.Length))}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {operationName} unexpected error: {ex.Message}");
        }
    }

    private class PlayerInfo
    {
        public required string Id { get; set; }
        public required string Token { get; set; }
        public required string Name { get; set; }
    }
}