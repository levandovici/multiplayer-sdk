package com.michitai.multiplayer.matchmaking.requests;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response containing the status of a specific join request.
 */
public class MatchmakingRequestStatusResponse extends ApiResponse {
    @JsonProperty("request")
    private MatchmakingRequestInfo request;

    public MatchmakingRequestInfo getRequest() {
        return request;
    }

    public void setRequest(MatchmakingRequestInfo request) {
        this.request = request;
    }
}
