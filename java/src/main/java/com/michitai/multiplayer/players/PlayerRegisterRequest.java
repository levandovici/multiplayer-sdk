package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for registering a new player.
 */
public class PlayerRegisterRequest {
    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("player_data")
    private String playerData;

    public PlayerRegisterRequest(String playerName, String playerData) {
        this.playerName = playerName;
        this.playerData = playerData;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public String getPlayerData() {
        return playerData;
    }

    public void setPlayerData(String playerData) {
        this.playerData = playerData;
    }
}
