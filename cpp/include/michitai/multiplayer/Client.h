#pragma once

#include <string>
#include <memory>
#include <stdexcept>
#include <utility>
#include <cpr/cpr.h>
#include <nlohmann/json.hpp>
#include "ApiResponse.h"
#include "Types.h"

namespace michitai {
namespace multiplayer {

/// Logger interface for debugging and error tracking
class ILogger {
public:
    virtual ~ILogger() = default;
    virtual void log(const std::string& message) = 0;
    virtual void error(const std::string& message) = 0;
    virtual void warn(const std::string& message) = 0;
};

/// Simple console logger implementation
class ConsoleLogger : public ILogger {
public:
    void log(const std::string& message) override;
    void error(const std::string& message) override;
    void warn(const std::string& message) override;
};

/// Main HTTP client for communicating with the Michitai Multiplayer API
class Client {
public:
    /// Initialize a new client
    /// @param apiToken Public API token for game identification
    /// @param apiPrivateToken Private API token for admin operations (may be
    ///        empty — omit it in shipped/game clients; admin endpoints then throw)
    /// @param baseUrl Base URL for the API (default: https://api.michitai.com/api)
    /// @param logger Optional logger for debugging and error tracking
    Client(const std::string& apiToken,
           const std::string& apiPrivateToken = "",
           const std::string& baseUrl = "https://api.michitai.com/api",
           std::shared_ptr<ILogger> logger = nullptr);
    
    /// Destructor
    ~Client() = default;
    
    // Delete copy constructor and assignment operator
    Client(const Client&) = delete;
    Client& operator=(const Client&) = delete;
    
    /// Get the API token
    const std::string& getApiToken() const { return apiToken_; }
    
    /// Get the API private token
    const std::string& getApiPrivateToken() const { return apiPrivateToken_; }
    
    /// Get the base URL
    const std::string& getBaseUrl() const { return baseUrl_; }
    
    /// Get the logger
    std::shared_ptr<ILogger> getLogger() const { return logger_; }

    /// Enable verbose request/response body logging. Default off — response
    /// bodies can contain credentials; keep disabled in production builds.
    void setDebugLogging(bool enabled) { debugLogging_ = enabled; }
    
    /// Generate a URL for an API endpoint. Credentials are never placed in
    /// the URL — they are sent as headers by the request methods.
    /// @param endpoint The API endpoint path
    /// @param extra Additional query parameters, with or without a leading '?' or '&'
    /// @return Complete URL without credentials
    std::string url(const std::string& endpoint, const std::string& extra = "") const;

    /// Send an HTTP GET request to the API and deserialize the response
    /// @tparam T The response type, must be constructible from JSON
    /// @param url The complete URL to send the request to
    /// @param playerToken Optional player token (sent as X-Game-Player-Token)
    /// @param includePrivateToken Send the admin X-Api-Private-Token header
    /// @return Deserialized API response of type T
    template<typename T>
    T get(const std::string& url, const std::string& playerToken = "", bool includePrivateToken = false) {
        auto headers = authHeaders(playerToken, includePrivateToken);
        headers.emplace("Content-Type", "application/json");
        cpr::Response response = cpr::Get(
            cpr::Url{url},
            headers,
            cpr::Timeout{30000}
        );

        if (debugLogging_) {
            logger_->log("API URL: " + url);
            logger_->log("API Response Status: " + std::to_string(response.status_code));
            logger_->log("API Response: " + response.text);
        }
        
        if (response.status_code == 0) {
            logger_->error("HTTP Request Failed: " + response.error.message);
            T result;
            result.success = false;
            result.error = "HTTP Request Failed: " + response.error.message;
            return result;
        }
        
        try {
            nlohmann::json jsonResponse = nlohmann::json::parse(response.text);
            T result = T::fromJson(jsonResponse);
            
            if (!result.success) {
                logger_->error("API Error: " + result.error);
            }
            
            return result;
        } catch (const nlohmann::json::exception& ex) {
            logger_->warn(debugLogging_ ? "JSON Deserialization Error. Raw: " + response.text + ". Exception: " + ex.what()
                                    : "JSON Deserialization Error. Exception: " + ex.what());
            
            T result;
            result.success = false;
            result.error = "Failed to deserialize response";
            return result;
        }
    }

    /// Send an HTTP POST request to the API and deserialize the response
    /// @tparam T The response type, must be constructible from JSON
    /// @param url The complete URL to send the request to
    /// @param body Request body to serialize as JSON
    /// @param playerToken Optional player token (sent as X-Game-Player-Token)
    /// @param includePrivateToken Send the admin X-Api-Private-Token header
    /// @return Deserialized API response of type T
    template<typename T>
    T post(const std::string& url, const nlohmann::json& body,
           const std::string& playerToken = "", bool includePrivateToken = false) {
        std::string bodyStr = body.dump();
        auto headers = authHeaders(playerToken, includePrivateToken);
        headers.emplace("Content-Type", "application/json");
        cpr::Response response = cpr::Post(
            cpr::Url{url},
            headers,
            cpr::Body{bodyStr},
            cpr::Timeout{30000}
        );

        if (debugLogging_) {
            logger_->log("API URL: " + url);
            logger_->log("API Response Status: " + std::to_string(response.status_code));
            logger_->log("API Response: " + response.text);
        }
        
        if (response.status_code == 0) {
            logger_->error("HTTP Request Failed: " + response.error.message);
            T result;
            result.success = false;
            result.error = "HTTP Request Failed: " + response.error.message;
            return result;
        }
        
        try {
            nlohmann::json jsonResponse = nlohmann::json::parse(response.text);
            T result = T::fromJson(jsonResponse);
            
            if (!result.success) {
                logger_->error("API Error: " + result.error);
            }
            
            return result;
        } catch (const nlohmann::json::exception& ex) {
            logger_->warn(debugLogging_ ? "JSON Deserialization Error. Raw: " + response.text + ". Exception: " + ex.what()
                                    : "JSON Deserialization Error. Exception: " + ex.what());
            
            T result;
            result.success = false;
            result.error = "Failed to deserialize response";
            return result;
        }
    }

    /// Send an HTTP PUT request to the API and deserialize the response
    /// @tparam T The response type, must be constructible from JSON
    /// @param url The complete URL to send the request to
    /// @param body Request body to serialize as JSON
    /// @param playerToken Optional player token (sent as X-Game-Player-Token)
    /// @param includePrivateToken Send the admin X-Api-Private-Token header
    /// @return Deserialized API response of type T
    template<typename T>
    T put(const std::string& url, const nlohmann::json& body,
          const std::string& playerToken = "", bool includePrivateToken = false) {
        std::string bodyStr = body.dump();
        auto headers = authHeaders(playerToken, includePrivateToken);
        headers.emplace("Content-Type", "application/json");
        cpr::Response response = cpr::Put(
            cpr::Url{url},
            headers,
            cpr::Body{bodyStr},
            cpr::Timeout{30000}
        );

        if (debugLogging_) {
            logger_->log("API URL: " + url);
            logger_->log("API Response Status: " + std::to_string(response.status_code));
            logger_->log("API Response: " + response.text);
        }
        
        if (response.status_code == 0) {
            logger_->error("HTTP Request Failed: " + response.error.message);
            T result;
            result.success = false;
            result.error = "HTTP Request Failed: " + response.error.message;
            return result;
        }
        
        try {
            nlohmann::json jsonResponse = nlohmann::json::parse(response.text);
            T result = T::fromJson(jsonResponse);
            
            if (!result.success) {
                logger_->error("API Error: " + result.error);
            }
            
            return result;
        } catch (const nlohmann::json::exception& ex) {
            logger_->warn(debugLogging_ ? "JSON Deserialization Error. Raw: " + response.text + ". Exception: " + ex.what()
                                    : "JSON Deserialization Error. Exception: " + ex.what());
            
            T result;
            result.success = false;
            result.error = "Failed to deserialize response";
            return result;
        }
    }

    /// Send an HTTP DELETE request to the API and deserialize the response
    /// @tparam T The response type, must be constructible from JSON
    /// @param url The complete URL to send the request to
    /// @param playerToken Optional player token (sent as X-Game-Player-Token)
    /// @param includePrivateToken Send the admin X-Api-Private-Token header
    /// @return Deserialized API response of type T
    template<typename T>
    T del(const std::string& url, const std::string& playerToken = "", bool includePrivateToken = false) {
        auto headers = authHeaders(playerToken, includePrivateToken);
        headers.emplace("Content-Type", "application/json");
        cpr::Response response = cpr::Delete(
            cpr::Url{url},
            headers,
            cpr::Timeout{30000}
        );

        if (debugLogging_) {
            logger_->log("API URL: " + url);
            logger_->log("API Response Status: " + std::to_string(response.status_code));
            logger_->log("API Response: " + response.text);
        }
        
        if (response.status_code == 0) {
            logger_->error("HTTP Request Failed: " + response.error.message);
            T result;
            result.success = false;
            result.error = "HTTP Request Failed: " + response.error.message;
            return result;
        }
        
        try {
            nlohmann::json jsonResponse = nlohmann::json::parse(response.text);
            T result = T::fromJson(jsonResponse);
            
            if (!result.success) {
                logger_->error("API Error: " + result.error);
            }
            
            return result;
        } catch (const nlohmann::json::exception& ex) {
            logger_->warn(debugLogging_ ? "JSON Deserialization Error. Raw: " + response.text + ". Exception: " + ex.what()
                                    : "JSON Deserialization Error. Exception: " + ex.what());
            
            T result;
            result.success = false;
            result.error = "Failed to deserialize response";
            return result;
        }
    }

private:
    std::string apiToken_;
    std::string apiPrivateToken_;
    std::string baseUrl_;
    std::shared_ptr<ILogger> logger_;
    bool debugLogging_ = false;

    /// Build the auth headers for a request: API token always, player/admin
    /// tokens only when the endpoint requires them.
    cpr::Header authHeaders(const std::string& playerToken, bool includePrivateToken) const {
        cpr::Header headers;
        headers.emplace("X-Api-Token", apiToken_);
        if (!playerToken.empty()) {
            headers.emplace("X-Game-Player-Token", playerToken);
        }
        if (includePrivateToken) {
            if (apiPrivateToken_.empty()) {
                throw std::logic_error(
                    "Admin operations require apiPrivateToken — construct Client with the private key (server-side tooling only)");
            }
            headers.emplace("X-Api-Private-Token", apiPrivateToken_);
        }
        return headers;
    }
};

} // namespace multiplayer
} // namespace michitai
