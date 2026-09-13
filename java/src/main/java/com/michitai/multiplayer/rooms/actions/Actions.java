package com.michitai.multiplayer.rooms.actions;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.michitai.multiplayer.Client;

import java.io.IOException;

/**
 * Provides methods for managing room actions.
 * Handles submitting actions, polling for completed actions, retrieving pending actions, and completing actions.
 */
public class Actions {
    private static final ObjectMapper objectMapper = Client.JSON_MAPPER;

    /**
     * Submits an action to target players in the room.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param request The action submission request containing targets and data.
     * @return Response containing the action IDs and target player information.
     * @throws IOException if the request fails.
     */
    public static ActionSubmitResponse submitAction(Client client, String playerToken, SubmitAction<?> request) throws IOException {
        String requestDataJson = request.getRequestData() != null ? objectMapper.writeValueAsString(request.getRequestData()) : null;
        ActionSubmitRequest submitRequest = new ActionSubmitRequest(
            request.getTargetPlayers(), 
            request.getActionType(), 
            requestDataJson, 
            request.getTargetPlayersIds()
        );
        return client.post(client.url(Endpoints.GAME_ROOM_ACTIONS, "&player_token=" + playerToken), submitRequest, ActionSubmitResponse.class);
    }

    /**
     * Polls for completed actions that were targeted to the current player.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @return Response containing the completed actions.
     * @throws IOException if the request fails.
     */
    public static ActionPollResponse<?> pollActions(Client client, String playerToken) throws IOException {
        return pollActions(client, playerToken, Object.class);
    }

    /**
     * Polls for completed actions that were targeted to the current player.
     *
     * @param <T> The type to deserialize action response data into.
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param dataType The class to deserialize action response data into.
     * @return Response containing the completed actions with typed response data.
     * @throws IOException if the request fails.
     */
    public static <T> ActionPollResponse<T> pollActions(Client client, String playerToken, Class<T> dataType) throws IOException {
        return client.get(client.url(Endpoints.GAME_ROOM_ACTIONS_POLL, "&player_token=" + playerToken),
            client.parametricType(ActionPollResponse.class, dataType));
    }

    /**
     * Retrieves pending actions that need to be completed by the host.
     * Only the host can retrieve pending actions.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @return Response containing the pending actions.
     * @throws IOException if the request fails.
     */
    public static ActionPendingResponse<?> getPendingActions(Client client, String playerToken) throws IOException {
        return getPendingActions(client, playerToken, Object.class);
    }

    /**
     * Retrieves pending actions that need to be completed by the host.
     * Only the host can retrieve pending actions.
     *
     * @param <T> The type to deserialize action request data into.
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param dataType The class to deserialize action request data into.
     * @return Response containing the pending actions with typed request data.
     * @throws IOException if the request fails.
     */
    public static <T> ActionPendingResponse<T> getPendingActions(Client client, String playerToken, Class<T> dataType) throws IOException {
        return client.get(client.url(Endpoints.GAME_ROOM_ACTIONS_PENDING, "&player_token=" + playerToken),
            client.parametricType(ActionPendingResponse.class, dataType));
    }

    /**
     * Marks an action as complete with an optional response.
     * Only the host can complete actions.
     *
     * @param client The API client instance.
     * @param actionId The ID of the action to complete.
     * @param playerToken The player's authentication token.
     * @param request The action completion request containing status and response data.
     * @return Response confirming the action was completed.
     * @throws IOException if the request fails.
     */
    public static ActionCompleteResponse completeAction(Client client, String actionId, String playerToken, ActionComplete<?> request) throws IOException {
        String responseDataJson = request.getResponseData() != null ? objectMapper.writeValueAsString(request.getResponseData()) : null;
        ActionCompleteRequest completeRequest = new ActionCompleteRequest(
            request.getStatus().name().toLowerCase(), 
            responseDataJson
        );
        String endpoint = String.format(Endpoints.GAME_ROOM_ACTION_COMPLETE, actionId);
        return client.post(client.url(endpoint, "&player_token=" + playerToken), completeRequest, ActionCompleteResponse.class);
    }
}
