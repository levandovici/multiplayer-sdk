import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.*;
import com.michitai.multiplayer.errors.*;
import com.michitai.multiplayer.games.*;
import com.michitai.multiplayer.leaderboard.*;
import com.michitai.multiplayer.matchmaking.*;
import com.michitai.multiplayer.matchmaking.requests.*;
import com.michitai.multiplayer.players.*;
import com.michitai.multiplayer.rooms.*;
import com.michitai.multiplayer.rooms.actions.*;
import com.michitai.multiplayer.rooms.realtime.*;
import com.michitai.multiplayer.rooms.updates.*;
import com.michitai.multiplayer.time.*;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;

public class Game {
    private static Client client;
    private static final Map<String, PlayerInfo> players = new HashMap<>();

    private static final String API_TOKEN = "YOUR_API_TOKEN";
    private static final String API_PRIVATE_TOKEN = "YOUR_PRIVATE_TOKEN";

    public static void main(String[] args) {
        System.out.println("=== MICHITAI Game SDK - Java Demo Started ===\n");

        Logger logger = new ConsoleLogger();
        client = new Client(
            API_TOKEN,
            API_PRIVATE_TOKEN,
            "https://api.michitai.com/api",
            logger
        );

        System.out.println("[INIT] SDK initialized with Jackson Databind");

        try {
            runAllDemos();
        } catch (Exception e) {
            System.err.println("[FATAL] Unexpected error: " + e.getMessage());
            e.printStackTrace();
        }

        System.out.println("=== All Demos Finished ===");
    }

    private static void runAllDemos() throws Exception {
        runDemoWithJoinByRequests();
        cleanupEverything();

        runDemoWithoutJoinByRequests();
        cleanupEverything();

        runDemoDirectRoom();
        cleanupEverything();

        runCommonTests();
    }

    // ====================== COMMON TESTS ======================
    private static void runCommonTests() throws Exception {
        System.out.println("\n=== COMMON TESTS: TIME, LEADERBOARD, GAME DATA ===\n");

        // Game Data
        safeExecute(() -> {
            GameDataResponse<GameData> gd = Games.getGameData(client, GameData.class);
            System.out.println("[GAME DATA] Retrieved. Game ID: " + gd.getGameId());
            GameData gameData = gd.getData();
            System.out.println("[GAME DATA] Event: " + gameData.getCurrentEvent() + ", Version: " + gameData.getVersion());
            
            GameData newGameData = new GameData();
            newGameData.setCurrentEvent("SpringFestival");
            newGameData.setVersion("1.2.3");
            Games.updateGameData(client, newGameData);
            System.out.println("[GAME DATA] Global data updated");
        }, "Game Data");

        // Server Time
        safeExecute(() -> {
            ServerTimeResponse time = Time.getServerTime(client);
            System.out.println("[TIME] Server UTC: " + time.getUtc());
        }, "GetServerTime");

        safeExecute(() -> {
            ServerTimeWithOffsetResponse timeOffset = Time.getServerTimeWithOffset(client, 3);
            System.out.println("[TIME] Server UTC+3: " + timeOffset.getUtc());
        }, "GetServerTimeWithOffset");

        // Leaderboard
        safeExecute(() -> {
            LeaderboardResponse<PlayerData> lb = Leaderboard.getLeaderboard(
                client, 
                new String[]{"level", "wins"}, 
                10, 
                PlayerData.class
            );
            System.out.println("[LEADERBOARD] Top " + lb.getLeaderboard().size() + " players");
            if (!lb.getLeaderboard().isEmpty()) {
                LeaderboardPlayer<PlayerData> top = lb.getLeaderboard().get(0);
                System.out.println("[LEADERBOARD] #1: " + top.getPlayerName() + ", Level: " + top.getPlayerData().getLevel() + " (Rank " + top.getRank() + ")");
            }
        }, "GetLeaderboard");
    }

    // ===================================================================
    // DEMO 1: MATCHMAKING WITH JOIN REQUESTS (Approval Flow)
    // ===================================================================
    private static void runDemoWithJoinByRequests() throws Exception {
        System.out.println("\n=== DEMO 1: MATCHMAKING WITH JOIN REQUESTS ===\n");
        setupPlayers();

        String matchmakingId = createMatchmakingLobby("DEMO 1 Matchmaking", true);

        String req1 = requestToJoinMatchmaking(players.get("p1").getToken(), matchmakingId, null);
        checkJoinRequestStatus(players.get("p1").getToken(), req1);
        approveJoinRequest(players.get("host").getToken(), req1);

        String req2 = requestToJoinMatchmaking(players.get("p2").getToken(), matchmakingId, new PlayerData());
        checkJoinRequestStatus(players.get("p2").getToken(), req2);

        getCurrentMatchmakingStatus();

        approveJoinRequest(players.get("host").getToken(), req2);

        getCurrentMatchmakingStatus();
        getMatchmakingPlayers();

        String roomId = startMatchmakingAndCreateRoom();
        runGameRoomFlow(roomId);
    }

    // ===================================================================
    // DEMO 2: MATCHMAKING DIRECT JOIN
    // ===================================================================
    private static void runDemoWithoutJoinByRequests() throws Exception {
        System.out.println("\n=== DEMO 2: MATCHMAKING DIRECT JOIN ===\n");
        setupPlayers();

        String matchmakingId = createMatchmakingLobby("DEMO 2 Matchmaking", false);

        for (PlayerInfo p : players.values()) {
            if (p.getToken().equals(players.get("host").getToken())) continue;
            joinMatchmakingDirectly(p.getToken(), matchmakingId, new PlayerData());
        }

        getCurrentMatchmakingStatus();
        getMatchmakingPlayers();

        String roomId = startMatchmakingAndCreateRoom();
        runGameRoomFlow(roomId);
    }

    // ===================================================================
    // DEMO 3: DIRECT ROOM (No Matchmaking)
    // ===================================================================
    private static void runDemoDirectRoom() throws Exception {
        System.out.println("\n=== DEMO 3: DIRECT ROOM CREATION ===\n");
        setupPlayers();

        RoomCreateResponse create = Rooms.createRoom(
            client,
            players.get("host").getToken(),
            "Direct Battle Arena",
            4,
            null,
            false,
            true,
            false,
            new PlayerData(),
            new RulesData()
        );
        String roomId = create.getRoomId();

        joinRoom(players.get("p1").getToken(), roomId);
        joinRoom(players.get("p2").getToken(), roomId);

        runGameRoomFlow(roomId);
    }

    // ====================== SETUP ======================
    private static void setupPlayers() throws Exception {
        System.out.println("[SETUP] Registering players...");

        PlayerRegisterResponse h = registerPlayer("GameHost", new PlayerData(15, 42, "gold"));
        PlayerRegisterResponse p1 = registerPlayer("PlayerOne", new PlayerData(12, 28, "silver"));
        PlayerRegisterResponse p2 = registerPlayer("PlayerTwo", new PlayerData(10, 15, "bronze"));

        players.put("host", new PlayerInfo(h.getPlayerId(), h.getPrivateKey(), "GameHost"));
        players.put("p1", new PlayerInfo(p1.getPlayerId(), p1.getPrivateKey(), "PlayerOne"));
        players.put("p2", new PlayerInfo(p2.getPlayerId(), p2.getPrivateKey(), "PlayerTwo"));

        // Authenticate all players
        for (PlayerInfo p : players.values()) {
            authenticatePlayer(p.getToken());
        }

        // Send players heartbeat
        for (PlayerInfo p : players.values()) {
            sendPlayerHeartbeat(p.getToken());
        }

        getAllPlayersList();

        for (PlayerInfo p : players.values()) {
            PlayerDataResponse<PlayerData> data = getPlayerData(p.getToken());
            PlayerData player = data.getData();
            player.setLevel(player.getLevel() + 1);
            updatePlayerData(p.getToken(), player);
            data = getPlayerData(p.getToken());
        }
    }

    private static void cleanupEverything() {
        System.out.println("\n[CLEANUP] Logging out players...");

        for (PlayerInfo p : players.values()) {
            safeExecute(() -> Players.logoutPlayer(client, p.getToken()), "Logout " + p.getName());
        }

        players.clear();
    }

    // ====================== HELPER METHODS ======================
    private static PlayerRegisterResponse registerPlayer(String name, PlayerData playerData) throws IOException {
        PlayerRegisterResponse reg = Players.registerPlayer(client, name, playerData);
        System.out.println("[REGISTER] " + name + " registered");
        return reg;
    }

    private static PlayerAuthResponse<PlayerData> authenticatePlayer(String token) throws IOException {
        PlayerAuthResponse<PlayerData> auth = Players.authenticatePlayer(client, token, PlayerData.class);
        System.out.println("[AUTH] " + auth.getPlayer().getPlayerName() + " authenticated");
        return auth;
    }

    private static PlayerHeartbeatResponse sendPlayerHeartbeat(String token) throws IOException {
        PlayerHeartbeatResponse heartbeat = Players.sendPlayerHeartbeat(client, token);
        System.out.println("[HEARTBEAT] Player heartbeat sent");
        return heartbeat;
    }

    private static void getAllPlayersList() throws IOException {
        PlayerListResponse list = Games.getAllPlayers(client);
        System.out.println("[PLAYERS LIST] Total: " + list.getCount());

        for (PlayerShort player : list.getPlayers()) {
            System.out.println("[PLAYERS LIST] Id: " + player.getId() + ", Name: " + player.getPlayerName() + 
                ", Online: " + player.isOnline() + ", Login: " + player.getLastLogin() + 
                ", Created: " + player.getCreatedAt());
        }
    }

    private static PlayerDataResponse<PlayerData> getPlayerData(String token) throws IOException {
        PlayerDataResponse<PlayerData> data = Players.getPlayerData(client, token, PlayerData.class);
        System.out.println("[PLAYER DATA] Player data retrieved");
        return data;
    }

    private static SuccessResponse updatePlayerData(String token, PlayerData data) throws IOException {
        SuccessResponse res = Players.updatePlayerData(client, token, data);
        System.out.println("[PLAYER DATA] Player data updated");
        return res;
    }

    private static String createMatchmakingLobby(String matchmakingName, boolean joinByRequests) throws IOException {
        RulesData rules = new RulesData();
        rules.setMode("tdm");
        rules.setMap("arena");

        PlayerData playerData = new PlayerData();
        playerData.setLevel(3);
        playerData.setWins(5);
        playerData.setRank("Diamond");

        MatchmakingCreateResponse res = Requests.createMatchmakingLobby(
            client,
            players.get("host").getToken(),
            matchmakingName,
            4,
            false,
            joinByRequests,
            false,
            false,
            false,
            null,
            playerData,
            rules
        );

        System.out.println("[MATCHMAKING] Lobby created (requests mode: " + joinByRequests + ")");
        return res.getMatchmakingId();
    }

    private static String requestToJoinMatchmaking(String token, String matchmakingId, PlayerData playerData) throws IOException {
        MatchmakingJoinRequestResponse req = Requests.requestToJoinMatchmaking(client, token, matchmakingId, playerData);
        System.out.println("[REQUEST] Sent: " + req.getRequestId());
        return req.getRequestId();
    }

    private static void checkJoinRequestStatus(String token, String requestId) throws IOException {
        MatchmakingRequestStatusResponse status = Requests.checkJoinRequestStatus(client, token, requestId);
        System.out.println("[REQUEST STATUS] " + status.getRequest().getStatus());
    }

    private static void approveJoinRequest(String hostToken, String requestId) throws IOException {
        MatchmakingPermissionResponse resp = Requests.respondToJoinRequest(
            client, 
            hostToken, 
            requestId, 
            EMatchmakingRequestAction.APPROVE
        );
        System.out.println("[APPROVE] " + resp.getMessage());
    }

    private static void joinMatchmakingDirectly(String token, String matchmakingId, PlayerData playerData) throws IOException {
        Matchmaking.joinMatchmakingDirectly(client, token, matchmakingId, playerData);
        System.out.println("[JOIN] Player joined matchmaking directly");
    }

    private static void getCurrentMatchmakingStatus() throws IOException {
        MatchmakingCurrentResponse<RulesData> s = Matchmaking.getCurrentMatchmakingStatus(
            client, 
            players.get("host").getToken(), 
            RulesData.class
        );

        System.out.println("[MATCHMAKING STATUS] Players in lobby: " + 
            (s.getMatchmaking() != null ? s.getMatchmaking().getCurrentPlayers() : 0));
    }

    private static void getMatchmakingPlayers() throws IOException {
        MatchmakingPlayersResponse<PlayerData> list = Matchmaking.getMatchmakingPlayers(
            client, 
            players.get("host").getToken(), 
            PlayerData.class
        );
        System.out.println("[MATCHMAKING PLAYERS] " + list.getPlayers().size() + " players");
    }

    private static String startMatchmakingAndCreateRoom() throws IOException {
        MatchmakingStartResponse start = Matchmaking.startGameFromMatchmaking(client, players.get("host").getToken());
        System.out.println("[START] Room created: " + start.getRoomId());
        return start.getRoomId();
    }

    private static void joinRoom(String token, String roomId) throws IOException {
        Rooms.joinRoom(client, token, roomId, null, new PlayerData());
        System.out.println("[ROOM] Player joined room " + roomId);
    }

    private static void runGameRoomFlow(String roomId) throws Exception {
        System.out.println("\n=== GAME ROOM FLOW ===\n");

        Rooms.getRooms(client, null, null, RulesData.class);

        CurrentRoomResponse<RulesData> room = Rooms.getCurrentRoom(
            client, 
            players.get("host").getToken(), 
            RulesData.class
        );

        // Players submit actions
        for (PlayerInfo p : players.values()) {
            safeExecute(() -> {
                ActionData actionData = new ActionData();
                actionData.setReady(true);
                
                SubmitAction<ActionData> actionReq = new SubmitAction<>(
                    ERoomTargetPlayers.HOST, 
                    "player_ready", 
                    actionData
                );
                Actions.submitAction(client, p.getToken(), actionReq);
            }, "SubmitAction " + p.getName());
        }

        // Host checks pending actions
        safeExecute(() -> {
            ActionPendingResponse<ActionData> pending = Actions.getPendingActions(
                client, 
                players.get("host").getToken(), 
                ActionData.class
            );
            System.out.println("[PENDING ACTIONS] " + pending.getActions().size() + " actions");
        }, "GetPendingActions");

        // Host broadcasts update
        safeExecute(() -> {
            UpdateData updateData = new UpdateData();
            updateData.setRound(1);
            updateData.setMessage("Game Started!");
            
            UpdatePlayers<UpdateData> updateReq = new UpdatePlayers<>(
                ERoomTargetPlayers.ALL, 
                "game_start", 
                updateData
            );
            Updates.updatePlayers(client, players.get("host").getToken(), updateReq);
            System.out.println("[UPDATE] Broadcast sent to all players");
        }, "Send Room Update");

        // Players poll updates
        for (PlayerInfo p : players.values()) {
            safeExecute(() -> {
                PollUpdates pollReq = new PollUpdates(ERoomTargetPlayers.HOST);
                Updates.pollUpdates(client, p.getToken(), pollReq);
            }, "PollUpdates " + p.getName());
        }

        Rooms.getRoomPlayers(client, players.get("host").getToken());

        // Heartbeats
        for (PlayerInfo p : players.values()) {
            safeExecute(() -> Rooms.sendRoomHeartbeat(client, p.getToken()), "RoomHeartbeat " + p.getName());
        }

        // Stop room
        safeExecute(() -> Rooms.stopRoom(client, players.get("host").getToken()), "StopRoom " + room.getRoom().getRoomName());
    }

    private static void safeExecute(ThrowingRunnable action, String operation) {
        try {
            System.out.println("[LOG] " + operation);
            action.run();
        } catch (Exception e) {
            System.err.println("[CRASH] " + operation + ": " + e.getMessage());
            e.printStackTrace();
        }
    }

    @FunctionalInterface
    private interface ThrowingRunnable {
        void run() throws Exception;
    }

    // ====================== PLAYER INFO ======================
    private static class PlayerInfo {
        private int id;
        private String token;
        private String name;

        public PlayerInfo(int id, String token, String name) {
            this.id = id;
            this.token = token;
            this.name = name;
        }

        public int getId() { return id; }
        public String getToken() { return token; }
        public String getName() { return name; }
    }

    // ====================== GAME DATA ========================
    private static class GameData {
        @JsonProperty("currentEvent")
        private String currentEvent;
        
        @JsonProperty("version")
        private String version;

        public String getCurrentEvent() { return currentEvent; }
        public void setCurrentEvent(String currentEvent) { this.currentEvent = currentEvent; }
        
        public String getVersion() { return version; }
        public void setVersion(String version) { this.version = version; }
    }

    // ====================== PLAYER DATA ======================
    private static class PlayerData {
        @JsonProperty("level")
        private int level;
        
        @JsonProperty("wins")
        private int wins;
        
        @JsonProperty("rank")
        private String rank;

        public PlayerData() {
            this.level = 1;
            this.wins = 0;
            this.rank = "Default";
        }

        public PlayerData(int level, int wins, String rank) {
            this.level = level;
            this.wins = wins;
            this.rank = rank;
        }

        public int getLevel() { return level; }
        public void setLevel(int level) { this.level = level; }
        
        public int getWins() { return wins; }
        public void setWins(int wins) { this.wins = wins; }
        
        public String getRank() { return rank; }
        public void setRank(String rank) { this.rank = rank; }
    }

    // ====================== MATCHMAKING DATA =================
    private static class RulesData {
        @JsonProperty("mode")
        private String mode;
        
        @JsonProperty("map")
        private String map;

        public String getMode() { return mode; }
        public void setMode(String mode) { this.mode = mode; }
        
        public String getMap() { return map; }
        public void setMap(String map) { this.map = map; }
    }

    // ====================== ROOM DATA ========================
    private static class ActionData {
        @JsonProperty("ready")
        private boolean ready;

        public boolean isReady() { return ready; }
        public void setReady(boolean ready) { this.ready = ready; }
    }

    private static class UpdateData {
        @JsonProperty("round")
        private int round;
        
        @JsonProperty("message")
        private String message;

        public int getRound() { return round; }
        public void setRound(int round) { this.round = round; }
        
        public String getMessage() { return message; }
        public void setMessage(String message) { this.message = message; }
    }
}
