#pragma once

#include "../../Client.h"
#include "../../ApiResponse.h"
#include "../../Types.h"
#include <string>
#include <vector>
#include <functional>
#include <memory>

namespace michitai {
namespace multiplayer {
namespace rooms {
namespace realtime {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* RealtimeToken = "realtime.php/token";
}

// ====================== ENUMS ======================

/// Specifies the target players for realtime communication
enum class RoomTargetPlayer {
    All,
    Host,
    Others,
    Specific
};

/// Convert RoomTargetPlayer to string
inline std::string roomTargetPlayerToString(RoomTargetPlayer target) {
    switch (target) {
        case RoomTargetPlayer::All: return "all";
        case RoomTargetPlayer::Host: return "host";
        case RoomTargetPlayer::Others: return "others";
        case RoomTargetPlayer::Specific: return "specific";
        default: return "all";
    }
}

// ====================== REALTIME TYPES ======================

/// Player information for realtime WebSocket connections
struct PlayerInfo {
    int playerId = 0;
    std::string playerName;
    std::string roomId;
    bool isHost = false;
    
    static PlayerInfo fromJson(const nlohmann::json& j) {
        PlayerInfo info;
        info.playerId = j.value("player_id", 0);
        info.playerName = j.value("player_name", "");
        info.roomId = j.value("room_id", "");
        info.isHost = j.value("is_host", false);
        return info;
    }
};

/// Information about the realtime WebSocket server
struct RealtimeServerInfo {
    std::string host;
    int port = 0;
    std::string protocol;
    
    static RealtimeServerInfo fromJson(const nlohmann::json& j) {
        RealtimeServerInfo info;
        info.host = j.value("host", "");
        info.port = j.value("port", 0);
        info.protocol = j.value("protocol", "");
        return info;
    }
};

/// Information about the sender of a realtime message
struct SenderInfo {
    bool isHost = false;
    int gamePlayerId = 0;
    std::string playerName;
    
    static SenderInfo fromJson(const nlohmann::json& j) {
        SenderInfo info;
        info.isHost = j.value("is_host", false);
        info.gamePlayerId = j.value("game_player_id", 0);
        info.playerName = j.value("player_name", "");
        return info;
    }
};

/// Realtime message structure
struct RealtimeMessage {
    std::string type;
    std::string command;
    nlohmann::json data;
    SenderInfo sender;
    
    static RealtimeMessage fromJson(const nlohmann::json& j) {
        RealtimeMessage msg;
        msg.type = j.value("type", "");
        msg.command = j.value("command", "");
        msg.data = j.value("data", nlohmann::json::object());
        if (j.contains("sender")) {
            msg.sender = SenderInfo::fromJson(j["sender"]);
        }
        return msg;
    }
};

// ====================== RESPONSE TYPES ======================

/// Response containing the WebSocket token for realtime communication
struct TokenResponse : public ApiResponse {
    std::string token;
    PlayerInfo playerInfo;
    RealtimeServerInfo realtimeServer;
    
    static TokenResponse fromJson(const nlohmann::json& j) {
        TokenResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.token = j.value("token", "");
        
        if (j.contains("player_info")) {
            response.playerInfo = PlayerInfo::fromJson(j["player_info"]);
        }
        
        if (j.contains("realtime_server")) {
            response.realtimeServer = RealtimeServerInfo::fromJson(j["realtime_server"]);
        }
        
        return response;
    }
};

// ====================== REALTIME CLASS ======================

/// Manages WebSocket connections for realtime communication in game rooms
/// Note: Full WebSocket implementation requires additional dependencies (e.g., websocketpp, uWebSockets)
/// This class provides token retrieval and basic structure for WebSocket functionality
class Realtime {
public:
    /// Callback type for receive events
    using ReceiveCallback = std::function<void(const std::string&, const nlohmann::json&, const SenderInfo&)>;
    
    /// Callback type for connected events
    using ConnectedCallback = std::function<void()>;
    
    /// Retrieves a realtime authentication token for WebSocket connections
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @return Response containing the realtime token
    static TokenResponse getToken(Client& client, const std::string& playerToken) {
        return client.post<TokenResponse>(
            client.url(Endpoints::RealtimeToken, "&player_token=" + playerToken)
        );
    }
    
    /// Constructs a WebSocket URL from server info and token
    /// @param serverInfo The realtime server information
    /// @param token The authentication token
    /// @param clientType The client type (default: "json")
    /// @return Complete WebSocket URL
    static std::string buildWebSocketUrl(const RealtimeServerInfo& serverInfo,
                                         const std::string& token,
                                         const std::string& clientType = "json") {
        return serverInfo.protocol + "://" + serverInfo.host + ":" + 
               std::to_string(serverInfo.port) + "?token=" + token + "&client=" + clientType;
    }
};

} // namespace realtime
} // namespace rooms
} // namespace multiplayer
} // namespace michitai
