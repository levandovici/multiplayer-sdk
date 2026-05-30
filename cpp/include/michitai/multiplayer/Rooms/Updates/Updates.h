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
namespace updates {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* GameRoomUpdates = "game_room.php/updates";
    constexpr const char* GameRoomUpdatesPoll = "game_room.php/updates/poll";
}

// ====================== UPDATE TYPES ======================

/// Player update information
template<typename T = nlohmann::json>
struct PlayerUpdate {
    int senderPlayerId = 0;
    std::string senderPlayerName;
    std::string type;
    T data;
    std::string createdAt;
    
    static PlayerUpdate fromJson(const nlohmann::json& j) {
        PlayerUpdate update;
        update.senderPlayerId = j.value("sender_player_id", 0);
        update.senderPlayerName = j.value("sender_player_name", "");
        update.type = j.value("type", "");
        update.createdAt = j.value("created_at", "");
        
        if (j.contains("data")) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                update.data = j["data"];
            } else {
                update.data = j["data"].get<T>();
            }
        }
        
        return update;
    }
};

/// Update players structure
template<typename T = nlohmann::json>
struct UpdatePlayers {
    RoomTargetPlayers targetPlayers;
    std::string type;
    T data;
    std::vector<int> targetPlayerIds;
    
    UpdatePlayers(RoomTargetPlayers target, const std::string& t, const T& d,
                 const std::vector<int>& ids = {})
        : targetPlayers(target), type(t), data(d), targetPlayerIds(ids) {}
};

/// Poll updates structure
struct PollUpdates {
    RoomTargetPlayers fromPlayers;
    std::vector<int> fromPlayerIds;
    std::optional<std::string> lastUpdate;
    
    PollUpdates(RoomTargetPlayers from = RoomTargetPlayers::Host,
                const std::vector<int>& ids = {},
                const std::optional<std::string>& last = std::nullopt)
        : fromPlayers(from), fromPlayerIds(ids), lastUpdate(last) {}
};

// ====================== RESPONSE TYPES ======================

/// Response for update players
struct UpdatePlayersResponse : public ApiResponse {
    static UpdatePlayersResponse fromJson(const nlohmann::json& j) {
        UpdatePlayersResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for poll updates
template<typename T = nlohmann::json>
struct PollUpdatesResponse : public ApiResponse {
    std::vector<PlayerUpdate<T>> updates;
    
    static PollUpdatesResponse fromJson(const nlohmann::json& j) {
        PollUpdatesResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        
        if (j.contains("updates") && j["updates"].is_array()) {
            for (const auto& updateJson : j["updates"]) {
                response.updates.push_back(PlayerUpdate<T>::fromJson(updateJson));
            }
        }
        
        return response;
    }
};

// ====================== REQUEST TYPES ======================

/// Request for update players
template<typename T = nlohmann::json>
struct UpdatePlayersRequest {
    RoomTargetPlayers targetPlayers;
    std::string type;
    T data;
    std::vector<int> targetPlayerIds;
    
    UpdatePlayersRequest(RoomTargetPlayers target, const std::string& t, const T& d,
                       const std::vector<int>& ids = {})
        : targetPlayers(target), type(t), data(d), targetPlayerIds(ids) {}
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        j["target_players"] = roomTargetPlayersToString(targetPlayers);
        j["type"] = type;
        
        if constexpr (std::is_same_v<T, nlohmann::json>) {
            j["data"] = data;
        } else {
            j["data"] = nlohmann::json(data);
        }
        
        j["target_players_ids"] = targetPlayerIds;
        return j;
    }
};

/// Request for poll updates
struct PollUpdatesRequest {
    RoomTargetPlayers fromPlayers;
    std::vector<int> fromPlayerIds;
    std::optional<std::string> lastUpdate;
    
    PollUpdatesRequest(RoomTargetPlayers from = RoomTargetPlayers::Host,
                      const std::vector<int>& ids = {},
                      const std::optional<std::string>& last = std::nullopt)
        : fromPlayers(from), fromPlayerIds(ids), lastUpdate(last) {}
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        j["from_players"] = roomTargetPlayersToString(fromPlayers);
        j["from_players_ids"] = fromPlayerIds;
        if (lastUpdate.has_value()) {
            j["last_update"] = lastUpdate.value();
        }
        return j;
    }
};

// ====================== UPDATES CLASS ======================

/// Provides methods for managing room updates
class Updates {
public:
    /// Sends updates to specific players in the room.
    template<typename T = nlohmann::json>
    static UpdatePlayersResponse updatePlayers(Client& client,
                                               const std::string& playerToken,
                                               const UpdatePlayers<T>& update) {
        UpdatePlayersRequest<T> request(update.targetPlayers, update.type, 
                                       update.data, update.targetPlayerIds);
        return client.post<UpdatePlayersResponse>(
            client.url(Endpoints::GameRoomUpdates, "&player_token=" + playerToken),
            request.toJson()
        );
    }
    
    /// Polls for updates that were sent to the current player.
    template<typename T = nlohmann::json>
    static PollUpdatesResponse<T> pollUpdates(Client& client,
                                              const std::string& playerToken,
                                              const PollUpdates& poll) {
        PollUpdatesRequest request(poll.fromPlayers, poll.fromPlayerIds, poll.lastUpdate);
        return client.post<PollUpdatesResponse<T>>(
            client.url(Endpoints::GameRoomUpdatesPoll, "&player_token=" + playerToken),
            request.toJson()
        );
    }
};

} // namespace updates
} // namespace rooms
} // namespace multiplayer
} // namespace michitai
