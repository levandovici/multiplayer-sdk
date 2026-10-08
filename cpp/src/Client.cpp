#include "michitai/multiplayer/Client.h"
#include <iostream>

namespace michitai {
namespace multiplayer {

// ====================== CONSOLE LOGGER ======================

void ConsoleLogger::log(const std::string& message) {
    std::cout << "[LOG] " << message << std::endl;
}

void ConsoleLogger::error(const std::string& message) {
    std::cerr << "[ERROR] " << message << std::endl;
}

void ConsoleLogger::warn(const std::string& message) {
    std::cout << "[WARN] " << message << std::endl;
}

// ====================== CLIENT ======================

Client::Client(const std::string& apiToken,
               const std::string& apiPrivateToken,
               const std::string& baseUrl,
               std::shared_ptr<ILogger> logger)
    : apiToken_(apiToken)
    , apiPrivateToken_(apiPrivateToken)
    , baseUrl_(baseUrl)
    , logger_(logger ? logger : std::make_shared<ConsoleLogger>())
{
    // Ensure baseUrl doesn't end with a slash
    if (!baseUrl_.empty() && baseUrl_.back() == '/') {
        baseUrl_.pop_back();
    }
}

std::string Client::url(const std::string& endpoint, const std::string& extra) const {
    // Credentials are never placed in the URL — they are sent as headers
    // by the request methods. extra may start with '?' or '&'.
    std::string normalized = extra;
    if (!normalized.empty() && (normalized.front() == '?' || normalized.front() == '&')) {
        normalized.erase(0, 1);
    }
    std::string result = baseUrl_ + "/" + endpoint;
    if (!normalized.empty()) {
        result += "?" + normalized;
    }
    return result;
}

} // namespace multiplayer
} // namespace michitai
