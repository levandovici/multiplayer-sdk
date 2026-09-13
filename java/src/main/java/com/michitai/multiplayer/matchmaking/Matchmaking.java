package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.michitai.multiplayer.ApiResponse;
import com.michitai.multiplayer.Client;
import com.michitai.multiplayer.SuccessResponse;

import java.io.IOException;

/**
 * Provides methods for matchmaking lobby management including creation,
 * joining, player management, and game session initiation.
 */
public class Matchmaking {
    private static final ObjectMapper objectMapper = Client.JSON_MAPPER;

    /**
     * Retrieves a list of available matchmaking lobbies.
     *
     * @param client The API client instance.
     * @param search Optional search term to filter lobbies.
     * @param limit Optional limit on the number of results.
     * @return Response containing the list of matchmaking lobbies.
     * @throws IOException if the request fails.
     */
    public static MatchmakingListResponse<?> getMatchmakingLobbies(Client client, String search, Integer limit) throws IOException {
        return getMatchmakingLobbies(client, search, limit, Object.class);
    }

    /**
     * Retrieves a list of available matchmaking lobbies.
     *
     * @param <T> The type to deserialize lobby rules into.
     * @param client The API client instance.
     * @param search Optional search term to filter lobbies.
     * @param limit Optional limit on the number of results.
     * @param rulesType The class to deserialize lobby rules into.
     * @return Response containing the list of matchmaking lobbies with typed rules.
     * @throws IOException if the request fails.
     */
    public static <T> MatchmakingListResponse<T> getMatchmakingLobbies(Client client, String search, Integer limit, Class<T> rulesType) throws IOException {
        MatchmakingListRequest request = new MatchmakingListRequest(search, limit);
        return client.post(client.url(Endpoints.MATCHMAKING_LIST), request,
            client.parametricType(MatchmakingListResponse.class, rulesType));
    }

    /**
     * Creates a new matchmaking lobby with specified configuration.
     *
     * @param client The API client instance.
     * @param playerToken The host's player token.
     * @param matchmakingName The name of the matchmaking lobby.
     * @param maxPlayers Maximum number of players allowed.
     * @param strictFull Whether the lobby must be full to start.
     * @param joinByRequests Whether players must request to join and be approved.
     * @param hostSwitch Whether host switching is allowed.
     * @param canLeaveRoom Whether players can leave the resulting room.
     * @param realtimeRoom Whether the resulting room supports realtime.
     * @param password Optional password for the lobby.
     * @param playerData Optional player data for the host.
     * @param rules Optional game rules for the lobby.
     * @return Response containing the matchmaking lobby ID.
     * @throws IOException if the request fails.
     */
    public static MatchmakingCreateResponse createMatchmakingLobby(Client client, String playerToken, String matchmakingName,
            int maxPlayers, boolean strictFull, boolean joinByRequests, boolean hostSwitch, boolean canLeaveRoom,
            boolean realtimeRoom, String password, Object playerData, Object rules) throws IOException {
        String playerDataJson = playerData != null ? objectMapper.writeValueAsString(playerData) : null;
        String rulesJson = rules != null ? objectMapper.writeValueAsString(rules) : null;
        
        MatchmakingCreateRequest request = new MatchmakingCreateRequest(
            matchmakingName, maxPlayers, strictFull, joinByRequests, hostSwitch, canLeaveRoom, 
            realtimeRoom, password, playerDataJson, rulesJson);
        
        return client.post(client.url(Endpoints.MATCHMAKING_CREATE, "&player_token=" + playerToken), request, MatchmakingCreateResponse.class);
    }

    /**
     * Gets the current status of the player's matchmaking lobby.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @return Response containing the current lobby status and information.
     * @throws IOException if the request fails.
     */
    public static MatchmakingCurrentResponse<?> getCurrentMatchmakingStatus(Client client, String playerToken) throws IOException {
        return getCurrentMatchmakingStatus(client, playerToken, Object.class);
    }

    /**
     * Gets the current status of the player's matchmaking lobby.
     *
     * @param <T> The type to deserialize lobby rules into.
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param rulesType The class to deserialize lobby rules into.
     * @return Response containing the current lobby status and information with typed rules.
     * @throws IOException if the request fails.
     */
    public static <T> MatchmakingCurrentResponse<T> getCurrentMatchmakingStatus(Client client, String playerToken, Class<T> rulesType) throws IOException {
        return client.get(client.url(Endpoints.MATCHMAKING_CURRENT, "&player_token=" + playerToken),
            client.parametricType(MatchmakingCurrentResponse.class, rulesType));
    }

    /**
     * Joins a matchmaking lobby directly (without approval).
     * Only works if the lobby doesn't require host approval.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param matchmakingId The ID of the matchmaking lobby to join.
     * @param playerData Optional player data to include when joining.
     * @return Response confirming the player joined the lobby.
     * @throws IOException if the request fails.
     */
    public static MatchmakingDirectJoinResponse joinMatchmakingDirectly(Client client, String playerToken, String matchmakingId, Object playerData) throws IOException {
        String endpoint = String.format(Endpoints.MATCHMAKING_JOIN, matchmakingId);
        return client.post(client.url(endpoint, "&player_token=" + playerToken), playerData, MatchmakingDirectJoinResponse.class);
    }

    /**
     * Leaves the current matchmaking lobby.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @return Response confirming the player left the lobby.
     * @throws IOException if the request fails.
     */
    public static MatchmakingLeaveResponse leaveMatchmaking(Client client, String playerToken) throws IOException {
        return client.post(client.url(Endpoints.MATCHMAKING_LEAVE, "&player_token=" + playerToken), null, MatchmakingLeaveResponse.class);
    }

    /**
     * Gets the list of players in the current matchmaking lobby.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @return Response containing the list of players in the lobby.
     * @throws IOException if the request fails.
     */
    public static MatchmakingPlayersResponse<?> getMatchmakingPlayers(Client client, String playerToken) throws IOException {
        return getMatchmakingPlayers(client, playerToken, Object.class);
    }

    /**
     * Gets the list of players in the current matchmaking lobby.
     *
     * @param <T> The type to deserialize player data into.
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param dataType The class to deserialize player data into.
     * @return Response containing the list of players in the lobby with typed player data.
     * @throws IOException if the request fails.
     */
    public static <T> MatchmakingPlayersResponse<T> getMatchmakingPlayers(Client client, String playerToken, Class<T> dataType) throws IOException {
        return client.get(client.url(Endpoints.MATCHMAKING_PLAYERS, "&player_token=" + playerToken),
            client.parametricType(MatchmakingPlayersResponse.class, dataType));
    }

    /**
     * Sends a heartbeat to maintain the player's presence in the matchmaking lobby.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @return Response confirming the heartbeat was received.
     * @throws IOException if the request fails.
     */
    public static MatchmakingHeartbeatResponse sendMatchmakingHeartbeat(Client client, String playerToken) throws IOException {
        return client.post(client.url(Endpoints.MATCHMAKING_HEARTBEAT, "&player_token=" + playerToken), null, MatchmakingHeartbeatResponse.class);
    }

    /**
     * Removes the current matchmaking lobby (host only).
     *
     * @param client The API client instance.
     * @param playerToken The host's authentication token.
     * @return Response confirming the lobby was removed.
     * @throws IOException if the request fails.
     */
    public static MatchmakingRemoveResponse removeMatchmakingLobby(Client client, String playerToken) throws IOException {
        return client.post(client.url(Endpoints.MATCHMAKING_REMOVE, "&player_token=" + playerToken), null, MatchmakingRemoveResponse.class);
    }

    /**
     * Starts the game from the current matchmaking lobby and creates a game room.
     *
     * @param client The API client instance.
     * @param playerToken The host's authentication token.
     * @return Response containing the created room ID.
     * @throws IOException if the request fails.
     */
    public static MatchmakingStartResponse startGameFromMatchmaking(Client client, String playerToken) throws IOException {
        return client.post(client.url(Endpoints.MATCHMAKING_START, "&player_token=" + playerToken), null, MatchmakingStartResponse.class);
    }

    /**
     * Stops the current matchmaking lobby (host only).
     * Cannot be called after the game has started.
     *
     * @param client The API client instance.
     * @param playerToken The host's authentication token.
     * @return Success response confirming the lobby was stopped.
     * @throws IOException if the request fails.
     */
    public static SuccessResponse stopMatchmaking(Client client, String playerToken) throws IOException {
        return client.post(client.url(Endpoints.MATCHMAKING_STOP, "&player_token=" + playerToken), null, SuccessResponse.class);
    }

    /**
     * Kicks a player from the matchmaking lobby (host only).
     * Cannot kick players after the game has started.
     *
     * @param client The API client instance.
     * @param playerToken The host's authentication token.
     * @param playerId The ID of the player to kick.
     * @return Response confirming the player was kicked.
     * @throws IOException if the request fails.
     */
    public static MatchmakingKickResponse kickPlayer(Client client, String playerToken, int playerId) throws IOException {
        MatchmakingKickRequest request = new MatchmakingKickRequest(playerId);
        return client.post(client.url(Endpoints.MATCHMAKING_KICK, "&player_token=" + playerToken), request, MatchmakingKickResponse.class);
    }

    /**
     * Updates the password for the matchmaking lobby (host only).
     * Cannot change password after the game has started.
     *
     * @param client The API client instance.
     * @param playerToken The host's authentication token.
     * @param password New password, or null to remove the password.
     * @return Success response confirming the password was updated.
     * @throws IOException if the request fails.
     */
    public static SuccessResponse updateMatchmakingPassword(Client client, String playerToken, String password) throws IOException {
        MatchmakingPasswordUpdateRequest request = new MatchmakingPasswordUpdateRequest(password);
        return client.post(client.url(Endpoints.MATCHMAKING_PASSWORD, "&player_token=" + playerToken), request, SuccessResponse.class);
    }
}
