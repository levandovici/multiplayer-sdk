package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for joining a matchmaking lobby (direct join or join request).
 * Serializes to { password, player_data } — the shape the backend expects.
 */
public class MatchmakingJoinRequest {
    @JsonProperty("password")
    private String password;

    @JsonProperty("player_data")
    private String playerData;

    @JsonProperty("join_token")
    private String joinToken;

    public MatchmakingJoinRequest(String password, String playerData) {
        this(password, playerData, null);
    }

    public MatchmakingJoinRequest(String password, String playerData, String joinToken) {
        this.password = password;
        this.playerData = playerData;
        this.joinToken = joinToken;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPlayerData() {
        return playerData;
    }

    public void setPlayerData(String playerData) {
        this.playerData = playerData;
    }

    /**
     * Invite join token — bypasses the password and join_by_requests approval.
     */
    public String getJoinToken() {
        return joinToken;
    }

    public void setJoinToken(String joinToken) {
        this.joinToken = joinToken;
    }
}
