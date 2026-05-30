#pragma once

#include "../../Client.h"
#include "../../ApiResponse.h"
#include "../../Types.h"
#include <string>
#include <vector>
#include <optional>

namespace michitai {
namespace multiplayer {
namespace rooms {
namespace actions {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* GameRoomActions = "game_room.php/actions";
    constexpr const char* GameRoomActionsPoll = "game_room.php/actions/poll";
    constexpr const char* GameRoomActionsPending = "game_room.php/actions/pending";
    constexpr const char* GameRoomActionComplete = "game_room.php/actions/{0}/complete";
}

// ====================== ENUMS ======================

/// Status of a room action
enum class RoomActionStatus {
    Pending,
    Processing,
    Completed,
    Failed,
    Read
};

/// Status for completing a room action
enum class RoomCompleteActionStatus {
    Processing,
    Completed,
    Failed
};

// ====================== ACTION TYPES ======================

/// Action information
template<typename T = nlohmann::json>
struct ActionInfo {
    int actionId = 0;
    int senderPlayerId = 0;
    std::string senderPlayerName;
    RoomActionStatus status;
    std::string actionType;
    T requestData;
    std::string responseData;
    std::string createdAt;
    std::string updatedAt;
    
    static ActionInfo fromJson(const nlohmann::json& j) {
        ActionInfo info;
        info.actionId = j.value("action_id", 0);
        info.senderPlayerId = j.value("sender_player_id", 0);
        info.senderPlayerName = j.value("sender_player_name", "");
        
        std::string statusStr = j.value("status", "pending");
        if (statusStr == "pending") info.status = RoomActionStatus::Pending;
        else if (statusStr == "processing") info.status = RoomActionStatus::Processing;
        else if (statusStr == "completed") info.status = RoomActionStatus::Completed;
        else if (statusStr == "failed") info.status = RoomActionStatus::Failed;
        else if (statusStr == "read") info.status = RoomActionStatus::Read;
        
        info.actionType = j.value("action_type", "");
        info.createdAt = j.value("created_at", "");
        info.updatedAt = j.value("updated_at", "");
        
        if (j.contains("request_data")) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                info.requestData = j["request_data"];
            } else {
                info.requestData = j["request_data"].get<T>();
            }
        }
        
        info.responseData = j.value("response_data", "");
        
        return info;
    }
};

/// Pending action information
template<typename T = nlohmann::json>
struct PendingAction {
    int actionId = 0;
    int senderPlayerId = 0;
    std::string senderPlayerName;
    std::string actionType;
    T requestData;
    std::string createdAt;
    
    static PendingAction fromJson(const nlohmann::json& j) {
        PendingAction action;
        action.actionId = j.value("action_id", 0);
        action.senderPlayerId = j.value("sender_player_id", 0);
        action.senderPlayerName = j.value("sender_player_name", "");
        action.actionType = j.value("action_type", "");
        action.createdAt = j.value("created_at", "");
        
        if (j.contains("request_data")) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                action.requestData = j["request_data"];
            } else {
                action.requestData = j["request_data"].get<T>();
            }
        }
        
        return action;
    }
};

/// Submit action structure
template<typename T = nlohmann::json>
struct SubmitAction {
    RoomTargetPlayers targetPlayers;
    std::string actionType;
    T data;
    std::vector<int> targetPlayerIds;
    
    SubmitAction(RoomTargetPlayers target, const std::string& type, const T& d, 
                const std::vector<int>& ids = {})
        : targetPlayers(target), actionType(type), data(d), targetPlayerIds(ids) {}
};

// ====================== RESPONSE TYPES ======================

/// Response for action submission
struct ActionSubmitResponse : public ApiResponse {
    static ActionSubmitResponse fromJson(const nlohmann::json& j) {
        ActionSubmitResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for poll actions
template<typename T = nlohmann::json>
struct ActionPollResponse : public ApiResponse {
    std::vector<ActionInfo<T>> actions;
    
    static ActionPollResponse fromJson(const nlohmann::json& j) {
        ActionPollResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        
        if (j.contains("actions") && j["actions"].is_array()) {
            for (const auto& actionJson : j["actions"]) {
                response.actions.push_back(ActionInfo<T>::fromJson(actionJson));
            }
        }
        
        return response;
    }
};

/// Response for pending actions
template<typename T = nlohmann::json>
struct ActionPendingResponse : public ApiResponse {
    std::vector<PendingAction<T>> pendingActions;
    
    static ActionPendingResponse fromJson(const nlohmann::json& j) {
        ActionPendingResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        
        if (j.contains("pending_actions") && j["pending_actions"].is_array()) {
            for (const auto& actionJson : j["pending_actions"]) {
                response.pendingActions.push_back(PendingAction<T>::fromJson(actionJson));
            }
        }
        
        return response;
    }
};

/// Response for action completion
struct ActionCompleteResponse : public ApiResponse {
    static ActionCompleteResponse fromJson(const nlohmann::json& j) {
        ActionCompleteResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

// ====================== REQUEST TYPES ======================

/// Request for action submission
template<typename T = nlohmann::json>
struct ActionSubmitRequest {
    RoomTargetPlayers targetPlayers;
    std::string actionType;
    std::string requestDataJson;
    std::vector<int> targetPlayerIds;
    
    ActionSubmitRequest(RoomTargetPlayers target, const std::string& type, const T& data,
                       const std::vector<int>& ids = {})
        : targetPlayers(target), actionType(type), targetPlayerIds(ids) {
        if constexpr (std::is_same_v<T, nlohmann::json>) {
            requestDataJson = data.dump();
        } else {
            requestDataJson = nlohmann::json(data).dump();
        }
    }
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        j["target_players"] = roomTargetPlayersToString(targetPlayers);
        j["action_type"] = actionType;
        j["request_data_json"] = requestDataJson;
        j["target_players_ids"] = targetPlayerIds;
        return j;
    }
};

/// Request for action completion
template<typename T = nlohmann::json>
struct ActionCompleteRequest {
    RoomCompleteActionStatus status;
    std::string responseDataJson;
    
    ActionCompleteRequest(RoomCompleteActionStatus stat, const T& data)
        : status(stat) {
        if constexpr (std::is_same_v<T, nlohmann::json>) {
            responseDataJson = data.dump();
        } else {
            responseDataJson = nlohmann::json(data).dump();
        }
    }
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        std::string statusStr;
        switch (status) {
            case RoomCompleteActionStatus::Processing: statusStr = "processing"; break;
            case RoomCompleteActionStatus::Completed: statusStr = "completed"; break;
            case RoomCompleteActionStatus::Failed: statusStr = "failed"; break;
        }
        j["status"] = statusStr;
        j["response_data_json"] = responseDataJson;
        return j;
    }
};

/// Action complete structure
template<typename T = nlohmann::json>
struct ActionComplete {
    RoomCompleteActionStatus status;
    T responseData;
    
    ActionComplete(RoomCompleteActionStatus stat, const T& data)
        : status(stat), responseData(data) {}
};

// ====================== ACTIONS CLASS ======================

/// Provides methods for managing room actions
class Actions {
public:
    /// Submits an action to target players in the room.
    template<typename T = nlohmann::json>
    static ActionSubmitResponse submitAction(Client& client,
                                            const std::string& playerToken,
                                            const SubmitAction<T>& action) {
        ActionSubmitRequest<T> request(action.targetPlayers, action.actionType, 
                                       action.data, action.targetPlayerIds);
        return client.post<ActionSubmitResponse>(
            client.url(Endpoints::GameRoomActions, "&player_token=" + playerToken),
            request.toJson()
        );
    }
    
    /// Polls for completed actions that were targeted to the current player.
    template<typename T = nlohmann::json>
    static ActionPollResponse<T> pollActions(Client& client,
                                             const std::string& playerToken) {
        return client.get<ActionPollResponse<T>>(
            client.url(Endpoints::GameRoomActionsPoll, "&player_token=" + playerToken)
        );
    }
    
    /// Retrieves pending actions that need to be completed by the host.
    template<typename T = nlohmann::json>
    static ActionPendingResponse<T> getPendingActions(Client& client,
                                                      const std::string& playerToken) {
        return client.get<ActionPendingResponse<T>>(
            client.url(Endpoints::GameRoomActionsPending, "&player_token=" + playerToken)
        );
    }
    
    /// Marks an action as complete with an optional response.
    template<typename T = nlohmann::json>
    static ActionCompleteResponse completeAction(Client& client,
                                                 int actionId,
                                                 const std::string& playerToken,
                                                 const ActionComplete<T>& action) {
        ActionCompleteRequest<T> request(action.status, action.responseData);
        std::string endpoint = std::string(Endpoints::GameRoomActionComplete);
        size_t pos = endpoint.find("{0}");
        if (pos != std::string::npos) {
            endpoint.replace(pos, 3, std::to_string(actionId));
        }
        return client.post<ActionCompleteResponse>(
            client.url(endpoint, "&player_token=" + playerToken),
            request.toJson()
        );
    }
};

} // namespace actions
} // namespace rooms
} // namespace multiplayer
} // namespace michitai
