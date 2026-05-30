#pragma once

#include <string>
#include <vector>
#include <optional>
#include <nlohmann/json.hpp>

namespace michitai {
namespace multiplayer {

// ====================== COMMON ENUMS ======================

/// Ban duration options for player bans
enum class BanTime {
    Hour,
    Day,
    Week,
    Month,
    Quarter,
    Year,
    Forever
};

/// Convert BanTime to string for API requests
inline std::string banTimeToString(BanTime banTime) {
    switch (banTime) {
        case BanTime::Hour: return "hour";
        case BanTime::Day: return "day";
        case BanTime::Week: return "week";
        case BanTime::Month: return "month";
        case BanTime::Quarter: return "quarter";
        case BanTime::Year: return "year";
        case BanTime::Forever: return "forever";
        default: return "day";
    }
}

/// Target player types for room actions and updates
enum class RoomTargetPlayers {
    Host,
    All,
    Others,
    Specific
};

/// Convert RoomTargetPlayers to string for API requests
inline std::string roomTargetPlayersToString(RoomTargetPlayers target) {
    switch (target) {
        case RoomTargetPlayers::Host: return "host";
        case RoomTargetPlayers::All: return "all";
        case RoomTargetPlayers::Others: return "others";
        case RoomTargetPlayers::Specific: return "specific";
        default: return "all";
    }
}

/// Matchmaking request actions
enum class MatchmakingRequestAction {
    Approve,
    Reject
};

/// Convert MatchmakingRequestAction to string for API requests
inline std::string matchmakingRequestActionToString(MatchmakingRequestAction action) {
    switch (action) {
        case MatchmakingRequestAction::Approve: return "approve";
        case MatchmakingRequestAction::Reject: return "reject";
        default: return "approve";
    }
}

// ====================== COMMON TYPES ======================

/// Base type for JSON data - use nlohmann::json for flexible data
using JsonData = nlohmann::json;

/// Optional string type
using OptionalString = std::optional<std::string>;

/// Optional integer type
using OptionalInt = std::optional<int>;

// ====================== HELPER FUNCTIONS ======================

/// Helper to convert optional to JSON value
template<typename T>
inline nlohmann::json optionalToJson(const std::optional<T>& opt) {
    if (opt.has_value()) {
        return opt.value();
    }
    return nullptr;
}

/// Helper to get optional value from JSON
template<typename T>
inline std::optional<T> getOptionalJson(const nlohmann::json& j, const std::string& key) {
    if (j.contains(key) && !j[key].is_null()) {
        return j[key].get<T>();
    }
    return std::nullopt;
}

} // namespace multiplayer
} // namespace michitai
