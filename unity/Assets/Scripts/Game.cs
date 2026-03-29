using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using michitai;

public class Game : MonoBehaviour
{
    private static GameSDK sdk;
    private readonly Dictionary<string, PlayerInfo> players = new Dictionary<string, PlayerInfo>();

    private async void Start()
    {
        Debug.Log("=== MICHITAI Game SDK - Unity Demo Started ===\n");

        var logger = new ConsoleLogger();
        sdk = new GameSDK(
            apiToken: "YOUR_API_TOKEN",
            apiPrivateToken: "YOUR_PRIVATE_TOKEN",
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
            GameData gameData = gd.GetData;
            Debug.Log($"[GAME DATA] Event: {gameData.currentEvent}, Version: {gameData.version}");
            await sdk.UpdateGameData(new GameData { currentEvent = "SpringFestival", version = "1.2.3" });
            Debug.Log("[GAME DATA] Global data updated");
        }, "Game Data");

        // Server Time
        await SafeExecute(async () =>
        {
            var time = await sdk.GetServerTime();
            Debug.Log($"[TIME] Server UTC: {time.utc}");
        }, "GetServerTime");

        await SafeExecute(async () =>
        {
            var timeOffset = await sdk.GetServerTimeWithOffset(3);
            Debug.Log($"[TIME] UTC+3: {timeOffset.readable}");
        }, "GetServerTimeWithOffset");

        // Leaderboard
        await SafeExecute(async () =>
        {
            var lb = await sdk.GetLeaderboardAsync<PlayerData>(new[] { "level", "wins" }, 10);
            Debug.Log($"[LEADERBOARD] Top {lb.leaderboard.Count} players");
            if (lb.leaderboard.Count > 0)
            {
                var top = lb.leaderboard[0];
                Debug.Log($"[LEADERBOARD] #1: {top.player_name}, Level: {top.GetData.level} (Rank {top.rank})");
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

        string matchmakingId = await CreateMatchmakingLobby(joinByRequests: true);

        string req1 = await RequestToJoinMatchmaking(players["p1"].Token, matchmakingId);
        await CheckJoinRequestStatus(players["p1"].Token, req1);
        await ApproveJoinRequest(players["host"].Token, req1);

        string req2 = await RequestToJoinMatchmaking(players["p2"].Token, matchmakingId);
        await CheckJoinRequestStatus(players["p2"].Token, req2);
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

        string matchmakingId = await CreateMatchmakingLobby(joinByRequests: false);

        foreach (var p in players.Values)
        {
            if (p.Token == players["host"].Token) continue;
            await JoinMatchmakingDirectly(p.Token, matchmakingId);
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

        var create = await sdk.CreateRoomAsync(players["host"].Token, "Direct Battle Arena", null, 4);
        string roomId = create.room_id;

        await JoinRoom(players["p1"].Token, roomId);
        await JoinRoom(players["p2"].Token, roomId);

        await RunGameRoomFlow(roomId);
    }

    // ====================== SETUP ======================
    private async Task SetupPlayers()
    {
        Debug.Log("[SETUP] Registering players...");

        var h = await RegisterPlayer("GameHost", JsonUtility.ToJson(new PlayerData { level = 15, wins = 42, rank = "gold" }));
        var p1 = await RegisterPlayer("PlayerOne", JsonUtility.ToJson(new PlayerData { level = 12, wins = 28, rank = "silver" }));
        var p2 = await RegisterPlayer("PlayerTwo", JsonUtility.ToJson(new PlayerData { level = 10, wins = 15, rank = "bronze" }));

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

            var player = data.GetData;

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
    private async Task<PlayerRegisterResponse> RegisterPlayer(string name, string playerData = "")
    {
        var reg = await sdk.RegisterPlayer(name, playerData);
        Debug.Log($"[REGISTER] {name} registered");
        return reg;
    }

    private async Task<PlayerAuthResponse> AuthenticatePlayer(string token)
    {
        var auth = await sdk.AuthenticatePlayer(token);
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
            Debug.Log($"[PLAYERS LIST] Id: {player.id}, Name: {player.player_name}, Online: {player.is_active}, Login: {player.last_login}, Created: {player.created_at}");
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

    private async Task<string> CreateMatchmakingLobby(bool joinByRequests)
    {
        RulesData rules = new RulesData { mode = "tdm", map = "arena" };

        var res = await sdk.CreateMatchmakingLobbyAsync(
            players["host"].Token,
            maxPlayers: 4,
            strictFull: false,
            joinByRequests: joinByRequests,
            rules: JsonUtility.ToJson(rules)
        );

        Debug.Log($"[MATCHMAKING] Lobby created (requests mode: {joinByRequests})");
        return res.matchmaking_id;
    }

    private async Task<string> RequestToJoinMatchmaking(string token, string matchmakingId)
    {
        var req = await sdk.RequestToJoinMatchmakingAsync(token, matchmakingId);
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

    private async Task JoinMatchmakingDirectly(string token, string matchmakingId)
    {
        await sdk.JoinMatchmakingDirectlyAsync(token, matchmakingId);
        Debug.Log("[JOIN] Player joined matchmaking directly");
    }

    private async Task GetCurrentMatchmakingStatus()
    {
        var s = await sdk.GetCurrentMatchmakingStatusAsync(players["host"].Token);
        Debug.Log($"[MATCHMAKING STATUS] Players in lobby: {s.matchmaking?.current_players ?? 0}");
    }

    private async Task GetMatchmakingPlayers()
    {
        var list = await sdk.GetMatchmakingPlayersAsync(players["host"].Token);
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
        await sdk.JoinRoomAsync(token, roomId);
        Debug.Log($"[ROOM] Player joined room {roomId}");
    }

    private async Task RunGameRoomFlow(string roomId)
    {
        Debug.Log("\n=== GAME ROOM FLOW ===\n");

        await sdk.GetRoomsAsync();
        await sdk.GetCurrentRoomAsync(players["host"].Token);

        // Players submit actions
        foreach (var p in players.Values)
        {
            string requestDataJson = JsonUtility.ToJson(new ActionData { ready = true });

            await SafeExecute(async () =>
                await sdk.SubmitActionAsync(p.Token, "player_ready", requestDataJson),
                $"SubmitAction {p.Name}");
        }

        // Host checks pending actions
        await SafeExecute(async () =>
        {
            var pending = await sdk.GetPendingActionsAsync(players["host"].Token);
            Debug.Log($"[PENDING ACTIONS] {pending.actions.Count} actions");
        }, "GetPendingActions");

        // Host broadcasts update
        await SafeExecute(async () =>
        {
            string dataJson = JsonUtility.ToJson(new { round = 1, message = "Game Started!" });
            var updateReq = new UpdatePlayersRequest("all", "game_start", dataJson);
            await sdk.UpdatePlayersAsync(players["host"].Token, updateReq);
            Debug.Log("[UPDATE] Broadcast sent to all players");
        }, "Send Room Update");

        // Players poll updates
        foreach (var p in players.Values)
            await SafeExecute(async () => await sdk.PollUpdatesAsync(p.Token), $"PollUpdates {p.Name}");

        await sdk.GetRoomPlayersAsync(players["host"].Token);

        // Heartbeats
        foreach (var p in players.Values)
            await SafeExecute(async () => await sdk.SendRoomHeartbeatAsync(p.Token), $"RoomHeartbeat {p.Name}");

        // Leave room
        foreach (var p in players.Values)
            await SafeExecute(async () => await sdk.LeaveRoomAsync(p.Token), $"LeaveRoom {p.Name}");
    }

    private async Task SafeExecute(Func<Task> action, string operation)
    {
        try
        {
            await action();
        }
        catch (ApiException ex)
        {
            Debug.LogError($"[API ERROR] {operation}: {ex.ApiError ?? ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CRASH] {operation}: {ex.Message}");
        }
    }

    // ====================== PLAYER INFO ======================
    private class PlayerInfo
    {
        public string Id { get; set; }
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
}