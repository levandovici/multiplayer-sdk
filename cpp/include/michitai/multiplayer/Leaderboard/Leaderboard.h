#pragma once

#include "../Client.h"
#include "../ApiResponse.h"
#include "../Types.h"
#include <string>
#include <vector>

namespace michitai {
namespace multiplayer {
namespace leaderboard {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* Leaderboard = "leaderboard.php";
}

// ====================== LEADERBOARD TYPES ======================

/// Leaderboard entry with player ranking
template<typename T = nlohmann::json>
struct LeaderboardPlayer {
    int rank = 0;
    int playerId = 0;
    std::string playerName;
    T playerData;
    
    static LeaderboardPlayer fromJson(const nlohmann::json& j) {
        LeaderboardPlayer player;
        player.rank = j.value("rank", 0);
        player.playerId = j.value("player_id", 0);
        player.playerName = j.value("player_name", "");
        
        if (j.contains("player_data")) {
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

/// Response containing the leaderboard entries with rankings
template<typename T = nlohmann::json>
struct LeaderboardResponse : public ApiResponse {
    std::vector<LeaderboardPlayer<T>> leaderboard;
    int total = 0;
    std::vector<std::string> sortBy;
    int limit = 0;
    
    static LeaderboardResponse fromJson(const nlohmann::json& j) {
        LeaderboardResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.total = j.value("total", 0);
        response.limit = j.value("limit", 0);
        
        if (j.contains("leaderboard") && j["leaderboard"].is_array()) {
            for (const auto& playerJson : j["leaderboard"]) {
                response.leaderboard.push_back(LeaderboardPlayer<T>::fromJson(playerJson));
            }
        }
        
        if (j.contains("sort_by") && j["sort_by"].is_array()) {
            for (const auto& field : j["sort_by"]) {
                response.sortBy.push_back(field.get<std::string>());
            }
        }
        
        return response;
    }
};

// ====================== REQUEST TYPES ======================

/// Request for leaderboard query
struct LeaderboardRequest {
    std::vector<std::string> sortBy;
    int limit = 10;
    
    LeaderboardRequest(const std::vector<std::string>& fields, int lim = 10)
        : sortBy(fields), limit(lim) {}
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        j["sort_by"] = sortBy;
        j["limit"] = limit;
        return j;
    }
};

// ====================== LEADERBOARD CLASS ======================

/// Provides methods for querying and retrieving leaderboard rankings
class Leaderboard {
public:
    /// Retrieves the leaderboard with specified sorting and limit.
    /// @param client The API client instance
    /// @param sortBy Array of field names to sort by (e.g., {"level", "wins"})
    /// @param limit Maximum number of results to return (1-100, default: 10)
    /// @return Response containing the leaderboard entries with rankings
    template<typename T = nlohmann::json>
    static LeaderboardResponse<T> getLeaderboard(
        Client& client,
        const std::vector<std::string>& sortBy,
        int limit = 10) {
        LeaderboardRequest request(sortBy, limit);
        return client.post<LeaderboardResponse<T>>(
            client.url(Endpoints::Leaderboard),
            request.toJson()
        );
    }
};

} // namespace leaderboard
} // namespace multiplayer
} // namespace michitai
