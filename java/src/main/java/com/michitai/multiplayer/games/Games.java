package com.michitai.multiplayer.games;

import com.michitai.multiplayer.ApiResponse;
import com.michitai.multiplayer.Client;
import com.michitai.multiplayer.SuccessResponse;

import java.io.IOException;

/**
 * Provides methods for game-level operations including player listings and global game data management.
 */
public class Games {
    /**
     * Retrieves a list of all players in the game.
     * Requires the private API token for authentication.
     *
     * @param client The API client instance.
     * @return Response containing the list of all players with their basic information.
     * @throws IOException if the request fails.
     */
    public static PlayerListResponse getAllPlayers(Client client) throws IOException {
        return client.get(client.privateUrl(Endpoints.GAME_PLAYERS_LIST), PlayerListResponse.class);
    }

    /**
     * Retrieves global game data with typed deserialization support.
     *
     * @param client The API client instance.
     * @return Response containing the game data.
     * @throws IOException if the request fails.
     */
    public static GameDataResponse<?> getGameData(Client client) throws IOException {
        return getGameData(client, Object.class);
    }

    /**
     * Retrieves global game data with typed deserialization support.
     *
     * @param <T> The type to deserialize game data into.
     * @param client The API client instance.
     * @param dataType The class to deserialize game data into.
     * @return Response containing the game data.
     * @throws IOException if the request fails.
     */
    public static <T> GameDataResponse<T> getGameData(Client client, Class<T> dataType) throws IOException {
        return client.get(client.url(Endpoints.GAME_DATA_GAME_GET),
            client.parametricType(GameDataResponse.class, dataType));
    }

    /**
     * Updates global game data with the provided object.
     * Requires the private API token for authentication.
     *
     * @param client The API client instance.
     * @param data The game data object to update.
     * @return Success response confirming the update.
     * @throws IOException if the request fails.
     */
    public static SuccessResponse updateGameData(Client client, Object data) throws IOException {
        return client.put(client.privateUrl(Endpoints.GAME_DATA_GAME_UPDATE), data, SuccessResponse.class);
    }
}
