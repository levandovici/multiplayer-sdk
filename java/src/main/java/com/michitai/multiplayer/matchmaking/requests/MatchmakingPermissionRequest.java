package com.michitai.multiplayer.matchmaking.requests;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for responding to a matchmaking join request with approve/reject action.
 */
public class MatchmakingPermissionRequest {
    @JsonProperty("action")
    private String action;

    public MatchmakingPermissionRequest(String action) {
        this.action = action;
    }

    public String getAction() {
        return action;
    }

    public void setAction(String action) {
        this.action = action;
    }
}
