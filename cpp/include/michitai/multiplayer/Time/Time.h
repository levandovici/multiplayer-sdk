#pragma once

#include "../Client.h"
#include "../ApiResponse.h"
#include <string>

namespace michitai {
namespace multiplayer {
namespace time {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* Time = "time.php";
}

// ====================== TIME OFFSET ======================

/// Time offset information
struct TimeOffset {
    int hours = 0;
    std::string formatted;
    
    static TimeOffset fromJson(const nlohmann::json& j) {
        TimeOffset offset;
        offset.hours = j.value("hours", 0);
        offset.formatted = j.value("formatted", "");
        return offset;
    }
};

// ====================== RESPONSE TYPES ======================

/// Response containing the current server time in UTC
struct ServerTimeResponse : public ApiResponse {
    std::string utc;
    long timestamp = 0;
    std::string readable;
    
    static ServerTimeResponse fromJson(const nlohmann::json& j) {
        ServerTimeResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.utc = j.value("utc", "");
        response.timestamp = j.value("timestamp", 0);
        response.readable = j.value("readable", "");
        return response;
    }
};

/// Response containing the server time with a specified UTC offset
struct ServerTimeWithOffsetResponse : public ServerTimeResponse {
    std::optional<TimeOffset> offset;
    
    static ServerTimeWithOffsetResponse fromJson(const nlohmann::json& j) {
        ServerTimeWithOffsetResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.utc = j.value("utc", "");
        response.timestamp = j.value("timestamp", 0);
        response.readable = j.value("readable", "");
        
        if (j.contains("offset") && !j["offset"].is_null()) {
            response.offset = TimeOffset::fromJson(j["offset"]);
        }
        
        return response;
    }
};

// ====================== TIME CLASS ======================

/// Provides methods for querying server time information
class Time {
public:
    /// Retrieves the current server time in UTC.
    /// @param client The API client instance
    /// @return Response containing the server UTC time, timestamp, and readable format
    static ServerTimeResponse getServerTime(Client& client) {
        return client.get<ServerTimeResponse>(
            client.url(Endpoints::Time)
        );
    }
    
    /// Retrieves the server time with a specified UTC offset.
    /// @param client The API client instance
    /// @param utcOffset The UTC offset in hours (e.g., 3 for UTC+3, -5 for UTC-5)
    /// @return Response containing the adjusted time with offset information
    static ServerTimeWithOffsetResponse getServerTimeWithOffset(Client& client, int utcOffset) {
        std::string offsetStr = (utcOffset >= 0) ? ("&utc=+" + std::to_string(utcOffset)) : ("&utc=" + std::to_string(utcOffset));
        return client.get<ServerTimeWithOffsetResponse>(
            client.url(Endpoints::Time, offsetStr)
        );
    }
};

} // namespace time
} // namespace multiplayer
} // namespace michitai
