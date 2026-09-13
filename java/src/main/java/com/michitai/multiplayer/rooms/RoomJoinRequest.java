package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for joining an existing game room.
 */
public class RoomJoinRequest {
    @JsonProperty("password")
    private String password;

    @JsonProperty("player_data")
    private String playerData;

    public RoomJoinRequest(String password, String playerData) {
        this.password = password;
        this.playerData = playerData;
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
}
