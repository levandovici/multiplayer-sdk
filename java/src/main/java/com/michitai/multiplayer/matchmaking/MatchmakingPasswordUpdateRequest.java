package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for updating matchmaking lobby password.
 */
public class MatchmakingPasswordUpdateRequest {
    @JsonProperty("password")
    private String password;

    public MatchmakingPasswordUpdateRequest(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }
}
