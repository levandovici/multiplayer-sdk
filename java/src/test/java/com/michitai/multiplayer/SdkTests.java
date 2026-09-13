package com.michitai.multiplayer;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.JsonNode;
import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpServer;

import com.michitai.multiplayer.errors.ErrorConverter;
import com.michitai.multiplayer.errors.ErrorEnums;
import com.michitai.multiplayer.games.GameDataResponse;
import com.michitai.multiplayer.games.Games;
import com.michitai.multiplayer.players.EBanTime;
import com.michitai.multiplayer.players.PlayerBanResponse;
import com.michitai.multiplayer.players.PlayerRegisterResponse;
import com.michitai.multiplayer.players.Players;
import com.michitai.multiplayer.rooms.actions.ActionInfo;
import com.michitai.multiplayer.rooms.actions.ActionPendingResponse;
import com.michitai.multiplayer.rooms.actions.ActionPollResponse;
import com.michitai.multiplayer.rooms.actions.ActionSubmitRequest;
import com.michitai.multiplayer.rooms.actions.Actions;
import com.michitai.multiplayer.rooms.actions.ERoomActionStatus;
import com.michitai.multiplayer.rooms.actions.ERoomTargetPlayers;
import com.michitai.multiplayer.rooms.actions.PendingAction;
import com.michitai.multiplayer.rooms.actions.SubmitAction;
import com.michitai.multiplayer.rooms.updates.PlayerUpdate;
import com.michitai.multiplayer.rooms.updates.PollUpdates;
import com.michitai.multiplayer.rooms.updates.PollUpdatesRequest;
import com.michitai.multiplayer.rooms.updates.PollUpdatesResponse;
import com.michitai.multiplayer.rooms.updates.UpdatePlayersRequest;
import com.michitai.multiplayer.rooms.updates.Updates;

import java.io.IOException;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.nio.charset.StandardCharsets;
import java.util.HashMap;
import java.util.Map;

/**
 * Standalone test harness for the Michitai Multiplayer Java SDK.
 * Uses a local mock HTTP server (com.sun.net.httpserver) so no external
 * test framework or live API credentials are required.
 *
 * Run: java -cp "bin;bin-test;lib/*" com.michitai.multiplayer.SdkTests
 */
public class SdkTests {
    private static int passed = 0;
    private static int failed = 0;

    private static void check(boolean cond, String name) {
        if (cond) {
            passed++;
            System.out.println("  PASS  " + name);
        } else {
            failed++;
            System.out.println("  FAIL  " + name);
        }
    }

    private static void eq(Object expected, Object actual, String name) {
        boolean ok = expected == null ? actual == null : expected.equals(actual);
        if (!ok) {
            System.out.println("        expected=" + expected + " actual=" + actual);
        }
        check(ok, name);
    }

    // ====================== Mock server ======================

    private static class RecordedRequest {
        String method;
        String path;
        String query;
        String body;
    }

    private static class MockApi {
        final HttpServer server;
        final Map<String, String> responses = new HashMap<>();
        final Map<String, RecordedRequest> requests = new HashMap<>();

        MockApi() throws IOException {
            server = HttpServer.create(new InetSocketAddress("127.0.0.1", 0), 0);
            server.createContext("/", this::handle);
            server.start();
        }

        void when(String path, String json) {
            responses.put(path, json);
        }

        RecordedRequest lastRequest(String path) {
            return requests.get(path);
        }

        String baseUrl() {
            return "http://127.0.0.1:" + server.getAddress().getPort() + "/api";
        }

        void stop() {
            server.stop(0);
        }

        private void handle(HttpExchange ex) throws IOException {
            String path = ex.getRequestURI().getPath().replaceFirst("^/api/", "");

            RecordedRequest rec = new RecordedRequest();
            rec.method = ex.getRequestMethod();
            rec.path = path;
            rec.query = ex.getRequestURI().getRawQuery();
            rec.body = new String(ex.getRequestBody().readAllBytes(), StandardCharsets.UTF_8);
            requests.put(path, rec);

            String json = responses.getOrDefault(path, "{\"success\":false,\"error\":\"Invalid endpoint\"}");
            byte[] bytes = json.getBytes(StandardCharsets.UTF_8);
            ex.getResponseHeaders().set("Content-Type", "application/json");
            ex.sendResponseHeaders(200, bytes.length);
            try (OutputStream os = ex.getResponseBody()) {
                os.write(bytes);
            }
        }
    }

    // ====================== Payload types ======================

    public static class TestGameData {
        @JsonProperty("current_event")
        public String currentEvent;
        @JsonProperty("version")
        public String version;
    }

    public static class TestPlayerData {
        @JsonProperty("level")
        public int level;
        @JsonProperty("wins")
        public int wins;
    }

    // ====================== Tests ======================

    private static void testUrlConstruction() {
        System.out.println("[TEST] URL construction");

        Client client = new Client("PUB", "PRIV", "http://localhost:9999/api", null);
        eq("http://localhost:9999/api/game_data.php/game/get?api_token=PUB",
            client.url("game_data.php/game/get"), "url() appends api_token");
        eq("http://localhost:9999/api/time.php?api_token=PUB&private_token=PRIV",
            client.privateUrl("time.php"), "privateUrl() appends both tokens");
        eq("http://localhost:9999/api/x?api_token=PUB&player_token=PT",
            client.url("x", "&player_token=PT"), "url() appends extra params");

        Client noSlash = new Client("PUB", "PRIV", "http://localhost:9999/api", null);
        eq(noSlash.url("e"), client.url("e"), "missing trailing slash normalized");

        Client withSlash = new Client("PUB", "PRIV", "http://localhost:9999/api/", null);
        eq(withSlash.url("e"), client.url("e"), "existing trailing slash preserved");
    }

    private static void testEnumWireValues() throws IOException {
        System.out.println("[TEST] Enum wire values");

        for (ERoomTargetPlayers target : ERoomTargetPlayers.values()) {
            String expected = target.name().toLowerCase();
            ActionSubmitRequest req = new ActionSubmitRequest(target, "t", "{}", null);
            JsonNode node = Client.JSON_MAPPER.valueToTree(req);
            eq(expected, node.get("target_players").asText(), "action target_players=" + expected);
        }

        UpdatePlayersRequest upd = new UpdatePlayersRequest(ERoomTargetPlayers.OTHERS, "t", "{}", new int[]{1, 2});
        JsonNode node = Client.JSON_MAPPER.valueToTree(upd);
        eq("others", node.get("target_players").asText(), "update target_players=others");
        eq(2, node.get("target_players_ids").size(), "update target_players_ids serialized");

        PollUpdatesRequest poll = new PollUpdatesRequest(ERoomTargetPlayers.SPECIFIC, new int[]{5}, "u1");
        node = Client.JSON_MAPPER.valueToTree(poll);
        eq("specific", node.get("from_players").asText(), "poll from_players=specific");
        eq(5, node.get("from_players_ids").get(0).asInt(), "poll from_players_ids serialized");
        eq("u1", node.get("last_update").asText(), "poll last_update serialized");
    }

    private static void testRegisterPlayer(MockApi api, Client client) throws IOException {
        System.out.println("[TEST] registerPlayer");

        api.when("game_players.php/register",
            "{\"success\":true,\"player_id\":42,\"private_key\":\"pk-abc\",\"player_name\":\"Tester\",\"game_id\":7}");

        Map<String, Object> data = new HashMap<>();
        data.put("level", 3);
        PlayerRegisterResponse res = Players.registerPlayer(client, "Tester", data);

        check(res.isSuccess(), "registerPlayer success");
        eq(42, res.getPlayerId(), "registerPlayer player_id");
        eq("pk-abc", res.getPrivateKey(), "registerPlayer private_key");
        eq("Tester", res.getPlayerName(), "registerPlayer player_name");
        eq(7, res.getGameId(), "registerPlayer game_id");

        RecordedRequest rec = api.lastRequest("game_players.php/register");
        eq("POST", rec.method, "registerPlayer uses POST");
        check(rec.query != null && rec.query.contains("api_token=PUB"), "registerPlayer sends api_token");
        JsonNode body = parse(rec.body);
        eq("Tester", body.get("player_name").asText(), "register body player_name");
        eq("{\"level\":3}", body.get("player_data").asText(), "register body player_data is JSON string");
    }

    private static void testGameDataTyped(MockApi api, Client client) throws IOException {
        System.out.println("[TEST] getGameData typed deserialization");

        api.when("game_data.php/game/get",
            "{\"success\":true,\"type\":\"game\",\"game_id\":7," +
            "\"data\":{\"current_event\":\"SpringFestival\",\"version\":\"1.2.3\"}}");

        GameDataResponse<TestGameData> res = Games.getGameData(client, TestGameData.class);

        check(res.isSuccess(), "getGameData success");
        eq(7, res.getGameId(), "getGameData game_id");
        check(res.getData() != null, "getGameData data present");
        eq("SpringFestival", res.getData().currentEvent, "getGameData typed data.currentEvent");
        eq("1.2.3", res.getData().version, "getGameData typed data.version");

        RecordedRequest rec = api.lastRequest("game_data.php/game/get");
        eq("GET", rec.method, "getGameData uses GET");
        check(rec.query != null && rec.query.contains("api_token=PUB"), "getGameData sends api_token");
    }

    private static void testPendingActionsTyped(MockApi api, Client client) throws IOException {
        System.out.println("[TEST] getPendingActions typed deserialization");

        api.when("game_room.php/actions/pending",
            "{\"success\":true,\"actions\":[{" +
            "\"action_id\":\"a1\",\"player_id\":11,\"target_id\":0,\"player_name\":\"PlayerOne\"," +
            "\"is_host\":false,\"action_type\":\"player_ready\"," +
            "\"request_data\":{\"level\":9},\"created_at\":\"2025-01-01T00:00:00Z\"}]}");

        ActionPendingResponse<TestPlayerData> res =
            Actions.getPendingActions(client, "PT", TestPlayerData.class);

        check(res.isSuccess(), "getPendingActions success");
        eq(1, res.getActions().size(), "getPendingActions count");
        PendingAction<TestPlayerData> a = res.getActions().get(0);
        eq("a1", a.getActionId(), "pending action_id");
        eq(11, a.getPlayerId(), "pending player_id");
        eq("PlayerOne", a.getPlayerName(), "pending player_name");
        eq("player_ready", a.getActionType(), "pending action_type");
        eq(9, a.getRequestData().level, "pending typed request_data.level");

        RecordedRequest rec = api.lastRequest("game_room.php/actions/pending");
        check(rec.query != null && rec.query.contains("player_token=PT"), "getPendingActions sends player_token");
    }

    private static void testPollActionsTyped(MockApi api, Client client) throws IOException {
        System.out.println("[TEST] pollActions typed deserialization");

        api.when("game_room.php/actions/poll",
            "{\"success\":true,\"actions\":[{" +
            "\"action_id\":\"a2\",\"action_type\":\"player_ready\",\"is_host\":true,\"target_id\":22," +
            "\"status\":\"completed\",\"response_data\":{\"wins\":4}," +
            "\"processed_at\":\"2025-01-01T00:00:01Z\"}]}");

        ActionPollResponse<TestPlayerData> res = Actions.pollActions(client, "PT", TestPlayerData.class);

        check(res.isSuccess(), "pollActions success");
        ActionInfo<TestPlayerData> a = res.getActions().get(0);
        eq("a2", a.getActionId(), "poll action_id");
        eq("completed", a.getStatus(), "poll status");
        eq(ERoomActionStatus.COMPLETED, a.getActionStatus(), "poll getActionStatus() parsed");
        check(a.isHost(), "poll is_host");
        eq(22, a.getTargetId(), "poll target_id");
        eq(4, a.getResponseData().wins, "poll typed response_data.wins");
    }

    private static void testPollUpdatesTyped(MockApi api, Client client) throws IOException {
        System.out.println("[TEST] pollUpdates typed deserialization");

        api.when("game_room.php/updates/poll",
            "{\"success\":true,\"last_update\":\"u9\",\"updates\":[{" +
            "\"update_id\":\"u9\",\"from_player_id\":33,\"type\":\"game_start\"," +
            "\"data\":{\"level\":2},\"created_at\":\"2025-01-01T00:00:02Z\"}]}");

        PollUpdatesResponse<TestPlayerData> res =
            Updates.pollUpdates(client, "PT", new PollUpdates(ERoomTargetPlayers.HOST), TestPlayerData.class);

        check(res.isSuccess(), "pollUpdates success");
        eq("u9", res.getLastUpdate(), "pollUpdates last_update");
        PlayerUpdate<TestPlayerData> u = res.getUpdates().get(0);
        eq("u9", u.getUpdateId(), "update update_id");
        eq(33, u.getFromPlayerId(), "update from_player_id");
        eq("game_start", u.getType(), "update type");
        eq(2, u.getData().level, "update typed data.level");
        eq("2025-01-01T00:00:02Z", u.getCreatedAt(), "update created_at");

        RecordedRequest rec = api.lastRequest("game_room.php/updates/poll");
        eq("POST", rec.method, "pollUpdates uses POST");
        JsonNode body = parse(rec.body);
        eq("host", body.get("from_players").asText(), "pollUpdates body from_players=host");
    }

    private static void testBanPlayerWireValue(MockApi api, Client client) throws IOException {
        System.out.println("[TEST] banPlayer enum + private token");

        api.when("game_players.php/ban",
            "{\"success\":true,\"player_id\":11,\"ban_duration\":\"week\"}");

        PlayerBanResponse res = Players.banPlayer(client, 11, EBanTime.WEEK, "cheating");
        check(res.isSuccess(), "banPlayer success");

        RecordedRequest rec = api.lastRequest("game_players.php/ban");
        eq("POST", rec.method, "banPlayer uses POST");
        check(rec.query != null && rec.query.contains("api_token=PUB"), "banPlayer sends api_token");
        check(rec.query != null && rec.query.contains("private_token=PRIV"), "banPlayer sends private_token");
        JsonNode body = parse(rec.body);
        eq("week", body.get("ban_duration").asText(), "ban body ban_duration=week");
        eq(11, body.get("player_id").asInt(), "ban body player_id");
        eq("cheating", body.get("ban_reason").asText(), "ban body ban_reason");
    }

    private static void testErrorResponse(MockApi api, Client client) throws IOException {
        System.out.println("[TEST] Error responses");

        api.when("game_data.php/game/get", "{\"success\":false,\"error\":\"Invalid API token\"}");
        GameDataResponse<?> res = Games.getGameData(client);
        check(!res.isSuccess(), "error response success=false");
        eq("Invalid API token", res.getError(), "error message preserved");

        api.when("time.php", "this is not json");
        SuccessResponse bad = client.get(client.url("time.php"), SuccessResponse.class);
        check(!bad.isSuccess(), "garbage body -> success=false");
        eq("Failed to deserialize response", bad.getError(), "garbage body -> fallback error");
    }

    private static void testErrorConverter() {
        System.out.println("[TEST] ErrorConverter");

        eq(ErrorEnums.ECommonError.InvalidApiToken,
            ErrorConverter.convertToEnum("Invalid API token", ErrorEnums.ECommonError.class),
            "convertToEnum maps message");

        eq(ErrorEnums.ECommonError.Unknown,
            ErrorConverter.convertToEnum("some unmapped error", ErrorEnums.ECommonError.class),
            "convertToEnum falls back to first constant");

        eq(ErrorEnums.ERoomJoinError.RoomIsFull,
            ErrorConverter.convertToEnum("Room is full", ErrorEnums.ERoomJoinError.class),
            "convertToEnum endpoint-specific enum");

        eq("Room is full",
            ErrorConverter.getErrorMessage(ErrorEnums.ERoomJoinError.RoomIsFull),
            "getErrorMessage round-trip");
    }

    private static void testSubmitActionRequest(MockApi api, Client client) throws IOException {
        System.out.println("[TEST] submitAction request shape");

        api.when("game_room.php/actions", "{\"success\":true,\"action_id\":\"a9\"}");

        SubmitAction<TestPlayerData> req =
            new SubmitAction<>(ERoomTargetPlayers.SPECIFIC, "trade", new TestPlayerData(), new int[]{7});
        Actions.submitAction(client, "PT", req);

        RecordedRequest rec = api.lastRequest("game_room.php/actions");
        eq("POST", rec.method, "submitAction uses POST");
        check(rec.query != null && rec.query.contains("player_token=PT"), "submitAction sends player_token");
        JsonNode body = parse(rec.body);
        eq("specific", body.get("target_players").asText(), "action body target_players=specific");
        eq("trade", body.get("action_type").asText(), "action body action_type");
        eq(7, body.get("target_players_ids").get(0).asInt(), "action body target_players_ids");
        check(body.get("request_data").isTextual(), "action body request_data is JSON string");
    }

    private static JsonNode parse(String json) throws IOException {
        return Client.JSON_MAPPER.readTree(json);
    }

    // ====================== Main ======================

    public static void main(String[] args) throws Exception {
        MockApi api = new MockApi();
        Client client = new Client("PUB", "PRIV", api.baseUrl(), null);

        try {
            testUrlConstruction();
            testEnumWireValues();
            testRegisterPlayer(api, client);
            testGameDataTyped(api, client);
            testPendingActionsTyped(api, client);
            testPollActionsTyped(api, client);
            testPollUpdatesTyped(api, client);
            testSubmitActionRequest(api, client);
            testBanPlayerWireValue(api, client);
            testErrorResponse(api, client);
            testErrorConverter();
        } finally {
            api.stop();
        }

        System.out.println();
        System.out.println("=== " + passed + " passed, " + failed + " failed ===");
        if (failed > 0) {
            System.exit(1);
        }
    }
}
