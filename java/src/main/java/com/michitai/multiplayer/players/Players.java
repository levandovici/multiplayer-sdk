package com.michitai.multiplayer.players;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.michitai.multiplayer.ApiResponse;
import com.michitai.multiplayer.Client;
import com.michitai.multiplayer.SuccessResponse;

import java.io.IOException;

/**
 * Provides methods for player management including registration, authentication,
 * data management, and administrative operations like banning.
 */
public class Players {
    private static final ObjectMapper objectMapper = Client.JSON_MAPPER;

    /**
     * Registers a new player with the game.
     *
     * @param client The API client instance.
     * @param name The player's display name.
     * @param playerData Optional initial player data.
     * @return Response containing the player ID and private key token.
     * @throws IOException if the request fails.
     */
    public static PlayerRegisterResponse registerPlayer(Client client, String name, Object playerData) throws IOException {
        String dataJson = playerData != null ? objectMapper.writeValueAsString(playerData) : null;
        PlayerRegisterRequest request = new PlayerRegisterRequest(name, dataJson);
        return client.post(client.url(Endpoints.GAME_PLAYERS_REGISTER), request, PlayerRegisterResponse.class);
    }

    /**
     * Authenticates a player using their private token and retrieves their data.
     *
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @return Response containing the authenticated player information.
     * @throws IOException if the request fails.
     */
    public static PlayerAuthResponse<?> authenticatePlayer(Client client, String playerToken) throws IOException {
        return authenticatePlayer(client, playerToken, Object.class);
    }

    /**
     * Authenticates a player using their private token and retrieves their data.
     *
     * @param <T> The type to deserialize player data into.
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @param dataType The class to deserialize player data into.
     * @return Response containing the authenticated player information with typed player data.
     * @throws IOException if the request fails.
     */
    public static <T> PlayerAuthResponse<T> authenticatePlayer(Client client, String playerToken, Class<T> dataType) throws IOException {
        return client.send("PUT",client.url(Endpoints.GAME_PLAYERS_LOGIN), null,
            client.parametricType(PlayerAuthResponse.class, dataType), playerToken, false);
    }

    /**
     * Sends a heartbeat to maintain the player's online status.
     *
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @return Response confirming the heartbeat was received.
     * @throws IOException if the request fails.
     */
    public static PlayerHeartbeatResponse sendPlayerHeartbeat(Client client, String playerToken) throws IOException {
        return client.send("POST",client.url(Endpoints.GAME_PLAYERS_HEARTBEAT), null, PlayerHeartbeatResponse.class, playerToken, false);
    }

    /**
     * Logs out a player from the game.
     *
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @return Response confirming the logout.
     * @throws IOException if the request fails.
     */
    public static PlayerLogoutResponse logoutPlayer(Client client, String playerToken) throws IOException {
        return client.send("POST",client.url(Endpoints.GAME_PLAYERS_LOGOUT), null, PlayerLogoutResponse.class, playerToken, false);
    }

    /**
     * Renames a player to a new name (2-50 characters).
     *
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @param newName The new name for the player.
     * @return Response confirming the name change.
     * @throws IOException if the request fails.
     */
    public static PlayerRenameResponse renamePlayer(Client client, String playerToken, String newName) throws IOException {
        PlayerRenameRequest request = new PlayerRenameRequest(newName);
        return client.send("PUT",client.url(Endpoints.GAME_PLAYERS_RENAME), request, PlayerRenameResponse.class, playerToken, false);
    }

    /**
     * Rotates the player's private key. The current token authenticates the
     * request and is invalidated; persist the returned new key.
     *
     * @param client The API client instance.
     * @param playerToken The player's current private authentication token.
     * @return Response containing the new private key token.
     * @throws IOException if the request fails.
     */
    public static PlayerRotateResponse rotatePlayerKey(Client client, String playerToken) throws IOException {
        return client.send("POST",client.url(Endpoints.GAME_PLAYERS_ROTATE), null, PlayerRotateResponse.class, playerToken, false);
    }

    /**
     * Bans a player from the game with a specified duration.
     * Requires the private API token for authentication.
     *
     * @param client The API client instance.
     * @param playerId The ID of the player to ban.
     * @param banDuration The duration of the ban (HOUR, DAY, WEEK, MONTH, QUARTER, YEAR, FOREVER).
     * @param banReason Optional reason for the ban.
     * @return Response containing the ban details.
     * @throws IOException if the request fails.
     */
    public static PlayerBanResponse banPlayer(Client client, int playerId, String banDuration, String banReason) throws IOException {
        PlayerBanRequest request = new PlayerBanRequest(playerId, banDuration, banReason);
        return client.send("POST",client.url(Endpoints.GAME_PLAYERS_BAN), request, PlayerBanResponse.class, null, true);
    }

    /**
     * Bans a player from the game with a specified duration.
     * Requires the private API token for authentication.
     *
     * @param client The API client instance.
     * @param playerId The ID of the player to ban.
     * @param banDuration The duration of the ban.
     * @param banReason Optional reason for the ban.
     * @return Response containing the ban details.
     * @throws IOException if the request fails.
     */
    public static PlayerBanResponse banPlayer(Client client, int playerId, EBanTime banDuration, String banReason) throws IOException {
        return banPlayer(client, playerId, banDuration.name().toLowerCase(), banReason);
    }

    /**
     * Unbans a previously banned player.
     * Requires the private API token for authentication.
     *
     * @param client The API client instance.
     * @param playerId The ID of the player to unban.
     * @return Response confirming the player was unbanned.
     * @throws IOException if the request fails.
     */
    public static PlayerUnbanResponse unbanPlayer(Client client, int playerId) throws IOException {
        PlayerUnbanRequest request = new PlayerUnbanRequest(playerId);
        return client.send("POST",client.url(Endpoints.GAME_PLAYERS_UNBAN), request, PlayerUnbanResponse.class, null, true);
    }

    /**
     * Retrieves a player's data with typed deserialization support.
     *
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @return Response containing the player's data.
     * @throws IOException if the request fails.
     */
    public static PlayerDataResponse<?> getPlayerData(Client client, String playerToken) throws IOException {
        return getPlayerData(client, playerToken, Object.class);
    }

    /**
     * Retrieves a player's data with typed deserialization support.
     *
     * @param <T> The type to deserialize player data into.
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @param dataType The class to deserialize player data into.
     * @return Response containing the player's data.
     * @throws IOException if the request fails.
     */
    public static <T> PlayerDataResponse<T> getPlayerData(Client client, String playerToken, Class<T> dataType) throws IOException {
        return client.send("GET",client.url(Endpoints.GAME_DATA_PLAYER_GET), null,
            client.parametricType(PlayerDataResponse.class, dataType), playerToken, false);
    }

    /**
     * Updates a player's data with the provided object.
     *
     * @param client The API client instance.
     * @param playerToken The player's private authentication token.
     * @param data The player data object to update.
     * @return Success response confirming the update.
     * @throws IOException if the request fails.
     */
    public static SuccessResponse updatePlayerData(Client client, String playerToken, Object data) throws IOException {
        return client.send("PUT",client.url(Endpoints.GAME_DATA_PLAYER_UPDATE), data, SuccessResponse.class, playerToken, false);
    }
}
