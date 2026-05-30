#include <iostream>
#include <map>
#include <vector>
#include <memory>
#include "../include/michitai/multiplayer/Client.h"
#include "../include/michitai/multiplayer/Players/Players.h"
#include "../include/michitai/multiplayer/Games/Games.h"
#include "../include/michitai/multiplayer/Time/Time.h"
#include "../include/michitai/multiplayer/Rooms/Rooms.h"
#include "../include/michitai/multiplayer/Rooms/Actions/Actions.h"
#include "../include/michitai/multiplayer/Rooms/Updates/Updates.h"
#include "../include/michitai/multiplayer/Matchmaking/Matchmaking.h"
#include "../include/michitai/multiplayer/Matchmaking/Requests/Requests.h"
#include "../include/michitai/multiplayer/Leaderboard/Leaderboard.h"
#include "../include/michitai/multiplayer/Types.h"

using namespace michitai::multiplayer;
using namespace michitai::multiplayer::games;
using namespace michitai::multiplayer::time;
using namespace michitai::multiplayer::rooms;
using namespace michitai::multiplayer::rooms::actions;
using namespace michitai::multiplayer::rooms::updates;
using namespace michitai::multiplayer::matchmaking;
using namespace michitai::multiplayer::matchmaking::requests;
using namespace michitai::multiplayer::leaderboard;

// ====================== DATA STRUCTURES ======================

struct GameData {
    std::string currentEvent = "";
    std::string version = "";
};

struct PlayerData {
    int level = 1;
    std::string rank = "Default";
};

struct RulesData {
    std::string mode = "";
    std::string map = "";
};

struct ActionData {
    bool ready = false;
};

struct UpdateData {
    int round = 0;
    std::string message = "";
};

struct PlayerInfo {
    int id = 0;
    std::string token;
    std::string name;
};

// ====================== JSON SERIALIZATION HELPERS ======================

namespace nlohmann {
    template<>
    struct adl_serializer<GameData> {
        static void to_json(json& j, const GameData& g) {
            j = {{"current_event", g.currentEvent}, {"version", g.version}};
        }
        static void from_json(const json& j, GameData& g) {
            g.currentEvent = j.value("current_event", "");
            g.version = j.value("version", "");
        }
    };
    
    template<>
    struct adl_serializer<PlayerData> {
        static void to_json(json& j, const PlayerData& p) {
            j = {{"level", p.level}, {"rank", p.rank}};
        }
        static void from_json(const json& j, PlayerData& p) {
            p.level = j.value("level", 1);
            p.rank = j.value("rank", "Default");
        }
    };
    
    template<>
    struct adl_serializer<RulesData> {
        static void to_json(json& j, const RulesData& r) {
            j = {{"mode", r.mode}, {"map", r.map}};
        }
        static void from_json(const json& j, RulesData& r) {
            r.mode = j.value("mode", "");
            r.map = j.value("map", "");
        }
    };
    
    template<>
    struct adl_serializer<ActionData> {
        static void to_json(json& j, const ActionData& a) {
            j = {{"ready", a.ready}};
        }
        static void from_json(const json& j, ActionData& a) {
            a.ready = j.value("ready", false);
        }
    };
    
    template<>
    struct adl_serializer<UpdateData> {
        static void to_json(json& j, const UpdateData& u) {
            j = {{"round", u.round}, {"message", u.message}};
        }
        static void from_json(const json& j, UpdateData& u) {
            u.round = j.value("round", 0);
            u.message = j.value("message", "");
        }
    };
}

// ====================== GLOBAL STATE ======================

static std::unique_ptr<Client> client;
static std::map<std::string, PlayerInfo> playerMap;

// Forward declaration
void runGameRoomFlow(const std::string& roomId, bool isFromMatchmaking);

// ====================== SAFE EXECUTION ======================

void safeExecute(const std::string& operation, std::function<void()> action) {
    std::cout << "[LOG] " << operation << std::endl;
    try {
        action();
    } catch (const std::exception& ex) {
        std::cout << "[CRASH] " << operation << ": " << ex.what() << std::endl;
    }
}

// ====================== COMMON TESTS ======================

void runCommonTests() {
    std::cout << "\n=== COMMON TESTS: TIME, LEADERBOARD, GAME DATA ===\n" << std::endl;
    
    // Game Data
    safeExecute("Game Data", [&]() {
        auto gd = Games::getGameData<GameData>(*client);
        std::cout << "[GAME DATA] Retrieved global data" << std::endl;
        std::cout << "[GAME DATA] Event: " << gd.gameData.currentEvent << ", Version: " << gd.gameData.version << std::endl;
        
        GameData newGameData{"SpringFestival", "1.2.3"};
        Games::updateGameData(*client, newGameData);
        std::cout << "[GAME DATA] Global data updated" << std::endl;
    });
    
    // Time API
    safeExecute("GetServerTime", [&]() {
        auto time = Time::getServerTime(*client);
        std::cout << "[TIME] Server UTC: " << time.utc << std::endl;
    });
    
    safeExecute("GetServerTimeWithOffset", [&]() {
        auto timeOffset = Time::getServerTimeWithOffset(*client, 3);
        std::cout << "[TIME] Server UTC+3: " << timeOffset.utc << std::endl;
    });
    
    // Leaderboard
    safeExecute("GetLeaderboard", [&]() {
        auto lb = Leaderboard::getLeaderboard<PlayerData>(*client, {"level", "wins"}, 10);
        std::cout << "[LEADERBOARD] Top " << lb.leaderboard.size() << " players loaded" << std::endl;
        if (!lb.leaderboard.empty()) {
            std::cout << "[LEADERBOARD] #1: " << lb.leaderboard[0].playerName 
                      << ", Level: " << lb.leaderboard[0].playerData.level << std::endl;
        }
    });
    
    std::cout << std::endl;
}

// ====================== SETUP & CLEANUP ======================

void setupPlayers() {
    std::cout << "[SETUP] Registering players..." << std::endl;
    
    // Register host
    auto h = michitai::multiplayer::players::Players::registerPlayer(*client, "GameHost", std::optional<nlohmann::json>(nlohmann::json{{"level", 15}, {"rank", "gold"}}));
    std::cout << "[REGISTER] GameHost registered" << std::endl;
    playerMap["host"] = PlayerInfo{h.playerId, h.privateKey, "GameHost"};
    
    // Register p1
    auto p1 = michitai::multiplayer::players::Players::registerPlayer(*client, "PlayerOne", std::optional<nlohmann::json>(nlohmann::json{{"level", 12}, {"rank", "silver"}}));
    std::cout << "[REGISTER] PlayerOne registered" << std::endl;
    playerMap["p1"] = PlayerInfo{p1.playerId, p1.privateKey, "PlayerOne"};
    
    // Register p2
    auto p2 = michitai::multiplayer::players::Players::registerPlayer(*client, "PlayerTwo", std::optional<nlohmann::json>(nlohmann::json{{"level", 10}, {"rank", "bronze"}}));
    std::cout << "[REGISTER] PlayerTwo registered" << std::endl;
    playerMap["p2"] = PlayerInfo{p2.playerId, p2.privateKey, "PlayerTwo"};
    
    // Authenticate all players
    for (auto& [key, p] : playerMap) {
        auto auth = michitai::multiplayer::players::Players::authenticatePlayer<PlayerData>(*client, p.token);
        if (auth.player.has_value()) {
            std::cout << "[AUTH] " << auth.player->playerName << " authenticated" << std::endl;
        }
    }
    
    // Send heartbeat for all players
    for (auto& [key, p] : playerMap) {
        michitai::multiplayer::players::Players::sendPlayerHeartbeat(*client, p.token);
        std::cout << "[HEARTBEAT] Player heartbeat sent" << std::endl;
    }
    
    // Get all players list
    safeExecute("GetAllPlayersList", [&]() {
        auto list = Games::getAllPlayers(*client);
        std::cout << "[PLAYERS LIST] Total: " << list.count << std::endl;
        for (const auto& player : list.players) {
            std::cout << "[PLAYERS LIST] Id: " << player.id << ", Name: " << player.playerName 
                      << ", Online: " << player.isOnline << std::endl;
        }
    });
    
    // Get and update player data
    for (auto& [key, p] : playerMap) {
        safeExecute("GetPlayerData " + p.name, [&]() {
            auto data = michitai::multiplayer::players::Players::getPlayerData<PlayerData>(*client, p.token);
            std::cout << "[PLAYER DATA] Player data retrieved" << std::endl;
            
            PlayerData pd = data.playerData;
            pd.level++;
            michitai::multiplayer::players::Players::updatePlayerData(*client, p.token, pd);
            std::cout << "[PLAYER DATA] Player data updated" << std::endl;
        });
    }
}

void cleanupEverything() {
    std::cout << "\n[CLEANUP] Final cleanup..." << std::endl;
    
    for (auto& [key, p] : playerMap) {
        safeExecute("Logout " + p.name, [&]() {
            michitai::multiplayer::players::Players::logoutPlayer(*client, p.token);
        });
    }
    
    playerMap.clear();
}

// ====================== DEMO 1: MATCHMAKING WITH JOIN REQUESTS ======================

void runDemoWithJoinByRequests() {
    std::cout << "\n=== DEMO 1: MATCHMAKING WITH JOIN REQUESTS ===\n" << std::endl;
    setupPlayers();
    
    // Create matchmaking lobby with join-by-requests
    RulesData rules{"tdm", "arena"};
    PlayerData playerData{3, "Diamond"};
    
    auto createRes = Requests::createMatchmakingLobby<PlayerData, RulesData>(
        *client, playerMap["host"].token, "DEMO 1 Matchmaking", 4, false, false, false, false,
        std::nullopt, playerData, rules);
    std::string matchmakingId = createRes.matchmakingId;
    std::cout << "[MATCHMAKING] Lobby created (requests=true)" << std::endl;
    
    // Request to join
    auto req1 = Requests::requestToJoinMatchmaking<PlayerData>(
        *client, playerMap["p1"].token, matchmakingId, std::nullopt);
    std::cout << "[REQUEST] Sent: " << req1.requestId << std::endl;
    
    auto status1 = Requests::checkJoinRequestStatus(*client, playerMap["p1"].token, req1.requestId);
    std::cout << "[REQUEST STATUS] " << status1.request.dump() << std::endl;
    
    auto approve1 = Requests::respondToJoinRequest(
        *client, playerMap["host"].token, req1.requestId, michitai::multiplayer::matchmaking::requests::MatchmakingRequestAction::Approve);
    std::cout << "[APPROVE] " << approve1.message << std::endl;
    
    // Second player with data
    PlayerData p2Data;
    auto req2 = Requests::requestToJoinMatchmaking<PlayerData>(
        *client, playerMap["p2"].token, matchmakingId, p2Data);
    std::cout << "[REQUEST] Sent: " << req2.requestId << std::endl;
    
    auto status2 = Requests::checkJoinRequestStatus(*client, playerMap["p2"].token, req2.requestId);
    std::cout << "[REQUEST STATUS] " << status2.request.dump() << std::endl;
    
    // Get current status
    auto currentStatus = Matchmaking::getCurrentMatchmakingStatus<RulesData>(*client, playerMap["host"].token);
    if (currentStatus.matchmaking.has_value()) {
        std::cout << "[MATCHMAKING STATUS] Players: " << currentStatus.matchmaking->currentPlayers << std::endl;
    }
    
    // Approve second player
    auto approve2 = Requests::respondToJoinRequest(
        *client, playerMap["host"].token, req2.requestId, michitai::multiplayer::matchmaking::requests::MatchmakingRequestAction::Approve);
    std::cout << "[APPROVE] " << approve2.message << std::endl;
    
    currentStatus = Matchmaking::getCurrentMatchmakingStatus<RulesData>(*client, playerMap["host"].token);
    if (currentStatus.matchmaking.has_value()) {
        std::cout << "[MATCHMAKING STATUS] Players: " << currentStatus.matchmaking->currentPlayers << std::endl;
    }
    
    auto playersList = Matchmaking::getMatchmakingPlayers<PlayerData>(*client, playerMap["host"].token);
    std::cout << "[MATCHMAKING PLAYERS] " << playersList.players.size() << " players" << std::endl;
    
    // Start matchmaking and create room
    auto start = Matchmaking::startGameFromMatchmaking(*client, playerMap["host"].token);
    std::string roomId = start.roomId;
    std::cout << "[START] Room created: " << roomId << std::endl;
    
    // Run game room flow
    runGameRoomFlow(roomId, true);
}

// ====================== DEMO 2: MATCHMAKING DIRECT JOIN ======================

void runDemoWithoutJoinByRequests() {
    std::cout << "\n=== DEMO 2: MATCHMAKING DIRECT JOIN ===\n" << std::endl;
    setupPlayers();
    
    // Create matchmaking lobby without join-by-requests
    RulesData rules{"tdm", "arena"};
    PlayerData playerData{3, "Diamond"};
    
    auto createRes = Matchmaking::createMatchmakingLobby<PlayerData, RulesData>(
        *client, playerMap["host"].token, "DEMO 2 Matchmaking", 4, false, false, false, false, false,
        std::nullopt, playerData, rules);
    std::string matchmakingId = createRes.matchmakingId;
    std::cout << "[MATCHMAKING] Lobby created (requests=false)" << std::endl;
    
    // Join directly
    for (auto& [key, p] : playerMap) {
        if (key == "host") continue;
        
        std::optional<PlayerData> pd = PlayerData{};
        Matchmaking::joinMatchmakingDirectly(*client, p.token, matchmakingId, pd);
        std::cout << "[JOIN] Player joined directly" << std::endl;
    }
    
    // Get status
    auto currentStatus = Matchmaking::getCurrentMatchmakingStatus<RulesData>(*client, playerMap["host"].token);
    if (currentStatus.matchmaking.has_value()) {
        std::cout << "[MATCHMAKING STATUS] Players: " << currentStatus.matchmaking->currentPlayers << std::endl;
    }
    
    auto playersList = Matchmaking::getMatchmakingPlayers<PlayerData>(*client, playerMap["host"].token);
    std::cout << "[MATCHMAKING PLAYERS] " << playersList.players.size() << " players" << std::endl;
    
    // Start matchmaking and create room
    auto start = Matchmaking::startGameFromMatchmaking(*client, playerMap["host"].token);
    std::string roomId = start.roomId;
    std::cout << "[START] Room created: " << roomId << std::endl;
    
    // Run game room flow
    runGameRoomFlow(roomId, true);
}

// ====================== DEMO 3: DIRECT ROOM ======================

void runDemoDirectRoom() {
    std::cout << "\n=== DEMO 3: DIRECT ROOM CREATION ===\n" << std::endl;
    setupPlayers();
    
    RulesData rules{"tdm", "arena"};
    PlayerData playerData{3, "Diamond"};
    
    auto create = Rooms::createRoom<PlayerData, RulesData>(
        *client, playerMap["host"].token, "Direct Battle Arena", 4, std::nullopt, false, false,
        playerData, rules);
    std::string roomId = create.roomId;
    std::cout << "[ROOM] Room created: " << roomId << std::endl;
    
    // Join room
    Rooms::joinRoom(*client, playerMap["p1"].token, roomId);
    std::cout << "[ROOM] PlayerOne joined room" << std::endl;
    
    Rooms::joinRoom(*client, playerMap["p2"].token, roomId);
    std::cout << "[ROOM] PlayerTwo joined room" << std::endl;
    
    // Run game room flow
    runGameRoomFlow(roomId, false);
}

// ====================== GAME ROOM FLOW ======================

void runGameRoomFlow(const std::string& roomId, bool isFromMatchmaking) {
    std::cout << "\n=== GAME ROOM FLOW ===\n" << std::endl;
    
    // Get rooms
    safeExecute("GetRooms", [&]() {
        auto rooms = Rooms::getRooms<RulesData>(*client);
        std::cout << "[ROOMS] Retrieved room list" << std::endl;
    });
    
    // Get current room
    auto room = Rooms::getCurrentRoom<RulesData>(*client, playerMap["host"].token);
    std::cout << "[ROOM] Current room: " << room.room.roomName << std::endl;
    
    // Submit actions
    for (auto& [key, p] : playerMap) {
        safeExecute("SubmitAction " + p.name, [&]() {
            ActionData ad{true};
            SubmitAction<ActionData> action{RoomTargetPlayers::Host, "player_ready", ad, {}};
            Actions::submitAction(*client, p.token, action);
        });
    }
    
    // Get pending actions
    safeExecute("GetPendingActions", [&]() {
        auto pending = Actions::getPendingActions<ActionData>(*client, playerMap["host"].token);
        std::cout << "[PENDING ACTIONS] " << pending.pendingActions.size() << " actions" << std::endl;
    });
    
    // Send room update
    safeExecute("Send Room Update", [&]() {
        UpdateData ud{1, "Game Started!"};
        UpdatePlayers<UpdateData> update{RoomTargetPlayers::All, "game_start", ud, {}};
        Updates::updatePlayers(*client, playerMap["host"].token, update);
    });
    
    // Poll updates
    for (auto& [key, p] : playerMap) {
        safeExecute("PollUpdates " + p.name, [&]() {
            PollUpdates poll{RoomTargetPlayers::Host, {}, std::nullopt};
            Updates::pollUpdates<UpdateData>(*client, p.token, poll);
        });
    }
    
    // Get room players
    safeExecute("GetRoomPlayers", [&]() {
        auto roomPlayers = Rooms::getRoomPlayers<PlayerData>(*client, playerMap["host"].token);
        std::cout << "[ROOM PLAYERS] " << roomPlayers.players.size() << " players" << std::endl;
    });
    
    // Send heartbeat
    for (auto& [key, p] : playerMap) {
        safeExecute("RoomHeartbeat " + p.name, [&]() {
            Rooms::sendRoomHeartbeat(*client, p.token);
        });
    }
    
    // Stop room
    safeExecute("StopRoom", [&]() {
        Rooms::stopRoom(*client, playerMap["host"].token);
    });
}

// ====================== MAIN ======================

int main() {
    std::cout << "=== MICHITAI Game SDK - ALL THREE DEMOS + TIME + LEADERBOARD ===\n" << std::endl;
    
    auto logger = std::make_shared<ConsoleLogger>();
    client = std::make_unique<Client>("YOUR_API_TOKEN", "YOUR_PRIVATE_TOKEN", "https://api.michitai.com/api", logger);
    
    std::cout << "[INIT] SDK initialized successfully\n" << std::endl;
    
    try {
        // Three main demos
        runDemoWithJoinByRequests();
        cleanupEverything();
        
        runDemoWithoutJoinByRequests();
        cleanupEverything();
        
        runDemoDirectRoom();
        cleanupEverything();
        
        // Common tests
        runCommonTests();
    } catch (const std::exception& ex) {
        std::cout << "[FATAL] Unexpected error: " << ex.what() << std::endl;
    }
    
    std::cout << "\n=== All Demos Finished - All Endpoints Covered ===" << std::endl;
    
    std::cout << "\nPress Enter to exit...";
    std::cin.get();
    
    return 0;
}
