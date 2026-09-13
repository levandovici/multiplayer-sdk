package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for unbanning a player.
 */
public class PlayerUnbanRequest {
    @JsonProperty("player_id")
    private int playerId;

    public PlayerUnbanRequest(int playerId) {
        this.playerId = playerId;
    }

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }
}
