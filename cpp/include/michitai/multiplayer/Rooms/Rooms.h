#pragma once

#include "../Client.h"
#include "../ApiResponse.h"
#include "../Types.h"
#include <string>
#include <vector>
#include <optional>

namespace michitai {
namespace multiplayer {
namespace rooms {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* GameRoomCreate = "game_room.php/create";
    constexpr const char* GameRoomList = "game_room.php/list";
    constexpr const char* GameRoomJoin = "game_room.php/{0}/join";
    constexpr const char* GameRoomLeave = "game_room.php/leave";
    constexpr const char* GameRoomPlayers = "game_room.php/players";
    constexpr const char* GameRoomHeartbeat = "game_room.php/heartbeat";
    constexpr const char* GameRoomCurrent = "game_room.php/current";
    constexpr const char* GameRoomStop = "game_room.php/stop";
    constexpr const char* GameRoomKick = "game_room.php/kick";
    constexpr const char* GameRoomPassword = "game_room.php/password";
}

// ====================== ROOM TYPES ======================

/// Minimal information about a game room
template<typename T = nlohmann::json>
struct RoomShort {
    std::string roomId;
    std::string roomName;
    int maxPlayers = 0;
    int currentPlayers = 0;
    bool hasPassword = false;
    bool hostSwitch = false;
    bool canLeave = true;
    bool realtime = false;
    T rules;
    
    static RoomShort fromJson(const nlohmann::json& j) {
        RoomShort room;
        room.roomId = j.value("room_id", "");
        room.roomName = j.value("room_name", "");
        room.maxPlayers = j.value("max_players", 0);
        room.currentPlayers = j.value("current_players", 0);
        room.hasPassword = j.value("has_password", false);
        room.hostSwitch = j.value("host_switch", false);
        room.canLeave = j.value("can_leave", true);
        room.realtime = j.value("realtime", false);
        
        if (j.contains("rules")) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                room.rules = j["rules"];
            } else {
                room.rules = j["rules"].get<T>();
            }
        }
        
        return room;
    }
};

/// Information about a player in a game room
template<typename T = nlohmann::json>
struct RoomPlayer {
    int playerId = 0;
    bool isLocal = false;
    std::string playerName;
    bool isHost = false;
    bool isOnline = false;
    std::string lastHeartbeat;
    T playerData;
    
    static RoomPlayer fromJson(const nlohmann::json& j) {
        RoomPlayer player;
        player.playerId = j.value("player_id", 0);
        player.isLocal = j.value("is_local", false);
        player.playerName = j.value("player_name", "");
        player.isHost = j.value("is_host", false);
        player.isOnline = j.value("is_online", false);
        player.lastHeartbeat = j.value("last_heartbeat", "");
        
        if (j.contains("player_data") && !j["player_data"].is_null()) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                player.playerData = j["player_data"];
            } else {
                player.playerData = j["player_data"].get<T>();
            }
        }
        
        return player;
    }
};

// ====================== RESPONSE TYPES ======================

/// Response for room creation
struct RoomCreateResponse : public ApiResponse {
    std::string roomId;
    
    static RoomCreateResponse fromJson(const nlohmann::json& j) {
        RoomCreateResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.roomId = j.value("room_id", "");
        return response;
    }
};

/// Response for room list
template<typename T = nlohmann::json>
struct RoomListResponse : public ApiResponse {
    std::vector<RoomShort<T>> rooms;
    
    static RoomListResponse fromJson(const nlohmann::json& j) {
        RoomListResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        
        if (j.contains("rooms") && j["rooms"].is_array()) {
            for (const auto& roomJson : j["rooms"]) {
                response.rooms.push_back(RoomShort<T>::fromJson(roomJson));
            }
        }
        
        return response;
    }
};

/// Response for room join
struct RoomJoinResponse : public ApiResponse {
    static RoomJoinResponse fromJson(const nlohmann::json& j) {
        RoomJoinResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for room leave
struct RoomLeaveResponse : public ApiResponse {
    static RoomLeaveResponse fromJson(const nlohmann::json& j) {
        RoomLeaveResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for room players
template<typename T = nlohmann::json>
struct RoomPlayersResponse : public ApiResponse {
    std::vector<RoomPlayer<T>> players;
    
    static RoomPlayersResponse fromJson(const nlohmann::json& j) {
        RoomPlayersResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        
        if (j.contains("players") && j["players"].is_array()) {
            for (const auto& playerJson : j["players"]) {
                response.players.push_back(RoomPlayer<T>::fromJson(playerJson));
            }
        }
        
        return response;
    }
};

/// Response for heartbeat
struct HeartbeatResponse : public ApiResponse {
    static HeartbeatResponse fromJson(const nlohmann::json& j) {
        HeartbeatResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for current room
template<typename T = nlohmann::json>
struct CurrentRoomResponse : public ApiResponse {
    RoomShort<T> room;
    std::vector<RoomPlayer<T>> players;
    
    static CurrentRoomResponse fromJson(const nlohmann::json& j) {
        CurrentRoomResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        
        if (j.contains("room")) {
            response.room = RoomShort<T>::fromJson(j["room"]);
        }
        
        if (j.contains("players") && j["players"].is_array()) {
            for (const auto& playerJson : j["players"]) {
                response.players.push_back(RoomPlayer<T>::fromJson(playerJson));
            }
        }
        
        return response;
    }
};

/// Response for room kick
struct RoomKickResponse : public ApiResponse {
    static RoomKickResponse fromJson(const nlohmann::json& j) {
        RoomKickResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

// ====================== REQUEST TYPES ======================

/// Request for room creation
template<typename TPlayerData = nlohmann::json, typename TRules = nlohmann::json>
struct RoomCreateRequest {
    std::string roomName;
    int maxPlayers = 4;
    std::optional<std::string> password;
    bool hostSwitch = false;
    bool realtime = false;
    std::optional<TPlayerData> playerData;
    std::optional<TRules> rules;
    
    RoomCreateRequest(const std::string& name, int max, const std::optional<std::string>& pwd,
                     bool hostSw, bool real, const std::optional<TPlayerData>& pData,
                     const std::optional<TRules>& rls)
        : roomName(name), maxPlayers(max), password(pwd), hostSwitch(hostSw),
          realtime(real), playerData(pData), rules(rls) {}
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        j["room_name"] = roomName;
        j["max_players"] = maxPlayers;
        j["host_switch"] = hostSwitch;
        j["realtime"] = realtime;
        
        if (password.has_value()) {
            j["password"] = password.value();
        }
        
        if (playerData.has_value()) {
            if constexpr (std::is_same_v<TPlayerData, nlohmann::json>) {
                j["player_data"] = playerData.value();
            } else {
                j["player_data"] = nlohmann::json(playerData.value());
            }
        }
        
        if (rules.has_value()) {
            if constexpr (std::is_same_v<TRules, nlohmann::json>) {
                j["rules"] = rules.value();
            } else {
                j["rules"] = nlohmann::json(rules.value());
            }
        }
        
        return j;
    }
};

/// Request for room list
struct RoomListRequest {
    std::optional<std::string> search;
    std::optional<int> limit;
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        if (search.has_value()) {
            j["search"] = search.value();
        }
        if (limit.has_value()) {
            j["limit"] = limit.value();
        }
        return j;
    }
};

/// Request for room join
template<typename T = nlohmann::json>
struct RoomJoinRequest {
    std::optional<std::string> password;
    std::optional<T> playerData;
    
    RoomJoinRequest(const std::optional<std::string>& pwd, const std::optional<T>& pData)
        : password(pwd), playerData(pData) {}
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        if (password.has_value()) {
            j["password"] = password.value();
        }
        if (playerData.has_value()) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                j["player_data"] = playerData.value();
            } else {
                j["player_data"] = nlohmann::json(playerData.value());
            }
        }
        return j;
    }
};

/// Request for room kick
struct RoomKickRequest {
    int playerId;
    
    explicit RoomKickRequest(int id) : playerId(id) {}
    
    nlohmann::json toJson() const {
        return {{"player_id", playerId}};
    }
};

/// Request for room password update
struct RoomPasswordUpdateRequest {
    std::optional<std::string> password;
    
    explicit RoomPasswordUpdateRequest(const std::optional<std::string>& pwd) : password(pwd) {}
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        if (password.has_value()) {
            j["password"] = password.value();
        } else {
            j["password"] = "";
        }
        return j;
    }
};

// ====================== ROOMS CLASS ======================

/// Provides methods for game room management
class Rooms {
public:
    /// Creates a new game room with specified configuration.
    template<typename TPlayerData = nlohmann::json, typename TRules = nlohmann::json>
    static RoomCreateResponse createRoom(Client& client,
                                          const std::string& playerToken,
                                          const std::string& roomName,
                                          int maxPlayers = 4,
                                          const std::optional<std::string>& password = std::nullopt,
                                          bool hostSwitch = false,
                                          bool realtime = false,
                                          const std::optional<TPlayerData>& playerData = std::nullopt,
                                          const std::optional<TRules>& rules = std::nullopt) {
        RoomCreateRequest<TPlayerData, TRules> request(roomName, maxPlayers, password,
                                                       hostSwitch, realtime, playerData, rules);
        return client.post<RoomCreateResponse>(
            client.url(Endpoints::GameRoomCreate, "&player_token=" + playerToken),
            request.toJson()
        );
    }
    
    /// Retrieves a list of available game rooms.
    template<typename T = nlohmann::json>
    static RoomListResponse<T> getRooms(Client& client,
                                         const std::optional<std::string>& search = std::nullopt,
                                         const std::optional<int>& limit = std::nullopt) {
        RoomListRequest request{search, limit};
        return client.post<RoomListResponse<T>>(
            client.url(Endpoints::GameRoomList),
            request.toJson()
        );
    }
    
    /// Joins an existing game room.
    template<typename T = nlohmann::json>
    static RoomJoinResponse joinRoom(Client& client,
                                      const std::string& playerToken,
                                      const std::string& roomId,
                                      const std::optional<std::string>& password = std::nullopt,
                                      const std::optional<T>& playerData = std::nullopt) {
        std::string endpoint = std::string(Endpoints::GameRoomJoin);
        size_t pos = endpoint.find("{0}");
        if (pos != std::string::npos) {
            endpoint.replace(pos, 3, roomId);
        }
        
        bool hasBody = password.has_value() || playerData.has_value();
        if (hasBody) {
            RoomJoinRequest<T> request(password, playerData);
            return client.post<RoomJoinResponse>(
                client.url(endpoint, "&player_token=" + playerToken),
                request.toJson()
            );
        } else {
            return client.post<RoomJoinResponse>(
                client.url(endpoint, "&player_token=" + playerToken),
                nlohmann::json{}
            );
        }
    }
    
    /// Leaves the current game room.
    static RoomLeaveResponse leaveRoom(Client& client,
                                        const std::string& playerToken) {
        return client.post<RoomLeaveResponse>(
            client.url(Endpoints::GameRoomLeave, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Gets the list of players in the current game room.
    template<typename T = nlohmann::json>
    static RoomPlayersResponse<T> getRoomPlayers(Client& client,
                                                  const std::string& playerToken) {
        return client.get<RoomPlayersResponse<T>>(
            client.url(Endpoints::GameRoomPlayers, "&player_token=" + playerToken)
        );
    }
    
    /// Sends a heartbeat to maintain the player's presence in the game room.
    static HeartbeatResponse sendRoomHeartbeat(Client& client,
                                               const std::string& playerToken) {
        return client.post<HeartbeatResponse>(
            client.url(Endpoints::GameRoomHeartbeat, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Gets comprehensive information about the current game room.
    template<typename T = nlohmann::json>
    static CurrentRoomResponse<T> getCurrentRoom(Client& client,
                                                  const std::string& playerToken) {
        return client.get<CurrentRoomResponse<T>>(
            client.url(Endpoints::GameRoomCurrent, "&player_token=" + playerToken)
        );
    }
    
    /// Stops the current game room and removes all associated data (host only).
    static SuccessResponse stopRoom(Client& client,
                                    const std::string& playerToken) {
        return client.post<SuccessResponse>(
            client.url(Endpoints::GameRoomStop, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Kicks a player from the game room (host only).
    static RoomKickResponse kickPlayer(Client& client,
                                       const std::string& playerToken,
                                       int playerId) {
        RoomKickRequest request(playerId);
        return client.post<RoomKickResponse>(
            client.url(Endpoints::GameRoomKick, "&player_token=" + playerToken),
            request.toJson()
        );
    }
    
    /// Updates the password for the game room (host only).
    static SuccessResponse updateRoomPassword(Client& client,
                                              const std::string& playerToken,
                                              const std::optional<std::string>& password = std::nullopt) {
        RoomPasswordUpdateRequest request(password);
        return client.post<SuccessResponse>(
            client.url(Endpoints::GameRoomPassword, "&player_token=" + playerToken),
            request.toJson()
        );
    }
};

} // namespace rooms
} // namespace multiplayer
} // namespace michitai
