#pragma once

#include "../Client.h"
#include "../ApiResponse.h"
#include <string>

namespace michitai {
namespace multiplayer {
namespace moderation {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* Censor = "censor.php";
}

// ====================== RESPONSE TYPES ======================

/// Response from the text screening endpoint
struct CensorResponse : public ApiResponse {
    /// True when the text contains no listed word after normalization
    bool clean = false;

    /// The text with listed words masked and markup/control characters
    /// stripped — safe to display as-is
    std::string filtered;

    /// Seconds until the rate-limit bucket refills (only on a 429 reply)
    int retryAfter = 0;

    static CensorResponse fromJson(const nlohmann::json& j) {
        CensorResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.clean = j.value("clean", false);
        response.filtered = j.value("filtered", "");
        response.retryAfter = j.value("retry_after", 0);
        return response;
    }
};

// ====================== MODERATION CLASS ======================

/// Provides methods for screening player-visible text through the
/// server-side moderation filter. The filter normalizes leetspeak,
/// confusable scripts, separators and repeated-letter evasion before
/// matching, so "w e e d" or "w33d" are caught the same as "weed".
class Moderation {
public:
    /// Screens text and returns whether it is clean plus a filtered copy.
    /// @param client The API client instance
    /// @param text The text to screen (max 1000 characters)
    /// @return Response with clean (no listed word found) and filtered (masked display text)
    static CensorResponse censor(Client& client, const std::string& text) {
        nlohmann::json body;
        body["text"] = text;
        return client.post<CensorResponse>(
            client.url(Endpoints::Censor), body
        );
    }
};

} // namespace moderation
} // namespace multiplayer
} // namespace michitai
