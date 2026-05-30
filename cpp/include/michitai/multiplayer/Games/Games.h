#pragma once

#include "../Client.h"
#include "../ApiResponse.h"
#include "../Types.h"
#include <string>
#include <vector>

namespace michitai {
namespace multiplayer {
namespace games {

// ====================== ENDPOINTS ======================

namespace Endpoints {
    constexpr const char* GamePlayersList = "game_players.php/list";
    constexpr const char* GameDataGameGet = "game_data.php/game/get";
    constexpr const char* GameDataGameUpdate = "game_data.php/game/update";
}

// ====================== RESPONSE TYPES ======================

/// Short player information structure
struct PlayerShort {
    int id = 0;
    std::string playerName;
    bool isOnline = false;
    std::string lastLogin;
    std::string createdAt;
    
    static PlayerShort fromJson(const nlohmann::json& j) {
        PlayerShort info;
        info.id = j.value("id", 0);
        info.playerName = j.value("player_name", "");
        info.isOnline = j.value("is_online", false);
        info.lastLogin = j.value("last_login", "");
        info.createdAt = j.value("created_at", "");
        return info;
    }
};

/// Response containing a list of all players in the game
struct PlayerListResponse : public ApiResponse {
    int count = 0;
    std::vector<PlayerShort> players;
    
    static PlayerListResponse fromJson(const nlohmann::json& j) {
        PlayerListResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.count = j.value("count", 0);
        
        if (j.contains("players") && j["players"].is_array()) {
            for (const auto& playerJson : j["players"]) {
                response.players.push_back(PlayerShort::fromJson(playerJson));
            }
        }
        
        return response;
    }
};

/// Response containing global game data with typed deserialization support
template<typename T = nlohmann::json>
struct GameDataResponse : public ApiResponse {
    std::string type;
    int gameId = 0;
    T gameData;
    
    static GameDataResponse fromJson(const nlohmann::json& j) {
        GameDataResponse response;
        response.success = j.value("success", false);
        response.error = j.value("error", "");
        response.type = j.value("type", "");
        response.gameId = j.value("game_id", 0);
        
        if (j.contains("data")) {
            if constexpr (std::is_same_v<T, nlohmann::json>) {
                response.gameData = j["data"];
            } else {
                response.gameData = j["data"].get<T>();
            }
        }
        
        return response;
    }
};

// ====================== GAMES CLASS ======================

/// Provides methods for game-level operations including player listings and global game data management
class Games {
public:
    /// Retrieves a list of all players in the game.
    /// Requires the private API token for authentication.
    /// @param client The API client instance
    /// @return Response containing the list of all players with their basic information
    static PlayerListResponse getAllPlayers(Client& client) {
        return client.get<PlayerListResponse>(
            client.privateUrl(Endpoints::GamePlayersList)
        );
    }
    
    /// Retrieves global game data with typed deserialization support.
    /// @param client The API client instance
    /// @return Response containing the game data deserialized into the specified type
    template<typename T = nlohmann::json>
    static GameDataResponse<T> getGameData(Client& client) {
        return client.get<GameDataResponse<T>>(
            client.url(Endpoints::GameDataGameGet)
        );
    }
    
    /// Updates global game data with the provided object.
    /// Requires the private API token for authentication.
    /// @param client The API client instance
    /// @param data The game data object to update
    /// @return Success response confirming the update
    template<typename T>
    static SuccessResponse updateGameData(Client& client, const T& data) {
        nlohmann::json jsonData;
        if constexpr (std::is_same_v<T, nlohmann::json>) {
            jsonData = data;
        } else {
            jsonData = nlohmann::json(data);
        }
        return client.put<SuccessResponse>(
            client.privateUrl(Endpoints::GameDataGameUpdate),
            jsonData
        );
    }
};

} // namespace games
} // namespace multiplayer
} // namespace michitai
