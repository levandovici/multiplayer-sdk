package com.michitai.multiplayer.matchmaking.requests;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.michitai.multiplayer.Client;
import com.michitai.multiplayer.matchmaking.MatchmakingCreateRequest;
import com.michitai.multiplayer.matchmaking.MatchmakingCreateResponse;
import com.michitai.multiplayer.matchmaking.MatchmakingJoinRequest;

import java.io.IOException;

/**
 * Provides methods for managing matchmaking join requests.
 * Handles creating lobbies, requesting to join, responding to requests, and checking request status.
 */
public class Requests {
    private static final ObjectMapper objectMapper = Client.JSON_MAPPER;

    /**
     * Creates a new matchmaking lobby with the specified configuration.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param matchmakingName The name for the matchmaking lobby.
     * @param maxPlayers Maximum number of players allowed.
     * @param strictFull Whether the lobby must be full to start.
     * @param joinByRequests Whether players must request to join.
     * @param hostSwitch Whether host switching is allowed.
     * @param canLeaveRoom Whether players can leave the resulting room.
     * @param realtimeRoom Whether the room supports realtime communication.
     * @param password Optional password for the lobby.
     * @param playerData Optional player data to include.
     * @param rules Optional lobby rules to include.
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
        
        return client.send("POST",client.url(Endpoints.MATCHMAKING_CREATE), request, MatchmakingCreateResponse.class, playerToken, false);
    }

    /**
     * Requests to join an existing matchmaking lobby.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param matchmakingId The ID of the matchmaking lobby to join.
     * @param playerData Optional player data to include with the request.
     * @return Response containing the request ID.
     * @throws IOException if the request fails.
     */
    public static MatchmakingJoinRequestResponse requestToJoinMatchmaking(Client client, String playerToken, String matchmakingId, Object playerData) throws IOException {
        return requestToJoinMatchmaking(client, playerToken, matchmakingId, null, playerData);
    }

    /**
     * Requests to join an existing matchmaking lobby.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param matchmakingId The ID of the matchmaking lobby to join.
     * @param password Optional password if the lobby is password-protected.
     * @param playerData Optional player data to include with the request.
     * @return Response containing the request ID.
     * @throws IOException if the request fails.
     */
    public static MatchmakingJoinRequestResponse requestToJoinMatchmaking(Client client, String playerToken, String matchmakingId, String password, Object playerData) throws IOException {
        String playerDataJson = playerData != null ? objectMapper.writeValueAsString(playerData) : null;
        String endpoint = String.format(Endpoints.MATCHMAKING_REQUEST, matchmakingId);
        MatchmakingJoinRequest request = (password != null || playerDataJson != null) ?
            new MatchmakingJoinRequest(password, playerDataJson) : null;
        return client.send("POST",client.url(endpoint), request, MatchmakingJoinRequestResponse.class, playerToken, false);
    }

    /**
     * Responds to a pending join request (approve or reject).
     * Only the host can respond to join requests.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param requestId The ID of the join request to respond to.
     * @param action The action to take (APPROVE or REJECT).
     * @return Response confirming the response action.
     * @throws IOException if the request fails.
     */
    public static MatchmakingPermissionResponse respondToJoinRequest(Client client, String playerToken, String requestId, EMatchmakingRequestAction action) throws IOException {
        MatchmakingPermissionRequest request = new MatchmakingPermissionRequest(action.name().toLowerCase());
        String endpoint = String.format(Endpoints.MATCHMAKING_RESPONSE, requestId);
        return client.send("POST",client.url(endpoint), request, MatchmakingPermissionResponse.class, playerToken, false);
    }

    /**
     * Checks the status of a specific join request.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param requestId The ID of the join request to check.
     * @return Response containing the request status.
     * @throws IOException if the request fails.
     */
    public static MatchmakingRequestStatusResponse checkJoinRequestStatus(Client client, String playerToken, String requestId) throws IOException {
        String endpoint = String.format(Endpoints.MATCHMAKING_REQUEST_STATUS, requestId);
        return client.send("GET",client.url(endpoint), null, MatchmakingRequestStatusResponse.class, playerToken, false);
    }
}
