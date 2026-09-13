package com.michitai.multiplayer.games;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Short player information for player listings.
 */
public class PlayerShort {
    @JsonProperty("id")
    private int id;

    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("is_online")
    private boolean isOnline;

    @JsonProperty("last_login")
    private String lastLogin;

    @JsonProperty("created_at")
    private String createdAt;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public boolean isOnline() {
        return isOnline;
    }

    public void setOnline(boolean online) {
        isOnline = online;
    }

    public String getLastLogin() {
        return lastLogin;
    }

    public void setLastLogin(String lastLogin) {
        this.lastLogin = lastLogin;
    }

    public String getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(String createdAt) {
        this.createdAt = createdAt;
    }
}
