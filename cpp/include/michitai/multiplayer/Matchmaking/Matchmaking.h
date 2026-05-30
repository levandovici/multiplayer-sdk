#pragma once

#include "../Client.h"
#include "../ApiResponse.h"
#include "../Types.h"
#include <string>
#include <vector>
#include <optional>

namespace michitai {
namespace multiplayer {
namespace matchmaking {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* MatchmakingList = "matchmaking.php/list";
    constexpr const char* MatchmakingCreate = "matchmaking.php/create";
    constexpr const char* MatchmakingCurrent = "matchmaking.php/current";
    constexpr const char* MatchmakingJoin = "matchmaking.php/{0}/join";
    constexpr const char* MatchmakingLeave = "matchmaking.php/leave";
    constexpr const char* MatchmakingPlayers = "matchmaking.php/players";
    constexpr const char* MatchmakingHeartbeat = "matchmaking.php/heartbeat";
    constexpr const char* MatchmakingRemove = "matchmaking.php/remove";
    constexpr const char* MatchmakingStart = "matchmaking.php/start";
    constexpr const char* MatchmakingStop = "matchmaking.php/stop";
    constexpr const char* MatchmakingKick = "matchmaking.php/kick";
    constexpr const char* MatchmakingPassword = "matchmaking.php/password";
}

// ====================== MATCHMAKING TYPES ======================

/// Matchmaking lobby information
template<typename T = nlohmann::json>
struct MatchmakingLobby {
    std::string matchmakingId;
    std::string matchmakingName;
    int hostPlayerId = 0;
    int maxPlayers = 0;
    int currentPlayers = 0;
    bool strictFull = false;
    bool joinByRequests = false;
    bool hostSwitch = false;
    bool canLeaveRoom = false;
    bool realtimeRoom = false;
    bool hasPassword = false;
    T rules;
    std::string createdAt;
    std::string lastHeartbeat;
    bool isStarted = false;
    
    static MatchmakingLobby fromJson(const nlohmann::json& j) {
        MatchmakingLobby lobby;
        lobby.matchmakingId = j.value("matchmaking_id", "");
        lobby.matchmakingName = j.value("matchmaking_name", "");
        lobby.hostPlayerId = j.value("host_player_id", 0);
        lobby.maxPlayers = j.value("max_players", 0);
        lobby.currentPlayers = j.value("current_players", 0);
        lobby.strictFull = j.value("strict_full", false);
        lobby.joinByRequests = j.value("join_by_requests", false);
        lobby.hostSwitch = j.value("host_switch", false);
        lobby.canLeaveRoom = j.value("can_leave_room", false);
        lobby.realtimeRoom = j.value("realtime_room", false);
        lobby.hasPassword = j.value("has_password", false);
        lobby.createdAt = j.value("created_at", "");
        lobby.lastHeartbeat = j.value("last_heartbeat", "");
        lobby.isStarted = j.value("is_started", false);
        
        if (j.contains("rules")) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                lobby.rules = j["rules"];
            } else {
                lobby.rules = j["rules"].get<T>();
            }
        }
        
        return lobby;
    }
};

/// Player in matchmaking lobby
template<typename T = nlohmann::json>
struct MatchmakingPlayer {
    int playerId = 0;
    std::string playerName;
    bool isOnline = false;
    T playerData;
    std::string joinedAt;
    std::string lastHeartbeat;
    
    static MatchmakingPlayer fromJson(const nlohmann::json& j) {
        MatchmakingPlayer player;
        player.playerId = j.value("player_id", 0);
        player.playerName = j.value("player_name", "");
        player.isOnline = j.value("is_online", false);
        player.joinedAt = j.value("joined_at", "");
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

/// Response for matchmaking list
template<typename T = nlohmann::json>
struct MatchmakingListResponse : public ApiResponse {
    std::vector<MatchmakingLobby<T>> lobbies;
    
    static MatchmakingListResponse fromJson(const nlohmann::json& j) {
        MatchmakingListResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        
        if (j.contains("lobbies") && j["lobbies"].is_array()) {
            for (const auto& lobbyJson : j["lobbies"]) {
                response.lobbies.push_back(MatchmakingLobby<T>::fromJson(lobbyJson));
            }
        }
        
        return response;
    }
};

/// Response for matchmaking creation
struct MatchmakingCreateResponse : public ApiResponse {
    std::string matchmakingId;
    
    static MatchmakingCreateResponse fromJson(const nlohmann::json& j) {
        MatchmakingCreateResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.matchmakingId = j.value("matchmaking_id", "");
        return response;
    }
};

/// Response for current matchmaking status
template<typename T = nlohmann::json>
struct MatchmakingCurrentResponse : public ApiResponse {
    std::optional<MatchmakingLobby<T>> matchmaking;
    
    static MatchmakingCurrentResponse fromJson(const nlohmann::json& j) {
        MatchmakingCurrentResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        
        if (j.contains("matchmaking") && !j["matchmaking"].is_null()) {
            response.matchmaking = MatchmakingLobby<T>::fromJson(j["matchmaking"]);
        }
        
        return response;
    }
};

/// Response for direct join
struct MatchmakingDirectJoinResponse : public ApiResponse {
    static MatchmakingDirectJoinResponse fromJson(const nlohmann::json& j) {
        MatchmakingDirectJoinResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for leave matchmaking
struct MatchmakingLeaveResponse : public ApiResponse {
    static MatchmakingLeaveResponse fromJson(const nlohmann::json& j) {
        MatchmakingLeaveResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for matchmaking players
template<typename T = nlohmann::json>
struct MatchmakingPlayersResponse : public ApiResponse {
    std::vector<MatchmakingPlayer<T>> players;
    
    static MatchmakingPlayersResponse fromJson(const nlohmann::json& j) {
        MatchmakingPlayersResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        
        if (j.contains("players") && j["players"].is_array()) {
            for (const auto& playerJson : j["players"]) {
                response.players.push_back(MatchmakingPlayer<T>::fromJson(playerJson));
            }
        }
        
        return response;
    }
};

/// Response for heartbeat
struct MatchmakingHeartbeatResponse : public ApiResponse {
    static MatchmakingHeartbeatResponse fromJson(const nlohmann::json& j) {
        MatchmakingHeartbeatResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for remove matchmaking
struct MatchmakingRemoveResponse : public ApiResponse {
    static MatchmakingRemoveResponse fromJson(const nlohmann::json& j) {
        MatchmakingRemoveResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for start matchmaking
struct MatchmakingStartResponse : public ApiResponse {
    std::string roomId;
    
    static MatchmakingStartResponse fromJson(const nlohmann::json& j) {
        MatchmakingStartResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.roomId = j.value("room_id", "");
        return response;
    }
};

/// Response for kick player
struct MatchmakingKickResponse : public ApiResponse {
    static MatchmakingKickResponse fromJson(const nlohmann::json& j) {
        MatchmakingKickResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

// ====================== REQUEST TYPES ======================

/// Request for matchmaking list
struct MatchmakingListRequest {
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

/// Request for matchmaking creation
template<typename TPlayerData = nlohmann::json, typename TRules = nlohmann::json>
struct MatchmakingCreateRequest {
    std::string matchmakingName;
    int maxPlayers = 4;
    bool strictFull = false;
    bool joinByRequests = false;
    bool hostSwitch = false;
    bool canLeaveRoom = false;
    bool realtimeRoom = false;
    std::optional<std::string> password;
    std::optional<TPlayerData> playerData;
    std::optional<TRules> rules;
    
    MatchmakingCreateRequest(const std::string& name, int max, bool strict, bool joinReq,
                             bool hostSw, bool canLeave, bool realtime,
                             const std::optional<std::string>& pwd,
                             const std::optional<TPlayerData>& pData,
                             const std::optional<TRules>& rls)
        : matchmakingName(name), maxPlayers(max), strictFull(strict), joinByRequests(joinReq),
          hostSwitch(hostSw), canLeaveRoom(canLeave), realtimeRoom(realtime),
          password(pwd), playerData(pData), rules(rls) {}
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        j["matchmaking_name"] = matchmakingName;
        j["max_players"] = maxPlayers;
        j["strict_full"] = strictFull;
        j["join_by_requests"] = joinByRequests;
        j["host_switch"] = hostSwitch;
        j["can_leave_room"] = canLeaveRoom;
        j["realtime_room"] = realtimeRoom;
        
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

/// Request for kick player
struct MatchmakingKickRequest {
    int playerId;
    
    explicit MatchmakingKickRequest(int id) : playerId(id) {}
    
    nlohmann::json toJson() const {
        return {{"player_id", playerId}};
    }
};

/// Request for password update
struct MatchmakingPasswordUpdateRequest {
    std::optional<std::string> password;
    
    explicit MatchmakingPasswordUpdateRequest(const std::optional<std::string>& pwd) : password(pwd) {}
    
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

// ====================== MATCHMAKING CLASS ======================

/// Provides methods for matchmaking lobby management
class Matchmaking {
public:
    /// Retrieves a list of available matchmaking lobbies.
    template<typename T = nlohmann::json>
    static MatchmakingListResponse<T> getMatchmakingLobbies(
        Client& client,
        const std::optional<std::string>& search = std::nullopt,
        const std::optional<int>& limit = std::nullopt) {
        MatchmakingListRequest request{search, limit};
        return client.post<MatchmakingListResponse<T>>(
            client.url(Endpoints::MatchmakingList),
            request.toJson()
        );
    }
    
    /// Creates a new matchmaking lobby with specified configuration.
    template<typename TPlayerData = nlohmann::json, typename TRules = nlohmann::json>
    static MatchmakingCreateResponse createMatchmakingLobby(
        Client& client,
        const std::string& playerToken,
        const std::string& matchmakingName,
        int maxPlayers = 4,
        bool strictFull = false,
        bool joinByRequests = false,
        bool hostSwitch = false,
        bool canLeaveRoom = false,
        bool realtimeRoom = false,
        const std::optional<std::string>& password = std::nullopt,
        const std::optional<TPlayerData>& playerData = std::nullopt,
        const std::optional<TRules>& rules = std::nullopt) {
        MatchmakingCreateRequest<TPlayerData, TRules> request(
            matchmakingName, maxPlayers, strictFull, joinByRequests,
            hostSwitch, canLeaveRoom, realtimeRoom, password, playerData, rules);
        return client.post<MatchmakingCreateResponse>(
            client.url(Endpoints::MatchmakingCreate, "&player_token=" + playerToken),
            request.toJson()
        );
    }
    
    /// Gets the current status of the player's matchmaking lobby.
    template<typename T = nlohmann::json>
    static MatchmakingCurrentResponse<T> getCurrentMatchmakingStatus(
        Client& client,
        const std::string& playerToken) {
        return client.get<MatchmakingCurrentResponse<T>>(
            client.url(Endpoints::MatchmakingCurrent, "&player_token=" + playerToken)
        );
    }
    
    /// Joins a matchmaking lobby directly (without approval).
    template<typename T = nlohmann::json>
    static MatchmakingDirectJoinResponse joinMatchmakingDirectly(
        Client& client,
        const std::string& playerToken,
        const std::string& matchmakingId,
        const std::optional<T>& playerData = std::nullopt) {
        std::string endpoint = std::string(Endpoints::MatchmakingJoin);
        size_t pos = endpoint.find("{0}");
        if (pos != std::string::npos) {
            endpoint.replace(pos, 3, matchmakingId);
        }
        
        if (playerData.has_value()) {
            nlohmann::json jsonData;
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                jsonData = playerData.value();
            } else {
                jsonData = nlohmann::json(playerData.value());
            }
            return client.post<MatchmakingDirectJoinResponse>(
                client.url(endpoint, "&player_token=" + playerToken),
                jsonData
            );
        } else {
            return client.post<MatchmakingDirectJoinResponse>(
                client.url(endpoint, "&player_token=" + playerToken),
                nlohmann::json{}
            );
        }
    }
    
    /// Leaves the current matchmaking lobby.
    static MatchmakingLeaveResponse leaveMatchmaking(
        Client& client,
        const std::string& playerToken) {
        return client.post<MatchmakingLeaveResponse>(
            client.url(Endpoints::MatchmakingLeave, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Gets the list of players in the current matchmaking lobby.
    template<typename T = nlohmann::json>
    static MatchmakingPlayersResponse<T> getMatchmakingPlayers(
        Client& client,
        const std::string& playerToken) {
        return client.get<MatchmakingPlayersResponse<T>>(
            client.url(Endpoints::MatchmakingPlayers, "&player_token=" + playerToken)
        );
    }
    
    /// Sends a heartbeat to maintain the player's presence in the matchmaking lobby.
    static MatchmakingHeartbeatResponse sendMatchmakingHeartbeat(
        Client& client,
        const std::string& playerToken) {
        return client.post<MatchmakingHeartbeatResponse>(
            client.url(Endpoints::MatchmakingHeartbeat, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Removes the current matchmaking lobby (host only).
    static MatchmakingRemoveResponse removeMatchmakingLobby(
        Client& client,
        const std::string& playerToken) {
        return client.post<MatchmakingRemoveResponse>(
            client.url(Endpoints::MatchmakingRemove, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Starts the game from the current matchmaking lobby and creates a game room.
    static MatchmakingStartResponse startGameFromMatchmaking(
        Client& client,
        const std::string& playerToken) {
        return client.post<MatchmakingStartResponse>(
            client.url(Endpoints::MatchmakingStart, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Stops the current matchmaking lobby (host only).
    static SuccessResponse stopMatchmaking(
        Client& client,
        const std::string& playerToken) {
        return client.post<SuccessResponse>(
            client.url(Endpoints::MatchmakingStop, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Kicks a player from the matchmaking lobby (host only).
    static MatchmakingKickResponse kickPlayer(
        Client& client,
        const std::string& playerToken,
        int playerId) {
        MatchmakingKickRequest request(playerId);
        return client.post<MatchmakingKickResponse>(
            client.url(Endpoints::MatchmakingKick, "&player_token=" + playerToken),
            request.toJson()
        );
    }
    
    /// Updates the password for the matchmaking lobby (host only).
    static SuccessResponse updateMatchmakingPassword(
        Client& client,
        const std::string& playerToken,
        const std::optional<std::string>& password = std::nullopt) {
        MatchmakingPasswordUpdateRequest request(password);
        return client.post<SuccessResponse>(
            client.url(Endpoints::MatchmakingPassword, "&player_token=" + playerToken),
            request.toJson()
        );
    }
};

} // namespace matchmaking
} // namespace multiplayer
} // namespace michitai
