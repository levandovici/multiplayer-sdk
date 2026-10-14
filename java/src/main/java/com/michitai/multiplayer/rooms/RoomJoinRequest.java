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

    @JsonProperty("join_token")
    private String joinToken;

    public RoomJoinRequest(String password, String playerData) {
        this(password, playerData, null);
    }

    public RoomJoinRequest(String password, String playerData, String joinToken) {
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
     * Invite join token — bypasses the password.
     */
    public String getJoinToken() {
        return joinToken;
    }

    public void setJoinToken(String joinToken) {
        this.joinToken = joinToken;
    }
}
