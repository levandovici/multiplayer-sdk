package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Comprehensive information about a game room.
 *
 * @param <T> The type to deserialize room rules into.
 */
public class CurrentRoomInfo<T> {
    @JsonProperty("room_id")
    private String roomId;

    @JsonProperty("room_name")
    private String roomName;

    @JsonProperty("is_host")
    private boolean isHost;

    @JsonProperty("is_online")
    private boolean isOnline;

    @JsonProperty("max_players")
    private int maxPlayers;

    @JsonProperty("current_players")
    private int currentPlayers;

    @JsonProperty("has_password")
    private boolean hasPassword;

    @JsonProperty("host_switch")
    private boolean hostSwitch;

    @JsonProperty("can_leave")
    private boolean canLeave;

    @JsonProperty("realtime")
    private boolean realtime;

    @JsonProperty("is_active")
    private boolean isActive;

    @JsonProperty("rules")
    private T rules;

    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("joined_at")
    private String joinedAt;

    @JsonProperty("last_heartbeat")
    private String lastHeartbeat;

    @JsonProperty("room_created_at")
    private String roomCreatedAt;

    @JsonProperty("room_last_activity")
    private String roomLastActivity;

    public String getRoomId() {
        return roomId;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
    }

    public String getRoomName() {
        return roomName;
    }

    public void setRoomName(String roomName) {
        this.roomName = roomName;
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

    public int getMaxPlayers() {
        return maxPlayers;
    }

    public void setMaxPlayers(int maxPlayers) {
        this.maxPlayers = maxPlayers;
    }

    public int getCurrentPlayers() {
        return currentPlayers;
    }

    public void setCurrentPlayers(int currentPlayers) {
        this.currentPlayers = currentPlayers;
    }

    public boolean isHasPassword() {
        return hasPassword;
    }

    public void setHasPassword(boolean hasPassword) {
        this.hasPassword = hasPassword;
    }

    public boolean isHostSwitch() {
        return hostSwitch;
    }

    public void setHostSwitch(boolean hostSwitch) {
        this.hostSwitch = hostSwitch;
    }

    public boolean isCanLeave() {
        return canLeave;
    }

    public void setCanLeave(boolean canLeave) {
        this.canLeave = canLeave;
    }

    public boolean isRealtime() {
        return realtime;
    }

    public void setRealtime(boolean realtime) {
        this.realtime = realtime;
    }

    public boolean isActive() {
        return isActive;
    }

    public void setActive(boolean active) {
        isActive = active;
    }

    public T getRules() {
        return rules;
    }

    public void setRules(T rules) {
        this.rules = rules;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public String getJoinedAt() {
        return joinedAt;
    }

    public void setJoinedAt(String joinedAt) {
        this.joinedAt = joinedAt;
    }

    public String getLastHeartbeat() {
        return lastHeartbeat;
    }

    public void setLastHeartbeat(String lastHeartbeat) {
        this.lastHeartbeat = lastHeartbeat;
    }

    public String getRoomCreatedAt() {
        return roomCreatedAt;
    }

    public void setRoomCreatedAt(String roomCreatedAt) {
        this.roomCreatedAt = roomCreatedAt;
    }

    public String getRoomLastActivity() {
        return roomLastActivity;
    }

    public void setRoomLastActivity(String roomLastActivity) {
        this.roomLastActivity = roomLastActivity;
    }
}
