package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for kicking a player from a matchmaking lobby.
 */
public class MatchmakingKickRequest {
    @JsonProperty("player_id")
    private int playerId;

    public MatchmakingKickRequest(int playerId) {
        this.playerId = playerId;
    }

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }
}
