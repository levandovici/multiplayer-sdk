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
    constexpr const char* GamePlayersRotate = "game_players.php/rotate";
    constexpr const char* GamePlayersDelete = "game_players.php/delete";
    constexpr const char* GamePlayersRestore = "game_players.php/restore";
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
    std::string steamId;
    std::string googlePlayerId;
    std::string playerTag; // public friend tag ("#XXXXXXXX")

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
        if (j.contains("steam_id") && !j["steam_id"].is_null()) {
            info.steamId = j["steam_id"].get<std::string>();
        }
        if (j.contains("google_player_id") && !j["google_player_id"].is_null()) {
            info.googlePlayerId = j["google_player_id"].get<std::string>();
        }
        if (j.contains("player_tag") && !j["player_tag"].is_null()) {
            info.playerTag = j["player_tag"].get<std::string>();
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
    std::string auth;      // "steam", "google_play" or "guest"
    std::string account;   // "created" or "existing"
    std::string steamId;   // SteamID64 when auth == "steam"
    std::string googlePlayerId; // Play Games player_id when auth == "google_play"
    std::string playerTag; // public friend tag ("#XXXXXXXX")

    static PlayerRegisterResponse fromJson(const nlohmann::json& j) {
        PlayerRegisterResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.playerId = j.value("player_id", 0);
        response.privateKey = j.value("private_key", "");
        response.playerName = j.value("player_name", "");
        response.gameId = j.value("game_id", 0);
        response.auth = j.value("auth", "");
        response.account = j.value("account", "");
        if (j.contains("steam_id") && !j["steam_id"].is_null()) {
            response.steamId = j["steam_id"].get<std::string>();
        }
        if (j.contains("google_player_id") && !j["google_player_id"].is_null()) {
            response.googlePlayerId = j["google_player_id"].get<std::string>();
        }
        if (j.contains("player_tag") && !j["player_tag"].is_null()) {
            response.playerTag = j["player_tag"].get<std::string>();
        }
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

/// Response for player key rotation. The previous token is invalidated;
/// store the returned new key.
struct PlayerRotateResponse : public ApiResponse {
    int playerId = 0;
    std::string privateKey;

    static PlayerRotateResponse fromJson(const nlohmann::json& j) {
        PlayerRotateResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.playerId = j.value("player_id", 0);
        response.privateKey = j.value("private_key", "");
        return response;
    }
};

/// Response for a scheduled player account deletion. The account is frozen
/// immediately and purged permanently after the grace window unless
/// restored — see restoreAccount().
struct PlayerDeleteResponse : public ApiResponse {
    int playerId = 0;
    std::string message;
    bool deletionPending = false;
    std::string purgeAfter;

    static PlayerDeleteResponse fromJson(const nlohmann::json& j) {
        PlayerDeleteResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.playerId = j.value("player_id", 0);
        response.message = j.value("message", "");
        response.deletionPending = j.value("deletion_pending", false);
        response.purgeAfter = j.value("purge_after", "");
        return response;
    }
};

/// Response for cancelling a pending account deletion inside the grace
/// window. The account is fully active again; the token was never
/// invalidated.
struct PlayerRestoreResponse : public ApiResponse {
    int playerId = 0;
    std::string message;

    static PlayerRestoreResponse fromJson(const nlohmann::json& j) {
        PlayerRestoreResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.playerId = j.value("player_id", 0);
        response.message = j.value("message", "");
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
    std::optional<std::string> steamTicket;
    std::optional<std::string> steamIdentity;
    std::optional<std::string> googleAuthCode;
    std::optional<std::string> googleIntegrityToken;
    bool restore = false;

    PlayerRegisterRequest(const std::string& name,
                          const std::optional<T>& data = std::nullopt,
                          const std::optional<std::string>& ticket = std::nullopt,
                          const std::optional<std::string>& identity = std::nullopt,
                          const std::optional<std::string>& authCode = std::nullopt,
                          const std::optional<std::string>& integrityToken = std::nullopt,
                          bool restore = false)
        : name(name), playerData(data), steamTicket(ticket), steamIdentity(identity),
          googleAuthCode(authCode), googleIntegrityToken(integrityToken), restore(restore) {}

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
        if (steamTicket.has_value()) {
            j["steam_ticket"] = steamTicket.value();
        }
        if (steamIdentity.has_value()) {
            j["steam_identity"] = steamIdentity.value();
        }
        if (googleAuthCode.has_value()) {
            j["google_auth_code"] = googleAuthCode.value();
        }
        if (googleIntegrityToken.has_value()) {
            j["google_integrity_token"] = googleIntegrityToken.value();
        }
        j["restore"] = restore;
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

    /// Registers or logs in a player via a Steam auth ticket. Requires the
    /// project to enable Steam accounts in the API cabinet. The ticket comes
    /// from ISteamUser::GetAuthTicketForWebApi, hex-encoded, and is
    /// single-use — request a fresh one for every login. The same call
    /// creates the account on first use and logs the player in afterwards,
    /// so the account survives reinstalls via the SteamID.
    /// @param client The API client instance
    /// @param steamTicket Hex-encoded ticket from GetAuthTicketForWebApi
    /// @param steamIdentity Identity string passed to GetAuthTicketForWebApi (omit if none was used)
    /// @param name Optional display name; the server generates one from the SteamID when omitted
    /// @param playerData Optional initial player data
    /// @param playerToken Optional existing player token; sent alongside the ticket the server
    ///        links the verified SteamID to that account instead of creating a second one
    /// @param restore Explicit user consent to cancel a pending account deletion. Set only
    ///        after the player confirms a restore prompt; the fresh provider ticket proves
    ///        identity even after a reinstall. Never set automatically.
    /// @return Response containing the player ID, private key token and steamId
    template<typename T = nlohmann::json>
    static PlayerRegisterResponse registerPlayerWithSteam(Client& client,
                                                          const std::string& steamTicket,
                                                          const std::optional<std::string>& steamIdentity = std::nullopt,
                                                          const std::optional<std::string>& name = std::nullopt,
                                                          const std::optional<T>& playerData = std::nullopt,
                                                          const std::string& playerToken = "",
                                                          bool restore = false) {
        PlayerRegisterRequest<T> request(name.value_or(""), playerData, steamTicket, steamIdentity,
                                         std::nullopt, std::nullopt, restore);
        return client.post<PlayerRegisterResponse>(
            client.url(Endpoints::GamePlayersRegister),
            request.toJson(), playerToken
        );
    }

    /// Registers or logs in a player via a Google Play Games Services server
    /// auth code. Requires the project to enable Google Play accounts in the
    /// API cabinet. The code comes from
    /// GamesSignInClient.requestServerSideAccess (minted for the project's
    /// OAuth web credential) and is single-use — request a fresh one for
    /// every login. The same call creates the account on first use and logs
    /// the player in afterwards, so the account survives reinstalls via the
    /// Play Games player_id.
    /// @param client The API client instance
    /// @param googleAuthCode Server auth code from requestServerSideAccess
    /// @param googleIntegrityToken Play Integrity token — required when the
    ///        project enables the ownership check (appLicensingVerdict=LICENSED)
    /// @param name Optional display name; the server generates one from the
    ///        player_id when omitted
    /// @param playerData Optional initial player data
    /// @param playerToken Optional existing player token; sent alongside the code the
    ///        server links the verified player_id to that account instead of
    ///        creating a second one
    /// @param restore Explicit user consent to cancel a pending account deletion. Set only
    ///        after the player confirms a restore prompt; the fresh provider credential proves
    ///        identity even after a reinstall. Never set automatically.
    /// @return Response containing the player ID, private key token and googlePlayerId
    template<typename T = nlohmann::json>
    static PlayerRegisterResponse registerPlayerWithGoogle(Client& client,
                                                          const std::string& googleAuthCode,
                                                          const std::optional<std::string>& googleIntegrityToken = std::nullopt,
                                                          const std::optional<std::string>& name = std::nullopt,
                                                          const std::optional<T>& playerData = std::nullopt,
                                                          const std::string& playerToken = "",
                                                          bool restore = false) {
        PlayerRegisterRequest<T> request(name.value_or(""), playerData, std::nullopt, std::nullopt,
                                         googleAuthCode, googleIntegrityToken, restore);
        return client.post<PlayerRegisterResponse>(
            client.url(Endpoints::GamePlayersRegister),
            request.toJson(), playerToken
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
            client.url(Endpoints::GamePlayersLogin),
            nlohmann::json{}, playerToken
        );
    }

    /// Sends a heartbeat to maintain the player's online status.
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @return Response confirming the heartbeat was received
    static PlayerHeartbeatResponse sendPlayerHeartbeat(Client& client,
                                                         const std::string& playerToken) {
        return client.post<PlayerHeartbeatResponse>(
            client.url(Endpoints::GamePlayersHeartbeat),
            nlohmann::json{}, playerToken
        );
    }

    /// Logs out a player from the game.
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @return Response confirming the logout
    static PlayerLogoutResponse logoutPlayer(Client& client,
                                               const std::string& playerToken) {
        return client.post<PlayerLogoutResponse>(
            client.url(Endpoints::GamePlayersLogout),
            nlohmann::json{}, playerToken
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
            client.url(Endpoints::GamePlayersRename),
            request.toJson(), playerToken
        );
    }

    /// Rotates the player's private key. The current token authenticates the
    /// request and is invalidated; persist the returned new key.
    /// @param client The API client instance
    /// @param playerToken The player's current private authentication token
    /// @return Response containing the new private key token
    static PlayerRotateResponse rotatePlayerKey(Client& client,
                                                const std::string& playerToken) {
        return client.post<PlayerRotateResponse>(
            client.url(Endpoints::GamePlayersRotate),
            nlohmann::json{}, playerToken
        );
    }

    /// Schedules deletion of the player's own account (store/data-rights
    /// compliance). Authenticated by the current token only — banned
    /// players can still delete their account. The account freezes immediately
    /// and is permanently purged after the grace window unless restored. Keep
    /// the credential during the window so the player can restore;
    /// provider-linked accounts can also restore through a fresh provider
    /// sign-in with the explicit restore flag after a reinstall.
    /// @param client The API client instance
    /// @param playerToken The player's private authentication token
    /// @return Response confirming the deletion
    static PlayerDeleteResponse deleteAccount(Client& client,
                                              const std::string& playerToken) {
        return client.del<PlayerDeleteResponse>(
            client.url(Endpoints::GamePlayersDelete),
            playerToken
        );
    }

    /// Cancels a pending account deletion inside the grace window.
    /// Authenticated by the player token — always a deliberate user action,
    /// never automatic, so background logins and heartbeats cannot silently
    /// undo a deletion the player actually wants.
    /// @param client Reference to the client instance
    /// @param playerToken The player's private authentication token
    /// @return Response confirming the restoration
    static PlayerRestoreResponse restoreAccount(Client& client,
                                                const std::string& playerToken) {
        return client.post<PlayerRestoreResponse>(
            client.url(Endpoints::GamePlayersRestore),
            nlohmann::json::object(),
            playerToken
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
            client.url(Endpoints::GamePlayersBan),
            request.toJson(), "", true
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
            client.url(Endpoints::GamePlayersUnban),
            request.toJson(), "", true
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
            client.url(Endpoints::GameDataPlayerGet), playerToken
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
            client.url(Endpoints::GameDataPlayerUpdate),
            jsonData, playerToken
        );
    }
};

} // namespace players
} // namespace multiplayer
} // namespace michitai
