package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response confirming a player was kicked from the matchmaking lobby.
 */
public class MatchmakingKickResponse extends ApiResponse {
    @JsonProperty("message")
    private String message;

    @JsonProperty("player_id")
    private int playerId;

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }
}
