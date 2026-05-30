#pragma once

#include <string>
#include "Types.h"

namespace michitai {
namespace multiplayer {

/// Base class for all API responses
struct ApiResponse {
    /// Indicates whether the API request was successful
    bool success = false;
    
    /// Error message if the request failed, empty otherwise
    std::string error;
    
    /// Default constructor
    ApiResponse() = default;
    
    /// Construct with success status
    ApiResponse(bool success) : success(success) {}
    
    /// Construct with success and error
    ApiResponse(bool success, const std::string& error) 
        : success(success), error(error) {}
    
    /// Parse from JSON
    static ApiResponse fromJson(const nlohmann::json& j) {
        ApiResponse response;
        if (j.contains("success")) {
            response.success = j["success"].get<bool>();
        }
        if (j.contains("error")) {
            response.error = j["error"].get<std::string>();
        }
        return response;
    }
};

/// Simple success response
struct SuccessResponse : public ApiResponse {
    /// Default constructor
    SuccessResponse() = default;
    
    /// Parse from JSON
    static SuccessResponse fromJson(const nlohmann::json& j) {
        SuccessResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

} // namespace multiplayer
} // namespace michitai
