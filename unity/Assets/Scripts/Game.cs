using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using michitai;

public class Game : MonoBehaviour
{
    private static GameSDK sdk;
    private readonly Dictionary<string, PlayerInfo> players = new Dictionary<string, PlayerInfo>();

    [SerializeField]
    private string apiToken = "YOUR_API_TOKEN";

    [SerializeField]
    private string apiPrivateToken = "YOUR_PRIVATE_TOKEN";



    private async void Start()
    {
        Debug.Log("=== MICHITAI Game SDK - Unity Demo Started ===\n");

        var logger = new ConsoleLogger();
        sdk = new GameSDK(
            apiToken: apiToken,
            apiPrivateToken: apiPrivateToken,
            logger: logger,
            useUnityFormat: true   // Important: Use Unity mode
        );

        Debug.Log("[INIT] SDK initialized with Unity format");

        try
        {
            await RunAllDemos();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FATAL] Unexpected error: {ex.Message}");
        }

        Debug.Log("=== All Demos Finished ===");
    }

    private async Task RunAllDemos()
    {
        await RunDemoWithJoinByRequests();
        await CleanupEverything();

        await RunDemoWithoutJoinByRequests();
        await CleanupEverything();

        await RunDemoDirectRoom();
        await CleanupEverything();

        await RunCommonTests();
    }

    // ====================== COMMON TESTS ======================
    private async Task RunCommonTests()
    {
        Debug.Log("\n=== COMMON TESTS: TIME, LEADERBOARD, GAME DATA ===\n");

        // Game Data
        await SafeExecute(async () =>
        {
            var gd = await sdk.GetGameData<GameData>();
            Debug.Log($"[GAME DATA] Retrieved. Game ID: {gd.game_id}");
            GameData gameData = gd.Data;
            Debug.Log($"[GAME DATA] Event: {gameData.currentEvent}, Version: {gameData.version}");
            await sdk.UpdateGameData(new GameData { currentEvent = "SpringFestival", version = "1.2.3" });
            Debug.Log("[GAME DATA] Global data updated");
        }, "Game Data");

        // Server Time
        await SafeExecute(async () =>
        {
            var time = await sdk.GetServerTime();
            Debug.Log($"[TIME] Server UTC: {time.Utc}");
        }, "GetServerTime");

        await SafeExecute(async () =>
        {
            var timeOffset = await sdk.GetServerTimeWithOffset(3);
            Debug.Log($"[TIME] Server UTC+3: {timeOffset.Utc}");
        }, "GetServerTimeWithOffset");

        // Leaderboard
        await SafeExecute(async () =>
        {
            var lb = await sdk.GetLeaderboardAsync<PlayerData>(new[] { "level", "wins" }, 10);
            Debug.Log($"[LEADERBOARD] Top {lb.leaderboard.Count} players");
            if (lb.leaderboard.Count > 0)
            {
                var top = lb.leaderboard[0];
                Debug.Log($"[LEADERBOARD] #1: {top.player_name}, Level: {top.PlayerData.level} (Rank {top.rank})");
            }
        }, "GetLeaderboard");
    }

    // ===================================================================
    // DEMO 1: MATCHMAKING WITH JOIN REQUESTS (Approval Flow)
    // ===================================================================
    private async Task RunDemoWithJoinByRequests()
    {
        Debug.Log("\n=== DEMO 1: MATCHMAKING WITH JOIN REQUESTS ===\n");
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
        await RunGameRoomFlow(roomId);
    }

    // ===================================================================
    // DEMO 2: MATCHMAKING DIRECT JOIN
    // ===================================================================
    private async Task RunDemoWithoutJoinByRequests()
    {
        Debug.Log("\n=== DEMO 2: MATCHMAKING DIRECT JOIN ===\n");
        await SetupPlayers();

        string matchmakingId = await CreateMatchmakingLobby("DEMO 2 Matchmaking", joinByRequests: false);

        foreach (var p in players.Values)
        {
            if (p.Token == players["host"].Token) continue;
            await JoinMatchmakingDirectly(p.Token, matchmakingId, new PlayerData());
        }

        await GetCurrentMatchmakingStatus();
        await GetMatchmakingPlayers();

        string roomId = await StartMatchmakingAndCreateRoom();
        await RunGameRoomFlow(roomId);
    }

    // ===================================================================
    // DEMO 3: DIRECT ROOM (No Matchmaking)
    // ===================================================================
    private async Task RunDemoDirectRoom()
    {
        Debug.Log("\n=== DEMO 3: DIRECT ROOM CREATION ===\n");
        await SetupPlayers();

        var create = await sdk.CreateRoomAsync<PlayerData, RulesData>(players["host"].Token, "Direct Battle Arena", null, 4);
        string roomId = create.room_id;

        await JoinRoom(players["p1"].Token, roomId);
        await JoinRoom(players["p2"].Token, roomId);

        await RunGameRoomFlow(roomId);
    }

    // ====================== SETUP ======================
    private async Task SetupPlayers()
    {
        Debug.Log("[SETUP] Registering players...");

        var h = await RegisterPlayer("GameHost", new PlayerData { level = 15, wins = 42, rank = "gold" });
        var p1 = await RegisterPlayer("PlayerOne", new PlayerData { level = 12, wins = 28, rank = "silver" });
        var p2 = await RegisterPlayer("PlayerTwo", new PlayerData { level = 10, wins = 15, rank = "bronze" });

        players["host"] = new PlayerInfo { Id = h.player_id, Token = h.private_key, Name = "GameHost" };
        players["p1"] = new PlayerInfo { Id = p1.player_id, Token = p1.private_key, Name = "PlayerOne" };
        players["p2"] = new PlayerInfo { Id = p2.player_id, Token = p2.private_key, Name = "PlayerTwo" };

        // Authenticate all players
        foreach (var p in players.Values)
            await AuthenticatePlayer(p.Token);

        // Send players heartbeat
        foreach (var p in players.Values)
            await SendPlayerHeartbeat(p.Token);

        await GetAllPlayersList();

        foreach (var p in players.Values)
        {
            var data = await GetPlayerData(p.Token);

            var player = data.Data;

            player.level++;

            await UpdatePlayerData(p.Token, player);

            data = await GetPlayerData(p.Token);
        }
    }

    private async Task CleanupEverything()
    {
        Debug.Log("\n[CLEANUP] Logging out players...");

        foreach (var p in players.Values)
            await SafeExecute(async () => await sdk.LogoutPlayerAsync(p.Token), $"Logout {p.Name}");

        players.Clear();
    }

    // ====================== HELPER METHODS ======================
    private async Task<PlayerRegisterResponse> RegisterPlayer(string name, PlayerData playerData = null)
    {
        var reg = await sdk.RegisterPlayer<PlayerData>(name, playerData);
        Debug.Log($"[REGISTER] {name} registered");
        return reg;
    }

    private async Task<PlayerAuthResponse<PlayerData>> AuthenticatePlayer(string token)
    {
        var auth = await sdk.AuthenticatePlayer<PlayerData>(token);
        Debug.Log($"[AUTH] {auth.player.player_name} authenticated");
        return auth;
    }

    private async Task<PlayerHeartbeatResponse> SendPlayerHeartbeat(string token)
    {
        var heartbeat = await sdk.SendPlayerHeartbeatAsync(token);
        Debug.Log($"[HEARTBEAT] Player heartbeat sent");
        return heartbeat;
    }

    private async Task GetAllPlayersList()
    {
        var list = await sdk.GetAllPlayers();
        Debug.Log($"[PLAYERS LIST] Total: {list.count}");

        foreach (PlayerShort player in list.players)
        {
            Debug.Log($"[PLAYERS LIST] Id: {player.id}, Name: {player.player_name}, Online: {player.is_online}, Login: {player.LastLogin}, Created: {player.CreatedAt}");
        }
    }

    private async Task<PlayerDataResponse<PlayerData>> GetPlayerData(string token)
    {
        var data = await sdk.GetPlayerData<PlayerData>(token);
        Debug.Log($"[PLAYER DATA] Player data retrieved");
        return data;
    }

    private async Task<SuccessResponse> UpdatePlayerData(string token, PlayerData data)
    {
        var res = await sdk.UpdatePlayerData(token, data);
        Debug.Log($"[PLAYER DATA] Player data updated");
        return res;
    }

    private async Task<string> CreateMatchmakingLobby(string matchmakingName, bool joinByRequests)
    {
        RulesData rules = new RulesData { mode = "tdm", map = "arena" };

        PlayerData playerData = new PlayerData { level = 3, wins = 5, rank = "Diamond" };

        var res = await sdk.CreateMatchmakingLobbyAsync<PlayerData, RulesData>(
            players["host"].Token,
            matchmakingName,
            maxPlayers: 4,
            strictFull: false,
            joinByRequests: joinByRequests,
            hostSwitch:false,
            canLeaveRoom:false,
            playerData: playerData,
            rules: rules
        );

        Debug.Log($"[MATCHMAKING] Lobby created (requests mode: {joinByRequests})");
        return res.matchmaking_id;
    }

    private async Task<string> RequestToJoinMatchmaking(string token, string matchmakingId, PlayerData playerData = null)
    {
        var req = await sdk.RequestToJoinMatchmakingAsync<PlayerData>(token, matchmakingId);
        Debug.Log($"[REQUEST] Sent: {req.request_id}");
        return req.request_id;
    }

    private async Task CheckJoinRequestStatus(string token, string requestId)
    {
        var status = await sdk.CheckJoinRequestStatusAsync(token, requestId);
        Debug.Log($"[REQUEST STATUS] {status.request.status}");
    }

    private async Task ApproveJoinRequest(string hostToken, string requestId)
    {
        var resp = await sdk.RespondToJoinRequestAsync(hostToken, requestId, MatchmakingRequestAction.Approve);
        Debug.Log($"[APPROVE] {resp.message}");
    }

    private async Task JoinMatchmakingDirectly(string token, string matchmakingId, PlayerData playerData = null)
    {
        await sdk.JoinMatchmakingDirectlyAsync<PlayerData>(token, matchmakingId, playerData);
        Debug.Log("[JOIN] Player joined matchmaking directly");
    }

    private async Task GetCurrentMatchmakingStatus()
    {
        var s = await sdk.GetCurrentMatchmakingStatusAsync<RulesData>(players["host"].Token);

        Debug.Log($"[MATCHMAKING STATUS] Players in lobby: {s.matchmaking?.current_players ?? 0}");
    }

    private async Task GetMatchmakingPlayers()
    {
        var list = await sdk.GetMatchmakingPlayersAsync<PlayerData>(players["host"].Token);
        Debug.Log($"[MATCHMAKING PLAYERS] {list.players.Count} players");
    }

    private async Task<string> StartMatchmakingAndCreateRoom()
    {
        var start = await sdk.StartGameFromMatchmakingAsync(players["host"].Token);
        Debug.Log($"[START] Room created: {start.room_id}");
        return start.room_id;
    }

    private async Task JoinRoom(string token, string roomId)
    {
        await sdk.JoinRoomAsync<PlayerData>(token, roomId);
        Debug.Log($"[ROOM] Player joined room {roomId}");
    }

    private async Task RunGameRoomFlow(string roomId)
    {
        Debug.Log("\n=== GAME ROOM FLOW ===\n");

        await sdk.GetRoomsAsync<RulesData>();
        await sdk.GetCurrentRoomAsync<RulesData>(players["host"].Token);

        // Players submit actions
        foreach (var p in players.Values)
        {
            await SafeExecute(async () =>
                await sdk.SubmitActionAsync<ActionData>(p.Token, "player_ready", new ActionData { ready = true }),
                $"SubmitAction {p.Name}");
        }

        // Host checks pending actions
        await SafeExecute(async () =>
        {
            var pending = await sdk.GetPendingActionsAsync<ActionData>(players["host"].Token);
            Debug.Log($"[PENDING ACTIONS] {pending.actions.Count} actions");
        }, "GetPendingActions");

        // Host broadcasts update
        await SafeExecute(async () =>
        {
            UpdateData updateData = new UpdateData { round = 1, message = "Game Started!" };
            var updateReq = new UpdatePlayers<UpdateData>(RoomTargetPlayers.All, "game_start", updateData);
            await sdk.UpdatePlayersAsync<UpdateData>(players["host"].Token, updateReq);
            Debug.Log("[UPDATE] Broadcast sent to all players");
        }, "Send Room Update");

        // Players poll updates
        foreach (var p in players.Values)
            await SafeExecute(async () => await sdk.PollUpdatesAsync(p.Token), $"PollUpdates {p.Name}");

        await sdk.GetRoomPlayersAsync<PlayerData>(players["host"].Token);

        // Heartbeats
        foreach (var p in players.Values)
            await SafeExecute(async () => await sdk.SendRoomHeartbeatAsync(p.Token), $"RoomHeartbeat {p.Name}");

        // Leave room
        await SafeExecute(async () => await sdk.LeaveRoomAsync(players["host"].Token), $"LeaveRoom {players["host"].Name}");
    }

    private async Task SafeExecute(Func<Task> action, string operation)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CRASH] {operation}: {ex.Message}");
        }
    }

    // ====================== PLAYER INFO ======================
    private class PlayerInfo
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
    }

    // ====================== GAME DATA ========================

    [System.Serializable]
    private class GameData
    {
        public string currentEvent;
        public string version;
    }

    // ====================== PLAYER DATA ======================

    [System.Serializable]
    private class PlayerData
    {
        public int level;
        public int wins;
        public string rank;



        public PlayerData()
        {
            level = 1;

            wins = 0;

            rank = "Default";
        }
    }

    // ====================== MATCHMAKING DATA =================

    [System.Serializable]
    private class RulesData
    {
        public string mode;
        public string map;
    }

    // ====================== ROOM DATA ========================

    [System.Serializable]
    private class ActionData
    {
        public bool ready;
    }

    [System.Serializable]
    private class UpdateData
    {
        public int round;
        public string message;
    }
}