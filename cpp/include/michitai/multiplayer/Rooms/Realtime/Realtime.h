#pragma once

#include "../../Client.h"
#include "../../ApiResponse.h"
#include "../../Types.h"
#include <string>
#include <vector>
#include <functional>
#include <memory>
#include <thread>
#include <atomic>
#include <mutex>
#include <chrono>
#include <curl/curl.h>

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

/// Manages WebSocket connections for realtime communication in game rooms.
/// Implemented on libcurl's WebSocket API (curl_ws_send/curl_ws_recv,
/// curl >= 7.86 — bundled by cpr), which gives ws:// and wss:// (TLS)
/// support. Requires curl built with WebSocket support — CMakeLists sets
/// ENABLE_WEBSOCKETS/CPR_ENABLE_CURL_HTTP_ONLY for the bundled build.
class Realtime {
public:
    /// Callback type for receive events
    using ReceiveCallback = std::function<void(const std::string&, const nlohmann::json&, const SenderInfo&)>;

    /// Callback type for connected events
    using ConnectedCallback = std::function<void()>;

    /// Callback type for disconnect events
    using DisconnectedCallback = std::function<void()>;

    /// Constructor
    /// @param realtimeWebSocketUrl The WebSocket server URL (default: "wss://realtime.michitai.com")
    Realtime(const std::string& realtimeWebSocketUrl = "wss://realtime.michitai.com")
        : realtimeWebSocketUrl(realtimeWebSocketUrl) {}

    ~Realtime() {
        disconnect();
    }

    // Non-copyable — owns a socket + threads.
    Realtime(const Realtime&) = delete;
    Realtime& operator=(const Realtime&) = delete;

    /// Retrieves a realtime authentication token for WebSocket connections
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @return Response containing the realtime token
    static TokenResponse getToken(Client& client, const std::string& playerToken) {
        return client.post<TokenResponse>(
            client.url(Endpoints::RealtimeToken), nlohmann::json{}, playerToken
        );
    }

    /// Constructs a WebSocket URL from server info
    /// @param serverInfo The realtime server information
    /// @param clientType The client type (default: "json")
    /// @return WebSocket URL (token goes in the X-Realtime-Token header)
    static std::string buildWebSocketUrl(const RealtimeServerInfo& serverInfo,
                                         const std::string& clientType = "json") {
        return serverInfo.protocol + "://" + serverInfo.host + ":" +
               std::to_string(serverInfo.port) + "/?client=" + clientType;
    }

    /// True while the socket is open.
    bool isConnected() const { return connected.load(); }

    /// Connects to the realtime WebSocket server using the provided token.
    /// Sends X-Realtime-Token as a header (never in the URL).
    /// @param realtimeToken The realtime authentication token
    /// @return True if the WS handshake succeeded, false otherwise
    bool connect(const std::string& realtimeToken) {
        disconnect();

        curlVersionInfo* vinfo = curl_version_info(CURLVERSION_NOW);
        if (!(vinfo->features & CURL_VERSION_WS)) {
            return false; // curl built without WebSocket support
        }

        CURL* handle = curl_easy_init();
        if (!handle) return false;

        std::string url = realtimeWebSocketUrl;
        url += (url.find('?') == std::string::npos) ? "?client=json" : "&client=json";

        struct curl_slist* headers = nullptr;
        const std::string authHeader = "X-Realtime-Token: " + realtimeToken;
        headers = curl_slist_append(headers, authHeader.c_str());

        curl_easy_setopt(handle, CURLOPT_URL, url.c_str());
        curl_easy_setopt(handle, CURLOPT_HTTPHEADER, headers);
        curl_easy_setopt(handle, CURLOPT_CONNECT_ONLY, 2L);
        curl_easy_setopt(handle, CURLOPT_TIMEOUT, 15L);
        curl_easy_setopt(handle, CURLOPT_NOSIGNAL, 1L);

        CURLcode res = curl_easy_perform(handle);
        if (res != CURLE_OK) {
            curl_slist_free_all(headers);
            curl_easy_cleanup(handle);
            return false;
        }

        curl_socket_t sock = CURL_SOCKET_BAD;
        if (curl_easy_getinfo(handle, CURLINFO_ACTIVESOCKET, &sock) != CURLE_OK
            || sock == CURL_SOCKET_BAD) {
            curl_slist_free_all(headers);
            curl_easy_cleanup(handle);
            return false;
        }

        curl = handle;
        curlHeaders = headers;
        socketFd = sock;
        token = realtimeToken;
        running = true;
        connected = true;

        readerThread = std::thread(&Realtime::listenForMessages, this);
        heartbeatThread = std::thread([this] {
            while (running.load()) {
                for (int i = 0; i < 20 && running.load(); ++i) {
                    std::this_thread::sleep_for(std::chrono::seconds(1));
                }
                if (running.load()) sendHeartbeat();
            }
        });

        if (onConnected) onConnected();
        return true;
    }

    /// Sends a message to the specified players via WebSocket
    /// @param target The target players (All, Host, Others, Specific)
    /// @param command The command/type of the message
    /// @param data Optional data payload to send
    /// @param targetIds Specific player IDs if target is Specific
    bool send(RoomTargetPlayer target, const std::string& command,
              const nlohmann::json& data = nlohmann::json::object(),
              const std::vector<int>& targetIds = {}) {
        if (!connected.load()) return false;

        nlohmann::json message;
        message["type"] = "send";
        message["command"] = command;
        message["data"] = data;
        message["target"] = roomTargetPlayerToString(target);
        if (!targetIds.empty()) message["target_ids"] = targetIds;

        return sendText(message.dump());
    }

    /// Disconnects from the WebSocket server and cleans up resources
    void disconnect() {
        if (running.exchange(false)) {
            {
                std::lock_guard<std::mutex> lk(ioMutex);
                if (curl) {
                    size_t sent = 0;
                    curl_ws_send(curl, "", 0, &sent, 0, CURLWS_CLOSE);
                }
            }
            if (readerThread.joinable()) readerThread.join();
            if (heartbeatThread.joinable()) heartbeatThread.join();
        }
        {
            std::lock_guard<std::mutex> lk(ioMutex);
            if (curlHeaders) { curl_slist_free_all(curlHeaders); curlHeaders = nullptr; }
            if (curl) { curl_easy_cleanup(curl); curl = nullptr; }
        }
        connected = false;
    }

    /// Sets the callback for receive events
    /// @param callback The function to call when a message is received
    void setReceiveCallback(ReceiveCallback callback) {
        onReceive = std::move(callback);
    }

    /// Sets the callback for connected events
    /// @param callback The function to call when connection is established
    void setConnectedCallback(ConnectedCallback callback) {
        onConnected = std::move(callback);
    }

    /// Sets the callback for disconnect events (socket closed or lost)
    /// @param callback The function to call when the connection drops
    void setDisconnectedCallback(DisconnectedCallback callback) {
        onDisconnected = std::move(callback);
    }

private:
    std::string realtimeWebSocketUrl;
    std::string token;
    std::atomic<bool> connected{false};
    std::atomic<bool> running{false};
    CURL* curl = nullptr;
    struct curl_slist* curlHeaders = nullptr;
    curl_socket_t socketFd = CURL_SOCKET_BAD;
    std::thread readerThread;
    std::thread heartbeatThread;
    std::mutex ioMutex; // serializes all curl handle access
    ReceiveCallback onReceive;
    ConnectedCallback onConnected;
    DisconnectedCallback onDisconnected;

    /// Sends a text frame; the handle is shared, so all io is mutex-guarded.
    bool sendText(const std::string& payload) {
        std::lock_guard<std::mutex> lk(ioMutex);
        if (!curl) return false;
        size_t sent = 0;
        CURLcode res = curl_ws_send(curl, payload.data(), payload.size(),
                                    &sent, 0, CURLWS_TEXT);
        return res == CURLE_OK && sent == payload.size();
    }

    /// Sends a heartbeat message to keep the connection alive
    void sendHeartbeat() {
        if (!connected.load()) return;
        sendText("{\"type\":\"heartbeat\"}");
    }

    /// Waits for socket readability; select() works on both POSIX and
    /// Winsock sockets.
    bool waitReadable(int timeoutMs) {
        fd_set rfds;
        FD_ZERO(&rfds);
        FD_SET(socketFd, &rfds);
        timeval tv{timeoutMs / 1000, (timeoutMs % 1000) * 1000};
#ifdef _WIN32
        return select(0, &rfds, nullptr, nullptr, &tv) > 0;
#else
        return select(static_cast<int>(socketFd) + 1, &rfds, nullptr, nullptr, &tv) > 0;
#endif
    }

    /// Receive loop: collects (possibly fragmented) text messages, invokes
    /// onReceive for "receive" events. Runs on readerThread.
    void listenForMessages() {
        std::string buffer;
        while (running.load()) {
            if (!waitReadable(500)) continue;

            char chunk[16384];
            size_t received = 0;
            const curl_ws_frame* meta = nullptr;
            {
                std::lock_guard<std::mutex> lk(ioMutex);
                if (!curl) break;
                CURLcode res = curl_ws_recv(curl, chunk, sizeof(chunk), &received, &meta);
                if (res == CURLE_AGAIN) continue;
                if (res != CURLE_OK) break;
            }

            if (meta->flags & CURLWS_CLOSE) break;
            if (meta->flags & (CURLWS_TEXT | CURLWS_CONT | CURLWS_BINARY)) {
                buffer.append(chunk, received);
                if (meta->bytesleft == 0) {
                    handleIncoming(buffer);
                    buffer.clear();
                }
            }
        }

        connected = false;
        running = false;
        if (onDisconnected) onDisconnected();
    }

    void handleIncoming(const std::string& text) {
        nlohmann::json j;
        try {
            j = nlohmann::json::parse(text);
        } catch (...) {
            return;
        }
        const std::string type = j.value("type", "");
        if (type == "receive" && onReceive) {
            RealtimeMessage msg = RealtimeMessage::fromJson(j);
            onReceive(msg.command, msg.data, msg.sender);
        }
        // "sent", "heartbeat_ack", "error" need no dispatch.
    }
};

} // namespace realtime
} // namespace rooms
} // namespace multiplayer
} // namespace michitai
