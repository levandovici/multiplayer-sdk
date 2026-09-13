package com.michitai.multiplayer.rooms.updates;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.michitai.multiplayer.Client;

import java.io.IOException;

/**
 * Provides methods for managing room updates.
 * Handles sending updates to players and polling for received updates.
 */
public class Updates {
    private static final ObjectMapper objectMapper = Client.JSON_MAPPER;

    /**
     * Sends updates to specific players in the room.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param request The update request containing targets and data.
     * @return Response containing the update IDs and target player information.
     * @throws IOException if the request fails.
     */
    public static UpdatePlayersResponse updatePlayers(Client client, String playerToken, UpdatePlayers<?> request) throws IOException {
        String dataJson = request.getData() != null ? objectMapper.writeValueAsString(request.getData()) : null;
        UpdatePlayersRequest updateRequest = new UpdatePlayersRequest(
            request.getTargetPlayers(), 
            request.getType(), 
            dataJson, 
            request.getTargetPlayersIds()
        );
        return client.post(client.url(Endpoints.GAME_ROOM_UPDATES, "&player_token=" + playerToken), updateRequest, UpdatePlayersResponse.class);
    }

    /**
     * Polls for updates that were sent to the current player.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param request The poll request containing filters.
     * @return Response containing the received updates.
     * @throws IOException if the request fails.
     */
    public static PollUpdatesResponse<?> pollUpdates(Client client, String playerToken, PollUpdates request) throws IOException {
        return pollUpdates(client, playerToken, request, Object.class);
    }

    /**
     * Polls for updates that were sent to the current player.
     *
     * @param <T> The type to deserialize update data into.
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param request The poll request containing filters.
     * @param dataType The class to deserialize update data into.
     * @return Response containing the received updates with typed data.
     * @throws IOException if the request fails.
     */
    public static <T> PollUpdatesResponse<T> pollUpdates(Client client, String playerToken, PollUpdates request, Class<T> dataType) throws IOException {
        PollUpdatesRequest pollRequest = new PollUpdatesRequest(
            request.getFromPlayers(), 
            request.getFromPlayersIds(), 
            request.getLastUpdate()
        );
        return client.post(client.url(Endpoints.GAME_ROOM_UPDATES_POLL, "&player_token=" + playerToken), pollRequest,
            client.parametricType(PollUpdatesResponse.class, dataType));
    }
}
