#pragma once

#include "../../Client.h"
#include "../../ApiResponse.h"
#include <string>

namespace michitai {
namespace multiplayer {
namespace players {
namespace ban {

// ====================== RESPONSE TYPES ======================

/// Response containing detailed ban information for a player
struct BanResponse : public ApiResponse {
    std::string banId;
    int playerId = 0;
    std::string banDuration;
    std::string bannedUntil;
    std::string banReason;
    
    /// Checks if this response indicates the player is banned
    bool isBanned() const {
        return !success && error.find("You are banned") != std::string::npos;
    }
    
    static BanResponse fromJson(const nlohmann::json& j) {
        BanResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.banId = j.value("ban_id", "");
        response.playerId = j.value("player_id", 0);
        response.banDuration = j.value("ban_duration", "");
        response.bannedUntil = j.value("banned_until", "");
        response.banReason = j.value("ban_reason", "");
        return response;
    }
};

// ====================== BAN CLASS ======================

/// Static class providing utility methods for ban-related operations
class Ban {
public:
    /// Checks if an API response indicates the player is banned
    /// @param response The API response to check
    /// @return True if the error message indicates the player is banned, false otherwise
    static bool isBanned(const ApiResponse& response) {
        if (!response.success && !response.error.empty()) {
            return response.error.find("You are banned") != std::string::npos;
        }
        return false;
    }
};

} // namespace ban
} // namespace players
} // namespace multiplayer
} // namespace michitai
