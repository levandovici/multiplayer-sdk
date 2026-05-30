#pragma once

#include <string>

namespace michitai {
namespace multiplayer {
namespace errors {

// ====================== COMMON ERRORS ======================

/// Common default errors that apply to most endpoints
enum class CommonError {
    Unknown,
    ApiTokenIsRequired,
    InvalidApiToken,
    MethodNotAllowed,
    InternalServerError,
    FailedToDeserializeResponse,
    InvalidEndpoint,
    DatabaseError,
    AnUnexpectedErrorOccurred,
    YouAreBanned
};

/// Convert CommonError to string
inline std::string commonErrorToString(CommonError error) {
    switch (error) {
        case CommonError::Unknown: return "Unknown";
        case CommonError::ApiTokenIsRequired: return "Api token is required";
        case CommonError::InvalidApiToken: return "Invalid API token";
        case CommonError::MethodNotAllowed: return "Method not allowed";
        case CommonError::InternalServerError: return "Internal server error";
        case CommonError::FailedToDeserializeResponse: return "Failed to deserialize response";
        case CommonError::InvalidEndpoint: return "Invalid endpoint";
        case CommonError::DatabaseError: return "Database error";
        case CommonError::AnUnexpectedErrorOccurred: return "An unexpected error occurred";
        case CommonError::YouAreBanned: return "You are banned";
        default: return "Unknown";
    }
}

// ====================== ERROR HELPER ======================

/// Check if error message indicates player is banned
inline bool isBanned(const std::string& errorMessage) {
    return errorMessage.find("You are banned") != std::string::npos;
}

} // namespace errors
} // namespace multiplayer
} // namespace michitai
