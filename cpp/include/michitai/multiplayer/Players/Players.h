#pragma once

#include "../Client.h"
#include "../ApiResponse.h"
#include "../Types.h"
#include <string>
#include <optional>

namespace michitai {
namespace multiplayer {
namespace players {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* GamePlayersRegister = "game_players.php/register";
    constexpr const char* GamePlayersLogin = "game_players.php/login";
    constexpr const char* GamePlayersHeartbeat = "game_players.php/heartbeat";
    constexpr const char* GamePlayersLogout = "game_players.php/logout";
    constexpr const char* GamePlayersRename = "game_players.php/rename";
    constexpr const char* GamePlayersBan = "game_players.php/ban";
    constexpr const char* GamePlayersUnban = "game_players.php/unban";
    constexpr const char* GameDataPlayerGet = "game_data.php/player/get";
    constexpr const char* GameDataPlayerUpdate = "game_data.php/player/update";
}

// ====================== RESPONSE TYPES ======================

/// Player information structure
struct PlayerInfo {
    int id = 0;
    std::string playerName;
    std::string privateKey;
    int gameId = 0;
    nlohmann::json playerData;
    bool isOnline = false;
    std::string lastLogin;
    std::string lastHeartbeat;
    std::string lastLogout;
    std::string createdAt;
    std::string updatedAt;
    
    static PlayerInfo fromJson(const nlohmann::json& j) {
        PlayerInfo info;
        info.id = j.value("id", 0);
        info.playerName = j.value("player_name", "");
        info.privateKey = j.value("private_key", "");
        info.gameId = j.value("game_id", 0);
        info.playerData = j.value("player_data", nlohmann::json::object());
        info.isOnline = j.value("is_online", false);
        
        // Handle null values for timestamp fields
        if (j.contains("last_login") && !j["last_login"].is_null()) {
            info.lastLogin = j["last_login"].get<std::string>();
        }
        if (j.contains("last_heartbeat") && !j["last_heartbeat"].is_null()) {
            info.lastHeartbeat = j["last_heartbeat"].get<std::string>();
        }
        if (j.contains("last_logout") && !j["last_logout"].is_null()) {
            info.lastLogout = j["last_logout"].get<std::string>();
        }
        if (j.contains("created_at") && !j["created_at"].is_null()) {
            info.createdAt = j["created_at"].get<std::string>();
        }
        if (j.contains("updated_at") && !j["updated_at"].is_null()) {
            info.updatedAt = j["updated_at"].get<std::string>();
        }
        
        return info;
    }
};

/// Response for player registration
struct PlayerRegisterResponse : public ApiResponse {
    int playerId = 0;
    std::string privateKey;
    std::string playerName;
    int gameId = 0;
    
    static PlayerRegisterResponse fromJson(const nlohmann::json& j) {
        PlayerRegisterResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.playerId = j.value("player_id", 0);
        response.privateKey = j.value("private_key", "");
        response.playerName = j.value("player_name", "");
        response.gameId = j.value("game_id", 0);
        return response;
    }
};

/// Response for player authentication
template<typename T = nlohmann::json>
struct PlayerAuthResponse : public ApiResponse {
    std::optional<PlayerInfo> player;
    
    static PlayerAuthResponse fromJson(const nlohmann::json& j) {
        PlayerAuthResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        if (j.contains("player") && !j["player"].is_null()) {
            response.player = PlayerInfo::fromJson(j["player"]);
        }
        return response;
    }
};

/// Response for player heartbeat
struct PlayerHeartbeatResponse : public ApiResponse {
    static PlayerHeartbeatResponse fromJson(const nlohmann::json& j) {
        PlayerHeartbeatResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for player logout
struct PlayerLogoutResponse : public ApiResponse {
    static PlayerLogoutResponse fromJson(const nlohmann::json& j) {
        PlayerLogoutResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for player rename
struct PlayerRenameResponse : public ApiResponse {
    static PlayerRenameResponse fromJson(const nlohmann::json& j) {
        PlayerRenameResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for player ban
struct PlayerBanResponse : public ApiResponse {
    int playerId = 0;
    std::string banDuration;
    std::string banReason;
    std::string bannedAt;
    std::string bannedUntil;
    
    static PlayerBanResponse fromJson(const nlohmann::json& j) {
        PlayerBanResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.playerId = j.value("player_id", 0);
        response.banDuration = j.value("ban_duration", "");
        response.banReason = j.value("ban_reason", "");
        response.bannedAt = j.value("banned_at", "");
        response.bannedUntil = j.value("banned_until", "");
        return response;
    }
};

/// Response for player unban
struct PlayerUnbanResponse : public ApiResponse {
    static PlayerUnbanResponse fromJson(const nlohmann::json& j) {
        PlayerUnbanResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        return response;
    }
};

/// Response for player data
template<typename T = nlohmann::json>
struct PlayerDataResponse : public ApiResponse {
    T playerData;
    
    static PlayerDataResponse fromJson(const nlohmann::json& j) {
        PlayerDataResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        if (j.contains("player_data")) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                response.playerData = j["player_data"];
            } else {
                // For custom types, assume they can be constructed from JSON
                // This is a simplification - in practice you'd need proper deserialization
                response.playerData = j["player_data"].get<T>();
            }
        }
        return response;
    }
};

// ====================== REQUEST TYPES ======================

/// Request for player registration
template<typename T = nlohmann::json>
struct PlayerRegisterRequest {
    std::string name;
    std::optional<T> playerData;
    
    PlayerRegisterRequest(const std::string& name, const std::optional<T>& data = std::nullopt)
        : name(name), playerData(data) {}
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        j["player_name"] = name;
        if (playerData.has_value()) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                j["player_data"] = playerData.value();
            } else {
                j["player_data"] = nlohmann::json(playerData.value());
            }
        }
        return j;
    }
};

/// Request for player rename
struct PlayerRenameRequest {
    std::string newName;
    
    explicit PlayerRenameRequest(const std::string& name) : newName(name) {}
    
    nlohmann::json toJson() const {
        return {{"new_name", newName}};
    }
};

/// Request for player ban
struct PlayerBanRequest {
    int playerId;
    BanTime banDuration;
    std::optional<std::string> banReason;
    
    PlayerBanRequest(int id, BanTime duration, const std::optional<std::string>& reason = std::nullopt)
        : playerId(id), banDuration(duration), banReason(reason) {}
    
    nlohmann::json toJson() const {
        nlohmann::json j;
        j["player_id"] = playerId;
        j["ban_duration"] = banTimeToString(banDuration);
        if (banReason.has_value()) {
            j["ban_reason"] = banReason.value();
        }
        return j;
    }
};

/// Request for player unban
struct PlayerUnbanRequest {
    int playerId;
    
    explicit PlayerUnbanRequest(int id) : playerId(id) {}
    
    nlohmann::json toJson() const {
        return {{"player_id", playerId}};
    }
};

// ====================== PLAYERS CLASS ======================

/// Provides methods for player management including registration, authentication,
/// data management, and administrative operations like banning.
class Players {
public:
    /// Registers a new player with the game.
    /// @param client The API client instance
    /// @param name The player's display name
    /// @param playerData Optional initial player data
    /// @return Response containing the player ID and private key token
    template<typename T = nlohmann::json>
    static PlayerRegisterResponse registerPlayer(Client& client, 
                                                   const std::string& name,
                                                   const std::optional<T>& playerData = std::nullopt) {
        PlayerRegisterRequest<T> request(name, playerData);
        return client.post<PlayerRegisterResponse>(
            client.url(Endpoints::GamePlayersRegister),
            request.toJson()
        );
    }
    
    /// Authenticates a player using their private token and retrieves their data.
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @return Response containing the authenticated player information
    template<typename T = nlohmann::json>
    static PlayerAuthResponse<T> authenticatePlayer(Client& client,
                                                      const std::string& playerToken) {
        return client.put<PlayerAuthResponse<T>>(
            client.url(Endpoints::GamePlayersLogin, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Sends a heartbeat to maintain the player's online status.
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @return Response confirming the heartbeat was received
    static PlayerHeartbeatResponse sendPlayerHeartbeat(Client& client,
                                                         const std::string& playerToken) {
        return client.post<PlayerHeartbeatResponse>(
            client.url(Endpoints::GamePlayersHeartbeat, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Logs out a player from the game.
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @return Response confirming the logout
    static PlayerLogoutResponse logoutPlayer(Client& client,
                                               const std::string& playerToken) {
        return client.post<PlayerLogoutResponse>(
            client.url(Endpoints::GamePlayersLogout, "&player_token=" + playerToken),
            nlohmann::json{}
        );
    }
    
    /// Renames a player to a new name (2-50 characters).
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @param newName The new name for the player
    /// @return Response confirming the name change
    static PlayerRenameResponse renamePlayer(Client& client,
                                              const std::string& playerToken,
                                              const std::string& newName) {
        PlayerRenameRequest request(newName);
        return client.put<PlayerRenameResponse>(
            client.url(Endpoints::GamePlayersRename, "&player_token=" + playerToken),
            request.toJson()
        );
    }
    
    /// Bans a player from the game with a specified duration.
    /// @param client The API client instance
    /// @param playerId The ID of the player to ban
    /// @param banDuration The duration of the ban
    /// @param banReason Optional reason for the ban
    /// @return Response containing the ban details
    static PlayerBanResponse banPlayer(Client& client,
                                        int playerId,
                                        BanTime banDuration,
                                        const std::optional<std::string>& banReason = std::nullopt) {
        PlayerBanRequest request(playerId, banDuration, banReason);
        return client.post<PlayerBanResponse>(
            client.privateUrl(Endpoints::GamePlayersBan),
            request.toJson()
        );
    }
    
    /// Unbans a previously banned player.
    /// @param client The API client instance
    /// @param playerId The ID of the player to unban
    /// @return Response confirming the player was unbanned
    static PlayerUnbanResponse unbanPlayer(Client& client,
                                            int playerId) {
        PlayerUnbanRequest request(playerId);
        return client.post<PlayerUnbanResponse>(
            client.privateUrl(Endpoints::GamePlayersUnban),
            request.toJson()
        );
    }
    
    /// Retrieves a player's data.
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @return Response containing the player's data
    template<typename T = nlohmann::json>
    static PlayerDataResponse<T> getPlayerData(Client& client,
                                                const std::string& playerToken) {
        return client.get<PlayerDataResponse<T>>(
            client.url(Endpoints::GameDataPlayerGet, "&player_token=" + playerToken)
        );
    }
    
    /// Updates a player's data.
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @param data The player data object to update
    /// @return Success response confirming the update
    template<typename T>
    static SuccessResponse updatePlayerData(Client& client,
                                            const std::string& playerToken,
                                            const T& data) {
        nlohmann::json jsonData;
        if constexpr (std::is_same_v<T, nlohmann::json>) {
            jsonData = data;
        } else {
            jsonData = nlohmann::json(data);
        }
        return client.put<SuccessResponse>(
            client.url(Endpoints::GameDataPlayerUpdate, "&player_token=" + playerToken),
            jsonData
        );
    }
};

} // namespace players
} // namespace multiplayer
} // namespace michitai
