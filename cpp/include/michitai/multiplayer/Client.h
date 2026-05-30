#pragma once

#include <string>
#include <memory>
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
    /// @param apiPrivateToken Private API token for admin operations
    /// @param baseUrl Base URL for the API (default: https://api.michitai.com/api)
    /// @param logger Optional logger for debugging and error tracking
    Client(const std::string& apiToken, 
           const std::string& apiPrivateToken,
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
    
    /// Generate a URL for public API endpoints
    /// @param endpoint The API endpoint path
    /// @param extra Additional query parameters
    /// @return Complete URL with API token
    std::string url(const std::string& endpoint, const std::string& extra = "") const;
    
    /// Generate a URL for private API endpoints requiring admin access
    /// @param endpoint The API endpoint path
    /// @param extra Additional query parameters
    /// @return Complete URL with API token and private token
    std::string privateUrl(const std::string& endpoint, const std::string& extra = "") const;
    
    /// Send an HTTP GET request to the API and deserialize the response
    /// @tparam T The response type, must be constructible from JSON
    /// @param url The complete URL to send the request to
    /// @return Deserialized API response of type T
    template<typename T>
    T get(const std::string& url) {
        cpr::Response response = cpr::Get(
            cpr::Url{url},
            cpr::Header{{"Content-Type", "application/json"}},
            cpr::Timeout{30000},
            cpr::VerifySsl{false}
        );
        
        logger_->log("API URL: " + url);
        logger_->log("API Response Status: " + std::to_string(response.status_code));
        logger_->log("API Response: " + response.text);
        
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
            logger_->warn("JSON Deserialization Error. Raw: " + response.text + ". Exception: " + ex.what());
            
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
    /// @return Deserialized API response of type T
    template<typename T>
    T post(const std::string& url, const nlohmann::json& body) {
        std::string bodyStr = body.dump();
        cpr::Response response = cpr::Post(
            cpr::Url{url},
            cpr::Header{{"Content-Type", "application/json"}},
            cpr::Body{bodyStr},
            cpr::Timeout{30000},
            cpr::VerifySsl{false}
        );
        
        logger_->log("API URL: " + url);
        logger_->log("API Response Status: " + std::to_string(response.status_code));
        logger_->log("API Response: " + response.text);
        
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
            logger_->warn("JSON Deserialization Error. Raw: " + response.text + ". Exception: " + ex.what());
            
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
    /// @return Deserialized API response of type T
    template<typename T>
    T put(const std::string& url, const nlohmann::json& body) {
        std::string bodyStr = body.dump();
        cpr::Response response = cpr::Put(
            cpr::Url{url},
            cpr::Header{{"Content-Type", "application/json"}},
            cpr::Body{bodyStr},
            cpr::Timeout{30000},
            cpr::VerifySsl{false}
        );
        
        logger_->log("API URL: " + url);
        logger_->log("API Response Status: " + std::to_string(response.status_code));
        logger_->log("API Response: " + response.text);
        
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
            logger_->warn("JSON Deserialization Error. Raw: " + response.text + ". Exception: " + ex.what());
            
            T result;
            result.success = false;
            result.error = "Failed to deserialize response";
            return result;
        }
    }

    /// Send an HTTP DELETE request to the API and deserialize the response
    /// @tparam T The response type, must be constructible from JSON
    /// @param url The complete URL to send the request to
    /// @return Deserialized API response of type T
    template<typename T>
    T del(const std::string& url) {
        cpr::Response response = cpr::Delete(
            cpr::Url{url},
            cpr::Header{{"Content-Type", "application/json"}},
            cpr::Timeout{30000},
            cpr::VerifySsl{false}
        );
        
        logger_->log("API URL: " + url);
        logger_->log("API Response Status: " + std::to_string(response.status_code));
        logger_->log("API Response: " + response.text);
        
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
            logger_->warn("JSON Deserialization Error. Raw: " + response.text + ". Exception: " + ex.what());
            
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
};

} // namespace multiplayer
} // namespace michitai
