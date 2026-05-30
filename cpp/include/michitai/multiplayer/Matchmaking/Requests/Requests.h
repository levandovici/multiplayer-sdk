#pragma once

#include "../../Client.h"
#include "../../ApiResponse.h"
#include "../../Types.h"
#include <string>

namespace michitai {
namespace multiplayer {
namespace matchmaking {
namespace requests {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* MatchmakingCreate = "matchmaking.php/create";
    constexpr const char* MatchmakingRequest = "matchmaking.php/{0}/request";
    constexpr const char* MatchmakingResponse = "matchmaking.php/{0}/response";
    constexpr const char* MatchmakingRequestStatus = "matchmaking.php/{0}/status";
}

// ====================== ENUMS ======================

/// Actions for responding to matchmaking join requests
enum class MatchmakingRequestAction {
    Approve,
    Reject
};

/// Convert MatchmakingRequestAction to string
inline std::string matchmakingRequestActionToString(MatchmakingRequestAction action) {
    switch (action) {
        case MatchmakingRequestAction::Approve: return "approve";
        case MatchmakingRequestAction::Reject: return "reject";
        default: return "approve";
    }
}

// ====================== REQUEST TYPES ======================

/// Request for creating a matchmaking lobby with join-by-requests
template<typename TPlayerData = nlohmann::json, typename TRules = nlohmann::json>
struct MatchmakingCreateRequest {
    std::string matchmakingName;
    int maxPlayers = 4;
    bool strictFull = false;
    bool joinByRequests = true;
    bool hostSwitch = false;
    bool canLeaveRoom = false;
    bool realtimeRoom = false;
    std::optional<std::string> password;
    std::optional<TPlayerData> playerData;
    std::optional<TRules> rules;
    
    MatchmakingCreateRequest(const std::string& name, int max, bool strict, bool joinByReq,
                           bool hostSw, bool canLeave, bool real, const std::optional<std::string>& pwd,
                           const std::optional<TPlayerData>& pData, const std::optional<TRules>& rls)
        : matchmakingName(name), maxPlayers(max), strictFull(strict), joinByRequests(joinByReq),
          hostSwitch(hostSw), canLeaveRoom(canLeave), realtimeRoom(real), password(pwd),
          playerData(pData), rules(rls) {}
    
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

/// Request for responding to a matchmaking join request
struct MatchmakingPermissionRequest {
    MatchmakingRequestAction action;
    
    explicit MatchmakingPermissionRequest(MatchmakingRequestAction act) : action(act) {}
    
    nlohmann::json toJson() const {
        return {{"action", matchmakingRequestActionToString(action)}};
    }
};

// ====================== RESPONSE TYPES ======================

/// Response returned when a player requests to join a matchmaking lobby
struct MatchmakingJoinRequestResponse : public ApiResponse {
    std::string requestId;
    std::string message;
    
    static MatchmakingJoinRequestResponse fromJson(const nlohmann::json& j) {
        MatchmakingJoinRequestResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.requestId = j.value("request_id", "");
        response.message = j.value("message", "");
        return response;
    }
};

/// Response returned when a host responds to a join request (approve/reject)
struct MatchmakingPermissionResponse : public ApiResponse {
    std::string message;
    std::string requestId;
    std::string action;
    
    static MatchmakingPermissionResponse fromJson(const nlohmann::json& j) {
        MatchmakingPermissionResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.message = j.value("message", "");
        response.requestId = j.value("request_id", "");
        response.action = j.value("action", "");
        return response;
    }
};

/// Response containing the status of a specific join request
struct MatchmakingRequestStatusResponse : public ApiResponse {
    nlohmann::json request;
    
    static MatchmakingRequestStatusResponse fromJson(const nlohmann::json& j) {
        MatchmakingRequestStatusResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.request = j.value("request", nlohmann::json::object());
        return response;
    }
};

// ====================== REQUESTS CLASS ======================

/// Provides methods for managing matchmaking join requests
class Requests {
public:
    /// Creates a new matchmaking lobby with the specified configuration for join-by-requests.
    template<typename TPlayerData = nlohmann::json, typename TRules = nlohmann::json>
    static MatchmakingCreateResponse createMatchmakingLobby(
        Client& client,
        const std::string& playerToken,
        const std::string& matchmakingName,
        int maxPlayers = 4,
        bool strictFull = false,
        bool hostSwitch = false,
        bool canLeaveRoom = false,
        bool realtimeRoom = false,
        const std::optional<std::string>& password = std::nullopt,
        const std::optional<TPlayerData>& playerData = std::nullopt,
        const std::optional<TRules>& rules = std::nullopt) {
        MatchmakingCreateRequest<TPlayerData, TRules> request(matchmakingName, maxPlayers, strictFull, true,
                                                               hostSwitch, canLeaveRoom, realtimeRoom,
                                                               password, playerData, rules);
        return client.post<MatchmakingCreateResponse>(
            client.url(Endpoints::MatchmakingCreate, "&player_token=" + playerToken),
            request.toJson()
        );
    }
    
    /// Requests to join an existing matchmaking lobby.
    template<typename T = nlohmann::json>
    static MatchmakingJoinRequestResponse requestToJoinMatchmaking(
        Client& client,
        const std::string& playerToken,
        const std::string& matchmakingId,
        const std::optional<T>& playerData = std::nullopt) {
        std::string endpoint = std::string(Endpoints::MatchmakingRequest);
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
            return client.post<MatchmakingJoinRequestResponse>(
                client.url(endpoint, "&player_token=" + playerToken),
                jsonData
            );
        } else {
            return client.post<MatchmakingJoinRequestResponse>(
                client.url(endpoint, "&player_token=" + playerToken),
                nlohmann::json{}
            );
        }
    }
    
    /// Responds to a pending join request (approve or reject).
    static MatchmakingPermissionResponse respondToJoinRequest(
        Client& client,
        const std::string& playerToken,
        const std::string& requestId,
        MatchmakingRequestAction action) {
        MatchmakingPermissionRequest request(action);
        std::string endpoint = std::string(Endpoints::MatchmakingResponse);
        size_t pos = endpoint.find("{0}");
        if (pos != std::string::npos) {
            endpoint.replace(pos, 3, requestId);
        }
        return client.post<MatchmakingPermissionResponse>(
            client.url(endpoint, "&player_token=" + playerToken),
            request.toJson()
        );
    }
    
    /// Checks the status of a specific join request.
    static MatchmakingRequestStatusResponse checkJoinRequestStatus(
        Client& client,
        const std::string& playerToken,
        const std::string& requestId) {
        std::string endpoint = std::string(Endpoints::MatchmakingRequestStatus);
        size_t pos = endpoint.find("{0}");
        if (pos != std::string::npos) {
            endpoint.replace(pos, 3, requestId);
        }
        return client.get<MatchmakingRequestStatusResponse>(
            client.url(endpoint, "&player_token=" + playerToken)
        );
    }
};

} // namespace requests
} // namespace matchmaking
} // namespace multiplayer
} // namespace michitai
