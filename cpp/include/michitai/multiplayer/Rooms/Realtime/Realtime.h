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
/// This class provides token retrieval and structure for WebSocket functionality
class Realtime {
public:
    /// Callback type for receive events
    using ReceiveCallback = std::function<void(const std::string&, const nlohmann::json&, const SenderInfo&)>;
    
    /// Callback type for connected events
    using ConnectedCallback = std::function<void()>;
    
    /// Constructor
    /// @param realtimeWebSocketUrl The WebSocket server URL (default: "wss://realtime.michitai.com")
    Realtime(const std::string& realtimeWebSocketUrl = "wss://realtime.michitai.com")
        : realtimeWebSocketUrl(realtimeWebSocketUrl), connected(false) {}
    
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
    
    /// Connects to the realtime WebSocket server using the provided token
    /// @param realtimeToken The realtime authentication token
    /// @return True if connection succeeded, false otherwise
    /// Note: Requires WebSocket library implementation (e.g., websocketpp, uWebSockets)
    bool connect(const std::string& realtimeToken) {
        // TODO: Implement WebSocket connection using external library
        // This is a placeholder for the actual implementation
        token = realtimeToken;
        
        // In a real implementation, you would:
        // 1. Initialize WebSocket client
        // 2. Connect to realtimeWebSocketUrl with token
        // 3. Start message listener thread
        // 4. Start heartbeat thread
        // 5. Invoke onConnected callback
        
        connected = true;
        if (onConnected) {
            onConnected();
        }
        
        return true;
    }
    
    /// Sends a message to the specified players via WebSocket
    /// @param target The target players (All, Host, Others, Specific)
    /// @param command The command/type of the message
    /// @param data Optional data payload to send
    /// @param targetIds Specific player IDs if target is Specific
    /// Note: Requires WebSocket library implementation
    void send(RoomTargetPlayer target, const std::string& command, 
              const nlohmann::json& data = nlohmann::json::object(),
              const std::vector<int>& targetIds = {}) {
        if (!connected) return;
        
        // TODO: Implement WebSocket send using external library
        // This is a placeholder for the actual implementation
        
        nlohmann::json message;
        message["type"] = "send";
        message["command"] = command;
        message["data"] = data;
        message["target_ids"] = targetIds;
        message["target"] = roomTargetPlayerToString(target);
        
        // In a real implementation, serialize and send via WebSocket
    }
    
    /// Disconnects from the WebSocket server and cleans up resources
    /// Note: Requires WebSocket library implementation
    void disconnect() {
        // TODO: Implement WebSocket disconnect using external library
        // This is a placeholder for the actual implementation
        
        // In a real implementation, you would:
        // 1. Stop heartbeat thread
        // 2. Stop message listener thread
        // 3. Close WebSocket connection
        // 4. Clean up resources
        
        connected = false;
    }
    
    /// Sets the callback for receive events
    /// @param callback The function to call when a message is received
    void setReceiveCallback(ReceiveCallback callback) {
        onReceive = callback;
    }
    
    /// Sets the callback for connected events
    /// @param callback The function to call when connection is established
    void setConnectedCallback(ConnectedCallback callback) {
        onConnected = callback;
    }
    
private:
    std::string realtimeWebSocketUrl;
    std::string token;
    bool connected;
    ReceiveCallback onReceive;
    ConnectedCallback onConnected;
    
    /// Sends a heartbeat message to keep the connection alive
    /// Note: Requires WebSocket library implementation
    void sendHeartbeat() {
        if (!connected) return;
        
        // TODO: Implement heartbeat send using external library
        nlohmann::json message;
        message["type"] = "heartbeat";
        
        // In a real implementation, send via WebSocket
    }
    
    /// Listens for incoming messages from the WebSocket
    /// Note: Requires WebSocket library implementation
    void listenForMessages() {
        // TODO: Implement message listener using external library
        // This would run in a separate thread and invoke onReceive callback
    }
};

} // namespace realtime
} // namespace rooms
} // namespace multiplayer
} // namespace michitai
