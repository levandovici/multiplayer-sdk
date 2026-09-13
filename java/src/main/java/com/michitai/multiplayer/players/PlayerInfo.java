package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Player information including ID, name, and custom data.
 *
 * @param <T> The type to deserialize player data into.
 */
public class PlayerInfo<T> {
    @JsonProperty("id")
    private int id;

    @JsonProperty("game_id")
    private int gameId;

    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("player_data_json")
    private String playerDataJson;

    @JsonProperty("player_data")
    private T playerData;

    @JsonProperty("is_online")
    private boolean isOnline;

    @JsonProperty("last_login")
    private String lastLogin;

    @JsonProperty("last_logout")
    private String lastLogout;

    @JsonProperty("last_heartbeat")
    private String lastHeartbeat;

    @JsonProperty("created_at")
    private String createdAt;

    @JsonProperty("updated_at")
    private String updatedAt;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getGameId() {
        return gameId;
    }

    public void setGameId(int gameId) {
        this.gameId = gameId;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public String getPlayerDataJson() {
        return playerDataJson;
    }

    public void setPlayerDataJson(String playerDataJson) {
        this.playerDataJson = playerDataJson;
    }

    public T getPlayerData() {
        return playerData;
    }

    public void setPlayerData(T playerData) {
        this.playerData = playerData;
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

    public String getLastLogout() {
        return lastLogout;
    }

    public void setLastLogout(String lastLogout) {
        this.lastLogout = lastLogout;
    }

    public String getLastHeartbeat() {
        return lastHeartbeat;
    }

    public void setLastHeartbeat(String lastHeartbeat) {
        this.lastHeartbeat = lastHeartbeat;
    }

    public String getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(String createdAt) {
        this.createdAt = createdAt;
    }

    public String getUpdatedAt() {
        return updatedAt;
    }

    public void setUpdatedAt(String updatedAt) {
        this.updatedAt = updatedAt;
    }
}
