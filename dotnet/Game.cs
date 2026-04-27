using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Michitai;
using Michitai.Multiplayer;
using Michitai.Multiplayer.Time;
using Michitai.Multiplayer.Games;
using Michitai.Multiplayer.Players;
using Michitai.Multiplayer.Matchmaking;
using Michitai.Multiplayer.Matchmaking.Requests;
using Michitai.Multiplayer.Rooms;
using Michitai.Multiplayer.Rooms.Actions;
using Michitai.Multiplayer.Rooms.Updates;
using Michitai.Multiplayer.Leaderboard;
using Michitai.Multiplayer.Errors;

public class Game
{
    private static Multiplayer? client;
    private static readonly Dictionary<string, PlayerInfo> players = new();

    public static async Task Main()
    {
        Console.WriteLine("=== MICHITAI Game SDK - ALL THREE DEMOS + TIME + LEADERBOARD ===\n");

        var logger = new ConsoleLogger();
        client = new Multiplayer("YOUR_API_TOKEN", "YOUR_PRIVATE_TOKEN", logger: logger);

        Console.WriteLine("[INIT] SDK initialized successfully\n");

        try
        {
            // Three main demos
            await RunDemoWithJoinByRequests();        // Demo 1: With approval
            await CleanupEverything();

            await RunDemoWithoutJoinByRequests();     // Demo 2: Direct join
            await CleanupEverything();

            await RunDemoDirectRoom();                // Demo 3: Direct room
            await CleanupEverything();

            // Common tests (run once)
            await RunCommonTests();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] Unexpected error: {ex.Message}");
        }

        Console.WriteLine("\n=== All Demos Finished - All Endpoints Covered ===");
    }

    // ====================== COMMON TESTS (Time + Leaderboard + Game Data) ======================
    private static async Task RunCommonTests()
    {
        Console.WriteLine("=== COMMON TESTS: TIME, LEADERBOARD, GAME DATA ===\n");

        // Game Data
        await SafeExecute(async () =>
        {
            var gd = await Games.GetGameData<GameData>(client!);
            Console.WriteLine($"[GAME DATA] Retrieved global data");
            GameData gameData = gd.GameData;
            Console.WriteLine($"[GAME DATA] Event: {gameData.CurrentEvent}, Version: {gameData.Version}");
            await Games.UpdateGameData(client!, new GameData { CurrentEvent = "SpringFestival", Version = "1.2.3" });
            Console.WriteLine("[GAME DATA] Global data updated");
        }, "Game Data");

        // Time API
        await SafeExecute(async () =>
        {
            var time = await Time.GetServerTime(client!);
            Console.WriteLine($"[TIME] Server UTC: {time.Utc}");
        }, "GetServerTime");

        await SafeExecute(async () =>
        {
            var timeOffset = await Time.GetServerTimeWithOffset(client!, 3);
            Console.WriteLine($"[TIME] Server UTC+3: {timeOffset.Utc}");
        }, "GetServerTimeWithOffset");

        // Leaderboard
        await SafeExecute(async () =>
        {
            var lb = await Leaderboard.GetLeaderboardAsync<PlayerData>(client!, new[] { "level", "wins" }, limit: 10);
            Console.WriteLine($"[LEADERBOARD] Top {lb.Leaderboard.Count} players loaded");
            if (lb.Leaderboard.Count > 0)
                Console.WriteLine($"[LEADERBOARD] #1: {lb.Leaderboard[0].Player_name}, Level: {lb.Leaderboard[0].PlayerData.Level}");
        }, "GetLeaderboard");

        Console.WriteLine();
    }

    // ===================================================================
    // DEMO 1: MATCHMAKING WITH JOIN REQUESTS
    // ===================================================================
    private static async Task RunDemoWithJoinByRequests()
    {
        Console.WriteLine("\n=== DEMO 1: MATCHMAKING WITH JOIN REQUESTS ===\n");
        await SetupPlayers();

        string matchmakingId = await CreateMatchmakingLobby("DEMO 1 Matchmaking", joinByRequests: true);

        string req1 = await RequestToJoinMatchmaking(players["p1"].Token, matchmakingId, null);
        await CheckJoinRequestStatus(players["p1"].Token, req1);
        await ApproveJoinRequest(players["host"].Token, req1);

        string req2 = await RequestToJoinMatchmaking(players["p2"].Token, matchmakingId, new PlayerData());
        await CheckJoinRequestStatus(players["p2"].Token, req2);

        await GetCurrentMatchmakingStatus();

        await ApproveJoinRequest(players["host"].Token, req2);

        await GetCurrentMatchmakingStatus();
        await GetMatchmakingPlayers();

        string roomId = await StartMatchmakingAndCreateRoom();
        await RunGameRoomFlow(roomId, isFromMatchmaking: true);
    }

    // ===================================================================
    // DEMO 2: MATCHMAKING DIRECT JOIN
    // ===================================================================
    private static async Task RunDemoWithoutJoinByRequests()
    {
        Console.WriteLine("\n=== DEMO 2: MATCHMAKING DIRECT JOIN ===\n");
        await SetupPlayers();

        string matchmakingId = await CreateMatchmakingLobby("DEMO 2 Matchmaking", joinByRequests: false);

        foreach (var p in players.Values)
        {
            if (players["host"].Token == p.Token)
                continue;

            await JoinMatchmakingDirectly(p.Token, matchmakingId, new PlayerData());
        }

        await GetCurrentMatchmakingStatus();
        await GetMatchmakingPlayers();

        string roomId = await StartMatchmakingAndCreateRoom();
        await RunGameRoomFlow(roomId, isFromMatchmaking: true);
    }

    // ===================================================================
    // DEMO 3: DIRECT ROOM (No Matchmaking)
    // ===================================================================
    private static async Task RunDemoDirectRoom()
    {
        Console.WriteLine("\n=== DEMO 3: DIRECT ROOM CREATION ===\n");
        await SetupPlayers();

        var create = await Rooms.CreateRoomAsync<PlayerData, RulesData>(client!, players["host"].Token, "Direct Battle Arena", 4);
        string roomId = create.Room_id;

        await JoinRoom(players["p1"].Token, roomId);
        await JoinRoom(players["p2"].Token, roomId);

        await RunGameRoomFlow(roomId, isFromMatchmaking: false);
    }

    // ====================== SETUP & CLEANUP ======================
    private static async Task SetupPlayers()
    {
        Console.WriteLine("[SETUP] Registering players...");

        var h = await RegisterPlayer("GameHost", new { level = 15, rank = "gold" });
        var p1 = await RegisterPlayer("PlayerOne", new { level = 12, rank = "silver" });
        var p2 = await RegisterPlayer("PlayerTwo", new { level = 10, rank = "bronze" });

        players["host"] = new PlayerInfo { Id = h.Player_id, Token = h.Private_key, Name = "GameHost" };
        players["p1"] = new PlayerInfo { Id = p1.Player_id, Token = p1.Private_key, Name = "PlayerOne" };
        players["p2"] = new PlayerInfo { Id = p2.Player_id, Token = p2.Private_key, Name = "PlayerTwo" };

        foreach (var p in players.Values)
            await AuthenticatePlayer(p.Token);

        foreach (var p in players.Values)
            await SendPlayerHeartbeat(p.Token);

        await GetAllPlayersList();

        foreach (var p in players.Values)
        {
            var data = await GetPlayerData(p.Token);

            var player = data.PlayerData;

            player.Level++;

            await UpdatePlayerData(p.Token, player);

            data = await GetPlayerData(p.Token);
        }
    }

    private static async Task CleanupEverything()
    {
        Console.WriteLine("\n[CLEANUP] Final cleanup...");

        foreach (var p in players.Values)
        {
            await SafeExecute(async () => await Players.LogoutPlayerAsync(client!, p.Token), $"Logout {p.Name}");
        }

        players.Clear();
    }

    // ====================== HELPER METHODS ======================
    private static async Task<PlayerRegisterResponse> RegisterPlayer(string name, object? data = null)
    {
        var reg = await Players.RegisterPlayer(client!, name, data);
        Console.WriteLine($"[REGISTER] {name} registered");
        return reg;
    }

    private static async Task<PlayerAuthResponse<PlayerData>> AuthenticatePlayer(string token)
    {
        var auth = await Players.AuthenticatePlayer<PlayerData>(client!, token);
        Console.WriteLine($"[AUTH] {auth!.Player!.Player_name} authenticated");
        return auth;
    }

    private static async Task<PlayerHeartbeatResponse> SendPlayerHeartbeat(string token)
    {
        var heartbeat = await Players.SendPlayerHeartbeatAsync(client!, token);
        Console.WriteLine($"[HEARTBEAT] Player heartbeat sent");
        return heartbeat;
    }

    private static async Task GetAllPlayersList()
    {
        var list = await Games.GetAllPlayers(client!);
        Console.WriteLine($"[PLAYERS LIST] Total: {list.Count}");

        foreach (PlayerShort player in list.Players)
        {
            Console.WriteLine($"[PLAYERS LIST] Id: {player.Id}, Name: {player.Player_name}, Online: {player.Is_online}, Login: {player.Last_login}, Created: {player.Created_at}");
        }
    }

    private static async Task<PlayerDataResponse<PlayerData>> GetPlayerData(string token)
    {
        var data = await Players.GetPlayerData<PlayerData>(client!, token);
        Console.WriteLine($"[PLAYER DATA] Player data retrieved");
        return data;
    }

    private static async Task<SuccessResponse> UpdatePlayerData(string token, PlayerData data)
    {
        var res = await Players.UpdatePlayerData(client!, token, data);
        Console.WriteLine($"[PLAYER DATA] Player data updated");
        return res;
    }

    private static async Task<string> CreateMatchmakingLobby(string matchmakingName, bool joinByRequests)
    {
        RulesData rules = new RulesData { Mode = "tdm", Map = "arena" };

        PlayerData playerData = new PlayerData { Level = 3, Rank = "Diamond" };

        var res = await Requests.CreateMatchmakingLobbyAsync<PlayerData, RulesData>(client!, players["host"].Token,
            matchmakingName, 4, false, joinByRequests, false, false, playerData, rules);
        Console.WriteLine($"[MATCHMAKING] Lobby created (requests={joinByRequests})");
        return res.Matchmaking_id;
    }

    private static async Task<string> RequestToJoinMatchmaking(string token, string matchmakingId, PlayerData? playerData = null)
    {
        var req = await Requests.RequestToJoinMatchmakingAsync<PlayerData>(client!, token, matchmakingId, playerData);
        Console.WriteLine($"[REQUEST] Sent: {req.Request_id}");
        return req.Request_id;
    }

    private static async Task CheckJoinRequestStatus(string token, string requestId)
    {
        var status = await Requests.CheckJoinRequestStatusAsync(client!, token, requestId);
        Console.WriteLine($"[REQUEST STATUS] {status.Request.Status}");
    }

    private static async Task ApproveJoinRequest(string hostToken, string requestId)
    {
        var resp = await Requests.RespondToJoinRequestAsync(client!, hostToken, requestId, EMatchmakingRequestAction.Approve);
        Console.WriteLine($"[APPROVE] {resp.Message}");
    }

    private static async Task JoinMatchmakingDirectly(string token, string matchmakingId, PlayerData? playerData = null)
    {
        await Matchmaking.JoinMatchmakingDirectlyAsync<PlayerData>(client!, token, matchmakingId, playerData);
        Console.WriteLine("[JOIN] Player joined directly");
    }

    private static async Task GetCurrentMatchmakingStatus()
    {
        var s = await Matchmaking.GetCurrentMatchmakingStatusAsync<RulesData>(client!, players["host"].Token);
        Console.WriteLine($"[MATCHMAKING STATUS] Players: {s.Matchmaking?.Current_players ?? 0}");
    }

    private static async Task GetMatchmakingPlayers()
    {
        var list = await Matchmaking.GetMatchmakingPlayersAsync<PlayerData>(client!, players["host"].Token);
        Console.WriteLine($"[MATCHMAKING PLAYERS] {list.Players.Count} players");
    }

    private static async Task<string> StartMatchmakingAndCreateRoom()
    {
        var start = await Matchmaking.StartGameFromMatchmakingAsync(client!, players["host"].Token);
        Console.WriteLine($"[START] Room created: {start.Room_id}");
        return start.Room_id;
    }

    private static async Task JoinRoom(string token, string roomId)
    {
        await Rooms.JoinRoomAsync<PlayerData>(client!, token, roomId);
        Console.WriteLine($"[ROOM] Player joined room");
    }

    private static async Task RunGameRoomFlow(string roomId, bool isFromMatchmaking)
    {
        Console.WriteLine("\n=== GAME ROOM FLOW ===\n");

        await Rooms.GetRoomsAsync<RulesData>(client!);
        await Rooms.GetCurrentRoomAsync<RulesData>(client!, players["host"].Token);

        foreach (var p in players.Values)
        {
            await SafeExecute(async () => 
            {
                var req = new SubmitAction<ActionData>(ERoomTargetPlayers.Host, "player_ready", new ActionData { Ready = true });
                await Actions.SubmitActionAsync(client!, p.Token, req);
            }, $"SubmitAction {p.Name}");
        }

        await SafeExecute(async () =>
        {
            var pending = await Actions.GetPendingActionsAsync<ActionData>(client!, players["host"].Token);
            Console.WriteLine($"[PENDING ACTIONS] {pending.Actions.Count} actions");
        }, "GetPendingActions");

        await SafeExecute(async () =>
        {
            var req = new UpdatePlayers<UpdateData>(ERoomTargetPlayers.All, "game_start", new UpdateData { Round = 1, Message = "Game Started!" });
            await Updates.UpdatePlayersAsync<UpdateData>(client!, players["host"].Token, req);
        }, "Send Room Update");

        foreach (var p in players.Values)
            await SafeExecute(async () => await Updates.PollUpdatesAsync<UpdateData>(client!, p.Token, new PollUpdates(ERoomTargetPlayers.Host)), $"PollUpdates {p.Name}");

        await Rooms.GetRoomPlayersAsync<PlayerData>(client!, players["host"].Token);

        foreach (var p in players.Values)
            await SafeExecute(async () => await Rooms.SendRoomHeartbeatAsync(client!, p.Token), $"RoomHeartbeat {p.Name}");

        await SafeExecute(async () => await Rooms.LeaveRoomAsync(client!, players["host"].Token), $"LeaveRoom {players["host"].Name}");
    }

    private static async Task SafeExecute(Func<Task> action, string operation)
    {
        try
        {
            Console.WriteLine($"[LOG] {operation}");

            await action();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRASH] {operation}: {ex.Message}");
        }
    }

    private class PlayerInfo
    {
        public required int Id { get; set; }
        public required string Token { get; set; }
        public required string Name { get; set; }
    }

    private class GameData
    {
        public string CurrentEvent { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;
    }

    private class PlayerData
    {
        public int Level { get; set; }

        public string Rank { get; set; } = string.Empty;



        public PlayerData()
        {
            Level = 1;

            Rank = "Default";
        }
    }

    private class RulesData
    {
        public string Mode { get; set; } = string.Empty;

        public string Map { get; set; } = string.Empty;
    }

    private class ActionData
    {
        public bool Ready { get; set; }
    }

    private class UpdateData
    {
        public int Round { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}