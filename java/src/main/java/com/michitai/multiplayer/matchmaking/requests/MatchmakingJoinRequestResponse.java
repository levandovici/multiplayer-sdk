package com.michitai.multiplayer.matchmaking.requests;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response returned when a player requests to join a matchmaking lobby.
 */
public class MatchmakingJoinRequestResponse extends ApiResponse {
    @JsonProperty("message")
    private String message;

    @JsonProperty("request_id")
    private String requestId;

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public String getRequestId() {
        return requestId;
    }

    public void setRequestId(String requestId) {
        this.requestId = requestId;
    }
}
