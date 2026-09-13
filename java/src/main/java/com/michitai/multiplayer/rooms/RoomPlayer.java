package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Player information within a game room.
 *
 * @param <T> The type to deserialize player data into.
 */
public class RoomPlayer<T> {
    @JsonProperty("player_id")
    private int playerId;

    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("is_host")
    private boolean isHost;

    @JsonProperty("is_online")
    private boolean isOnline;

    @JsonProperty("is_local")
    private boolean isLocal;

    @JsonProperty("player_data_json")
    private String playerDataJson;

    @JsonProperty("player_data")
    private T playerData;

    @JsonProperty("last_heartbeat")
    private String lastHeartbeat;

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public boolean isHost() {
        return isHost;
    }

    public void setHost(boolean host) {
        isHost = host;
    }

    public boolean isOnline() {
        return isOnline;
    }

    public void setOnline(boolean online) {
        isOnline = online;
    }

    public boolean isLocal() {
        return isLocal;
    }

    public void setLocal(boolean local) {
        isLocal = local;
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

    public String getLastHeartbeat() {
        return lastHeartbeat;
    }

    public void setLastHeartbeat(String lastHeartbeat) {
        this.lastHeartbeat = lastHeartbeat;
    }
}
