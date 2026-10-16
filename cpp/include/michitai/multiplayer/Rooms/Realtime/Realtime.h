#pragma once

#include "../../Client.h"
#include "../../ApiResponse.h"
#include "../../Types.h"
// UDP companion channel — plain BSD sockets on both platforms.
// winsock2.h must precede anything that may pull in windows.h (curl).
#ifdef _WIN32
#include <winsock2.h>
#include <ws2tcpip.h>
#endif

#include <string>
#include <vector>
#include <functional>
#include <memory>
#include <thread>
#include <atomic>
#include <mutex>
#include <chrono>
#include <cstring>
#include <curl/curl.h>

#ifndef _WIN32
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <unistd.h>
typedef int SOCKET;
constexpr SOCKET INVALID_SOCKET = -1;
#endif

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
    int udpPort = 0;  // unreliable datagram channel (0 = unavailable)
    
    static RealtimeServerInfo fromJson(const nlohmann::json& j) {
        RealtimeServerInfo info;
        info.host = j.value("host", "");
        info.port = j.value("port", 0);
        info.protocol = j.value("protocol", "");
        info.udpPort = j.value("udp_port", 0);
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
    // Sender-assigned sequence on unreliable (UDP) traffic — drop
    // messages whose seq is not newer than the last seen.
    uint32_t seq = 0;
    
    static RealtimeMessage fromJson(const nlohmann::json& j) {
        RealtimeMessage msg;
        msg.type = j.value("type", "");
        msg.command = j.value("command", "");
        msg.data = j.value("data", nlohmann::json::object());
        if (j.contains("sender")) {
            msg.sender = SenderInfo::fromJson(j["sender"]);
        }
        msg.seq = j.value("seq", 0u);
        return msg;
    }
};

// MUR1 datagram codec — [4B magic][1B type][16B connection id][JSON].
// Shared layout with realtime/src/protocol.js.
namespace udpwire {
    constexpr size_t HeaderSize = 21;
    enum class PacketType : uint8_t {
        Bind = 1, BindAck = 2, Msg = 3, Ping = 4, Pong = 5
    };

    inline bool uuidToBytes(const std::string& uuid, uint8_t out[16]) {
        std::string hex;
        hex.reserve(32);
        for (char c : uuid) {
            if (c != '-') hex.push_back(c);
        }
        if (hex.size() != 32) return false;
        auto val = [](char c) -> int {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            return -1;
        };
        for (int i = 0; i < 16; ++i) {
            int hi = val(hex[i * 2]), lo = val(hex[i * 2 + 1]);
            if (hi < 0 || lo < 0) return false;
            out[i] = static_cast<uint8_t>((hi << 4) | lo);
        }
        return true;
    }

    inline std::string bytesToUuid(const uint8_t* b) {
        static const char* digits = "0123456789abcdef";
        std::string s;
        s.reserve(36);
        for (int i = 0; i < 16; ++i) {
            if (i == 4 || i == 6 || i == 8 || i == 10) s.push_back('-');
            s.push_back(digits[b[i] >> 4]);
            s.push_back(digits[b[i] & 0xF]);
        }
        return s;
    }

    inline std::vector<uint8_t> encode(PacketType type, const std::string& connectionId,
                                       const std::string& jsonBody) {
        uint8_t id[16];
        if (!uuidToBytes(connectionId, id)) return {};
        std::vector<uint8_t> buf(HeaderSize + jsonBody.size());
        buf[0] = 'M'; buf[1] = 'U'; buf[2] = 'R'; buf[3] = '1';
        buf[4] = static_cast<uint8_t>(type);
        std::memcpy(buf.data() + 5, id, 16);
        if (!jsonBody.empty()) {
            std::memcpy(buf.data() + HeaderSize, jsonBody.data(), jsonBody.size());
        }
        return buf;
    }

    /// Returns the packet type, or 0 when the datagram is not MUR1.
    /// On success writes the connection id and body slice.
    inline uint8_t decode(const uint8_t* data, size_t size,
                          std::string& connectionIdOut, std::string& bodyOut) {
        if (!data || size < HeaderSize) return 0;
        if (data[0] != 'M' || data[1] != 'U' || data[2] != 'R' || data[3] != '1') return 0;
        connectionIdOut = bytesToUuid(data + 5);
        bodyOut.assign(reinterpret_cast<const char*>(data + HeaderSize), size - HeaderSize);
        return data[4];
    }
} // namespace udpwire

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

/// Manages realtime communication in game rooms over two channels:
/// a reliable WebSocket (TCP) connection for control traffic plus one
/// optional bound UDP address for high-frequency unreliable traffic
/// (movement state, inputs). One player may hold one TCP connection
/// and one UDP binding at the same time — see connectUdp().
/// The WebSocket is implemented on libcurl's WebSocket API
/// (curl_ws_send/curl_ws_recv, curl >= 7.86 — bundled by cpr), which
/// gives ws:// and wss:// (TLS) support. Requires curl built with
/// WebSocket support — CMakeLists sets ENABLE_WEBSOCKETS/
/// CPR_ENABLE_CURL_HTTP_ONLY for the bundled build.
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

    /// The connection id issued by the server (empty before connect).
    std::string connectionId() const { return connectionId_; }

    /// True while a UDP address is bound to this session.
    bool isUdpConnected() const { return udpBound.load(); }

    /// Last measured UDP round-trip time in milliseconds (0 = unknown).
    int lastUdpRttMs() const { return lastUdpRttMs.load(); }

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

    /// Binds a UDP endpoint to this session. Call after connect(); the
    /// server accepts one UDP binding per connection. Datagrams are
    /// authenticated with the connection id issued on the socket.
    /// @param udpPort 0 = the udp_port announced in the connected payload
    /// @return True when the server acknowledged the binding.
    bool connectUdp(int udpPort = 0) {
        if (!connected.load() || connectionId_.empty()) return false;
        if (udpBound.load()) return true;

        // The WS port is a proxy port for wss:// — UDP needs the service
        // port announced by the server (3000 by default).
        int port = udpPort > 0 ? udpPort : (serverUdpPort > 0 ? serverUdpPort : 3000);

        if (!initWinsock()) return false;

        SOCKET sock = ::socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
        if (sock == INVALID_SOCKET) return false;

        // Resolve the realtime host and set it as the default peer.
        addrinfo hints{};
        hints.ai_family = AF_INET;
        hints.ai_socktype = SOCK_DGRAM;
        addrinfo* res = nullptr;
        if (getaddrinfo(realtimeHost().c_str(), std::to_string(port).c_str(),
                        &hints, &res) != 0 || !res) {
            closeSocket(sock);
            return false;
        }
        if (::connect(sock, res->ai_addr, static_cast<int>(res->ai_addrlen)) != 0) {
            freeaddrinfo(res);
            closeSocket(sock);
            return false;
        }
        freeaddrinfo(res);

        {
            std::lock_guard<std::mutex> g(udpMutex);
            udpSocket = sock;
        }
        udpReaderThread = std::thread(&Realtime::udpReadLoop, this);

        // BIND is unreliable — retry once before giving up.
        auto bindPacket = udpwire::encode(udpwire::PacketType::Bind, connectionId_, "{\"v\":1}");
        for (int attempt = 0; attempt < 2 && !udpBound.load(); ++attempt) {
            ::send(sock, reinterpret_cast<const char*>(bindPacket.data()),
                   static_cast<int>(bindPacket.size()), 0);
            auto deadline = std::chrono::steady_clock::now() + std::chrono::milliseconds(1500);
            while (!udpBound.load() && std::chrono::steady_clock::now() < deadline) {
                std::this_thread::sleep_for(std::chrono::milliseconds(25));
            }
        }
        if (!udpBound.load()) {
            {
                std::lock_guard<std::mutex> g(udpMutex);
                closeSocket(udpSocket);
                udpSocket = INVALID_SOCKET;
            }
            if (udpReaderThread.joinable()) udpReaderThread.join();
            return false;
        }
        return true;
    }

    /// Sends over the bound UDP channel — unordered, unacknowledged,
    /// latest-wins via seq. Best for movement/state streams.
    /// Falls back to the WebSocket when UDP is not bound.
    bool sendUnreliable(RoomTargetPlayer target, const std::string& command,
                        const nlohmann::json& data = nlohmann::json::object(),
                        const std::vector<int>& targetIds = {}) {
        if (!udpBound.load() || connectionId_.empty()) {
            return send(target, command, data, targetIds);
        }

        nlohmann::json message;
        message["type"] = "send";
        message["command"] = command;
        message["data"] = data;
        message["seq"] = ++udpSeq;
        message["target"] = roomTargetPlayerToString(target);
        if (!targetIds.empty()) message["target_ids"] = targetIds;

        auto packet = udpwire::encode(udpwire::PacketType::Msg, connectionId_, message.dump());
        if (packet.empty()) return false;
        std::lock_guard<std::mutex> lk(udpMutex);
        if (udpSocket == INVALID_SOCKET) return false;
        int sent = ::send(udpSocket, reinterpret_cast<const char*>(packet.data()),
                          static_cast<int>(packet.size()), 0);
        return sent == static_cast<int>(packet.size());
    }

    /// Disconnects from the server and cleans up both channels
    void disconnect() {
        udpBound = false;
        {
            // Closing the socket unblocks udpReadLoop's recv.
            std::lock_guard<std::mutex> lk(udpMutex);
            if (udpSocket != INVALID_SOCKET) {
                closeSocket(udpSocket);
                udpSocket = INVALID_SOCKET;
            }
        }
        if (udpReaderThread.joinable()) udpReaderThread.join();

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

    // UDP companion channel state
    std::string connectionId_;
    int serverUdpPort = 0;
    SOCKET udpSocket = INVALID_SOCKET;
    std::thread udpReaderThread;
    std::mutex udpMutex;
    std::atomic<bool> udpBound{false};
    std::atomic<uint32_t> udpSeq{0};
    std::atomic<int> lastUdpRttMs{0};

    /// Winsock is a process-wide init on Windows; curl usually beats us
    /// to it, but don't rely on that.
    static bool initWinsock() {
#ifdef _WIN32
        static std::once_flag once;
        static bool ok = false;
        std::call_once(once, [] {
            WSADATA wsa;
            ok = (WSAStartup(MAKEWORD(2, 2), &wsa) == 0);
        });
        return ok;
#else
        return true;
#endif
    }

    static void closeSocket(SOCKET s) {
#ifdef _WIN32
        closesocket(s);
#else
        ::close(s);
#endif
    }

    /// Host part of realtimeWebSocketUrl (ws://host:port/...)
    std::string realtimeHost() const {
        std::string url = realtimeWebSocketUrl;
        auto pos = url.find("://");
        if (pos != std::string::npos) url = url.substr(pos + 3);
        auto slash = url.find('/');
        if (slash != std::string::npos) url = url.substr(0, slash);
        auto colon = url.rfind(':');
        if (colon != std::string::npos) url = url.substr(0, colon);
        return url;
    }

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

        // Keep the NAT mapping warm on the UDP channel too.
        if (udpBound.load() && !connectionId_.empty()) {
            auto nowMs = std::chrono::duration_cast<std::chrono::milliseconds>(
                std::chrono::system_clock::now().time_since_epoch()).count();
            auto ping = udpwire::encode(udpwire::PacketType::Ping, connectionId_,
                "{\"t\":" + std::to_string(nowMs) + "}");
            std::lock_guard<std::mutex> lk(udpMutex);
            if (udpSocket != INVALID_SOCKET) {
                ::send(udpSocket, reinterpret_cast<const char*>(ping.data()),
                       static_cast<int>(ping.size()), 0);
            }
        }
    }

    /// UDP receive loop — dispatches MSG datagrams to onReceive, tracks
    /// BIND_ACK / PONG. Runs on udpReaderThread until the socket closes.
    void udpReadLoop() {
        uint8_t buf[2048];
        while (true) {
            SOCKET sock;
            {
                std::lock_guard<std::mutex> g(udpMutex);
                sock = udpSocket;
            }
            if (sock == INVALID_SOCKET) break;

            fd_set rfds;
            FD_ZERO(&rfds);
            FD_SET(sock, &rfds);
            timeval tv{0, 200000}; // 200ms — exit checks the socket state
#ifdef _WIN32
            int ready = select(0, &rfds, nullptr, nullptr, &tv);
#else
            int ready = select(static_cast<int>(sock) + 1, &rfds, nullptr, nullptr, &tv);
#endif
            if (ready <= 0) {
                if (!isConnected() && !running.load()) break;
                continue;
            }

            int n = ::recv(sock, reinterpret_cast<char*>(buf), sizeof(buf), 0);
            if (n <= 0) break;

            std::string connId, body;
            uint8_t type = udpwire::decode(buf, static_cast<size_t>(n), connId, body);
            if (type == 0 || connId != connectionId_) continue;

            switch (static_cast<udpwire::PacketType>(type)) {
                case udpwire::PacketType::BindAck: {
                    try {
                        auto j = nlohmann::json::parse(body);
                        if (j.value("ok", false)) udpBound = true;
                    } catch (...) {}
                    break;
                }
                case udpwire::PacketType::Msg: {
                    try {
                        auto j = nlohmann::json::parse(body);
                        if (j.value("type", "") == "receive" && onReceive) {
                            RealtimeMessage msg = RealtimeMessage::fromJson(j);
                            onReceive(msg.command, msg.data, msg.sender);
                        }
                    } catch (...) {}
                    break;
                }
                case udpwire::PacketType::Pong: {
                    try {
                        auto j = nlohmann::json::parse(body);
                        auto sent = j.value("t", (int64_t)0);
                        auto nowMs = std::chrono::duration_cast<std::chrono::milliseconds>(
                            std::chrono::system_clock::now().time_since_epoch()).count();
                        if (sent > 0) {
                            lastUdpRttMs = static_cast<int>(
                                std::max<int64_t>(0, nowMs - sent));
                        }
                    } catch (...) {}
                    break;
                }
                default:
                    break;
            }
        }
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
        if (type == "connected" && j.contains("data")) {
            const auto& d = j["data"];
            connectionId_ = d.value("connection_id", "");
            serverUdpPort = d.value("udp_port", 0);
        } else if (type == "receive" && onReceive) {
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
