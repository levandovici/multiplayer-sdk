package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Short matchmaking lobby information for lobby listings.
 *
 * @param <T> The type to deserialize lobby rules into.
 */
public class MatchmakingLobby<T> {
    @JsonProperty("matchmaking_id")
    private String matchmakingId;

    @JsonProperty("matchmaking_name")
    private String matchmakingName;

    @JsonProperty("host_player_id")
    private int hostPlayerId;

    @JsonProperty("max_players")
    private int maxPlayers;

    @JsonProperty("current_players")
    private int currentPlayers;

    @JsonProperty("strict_full")
    private boolean strictFull;

    @JsonProperty("join_by_requests")
    private boolean joinByRequests;

    @JsonProperty("host_switch")
    private boolean hostSwitch;

    @JsonProperty("can_leave_room")
    private boolean canLeaveRoom;

    @JsonProperty("realtime_room")
    private boolean realtimeRoom;

    @JsonProperty("has_password")
    private boolean hasPassword;

    @JsonProperty("host_name")
    private String hostName;

    @JsonProperty("rules_json")
    private String rulesJson;

    @JsonProperty("rules")
    private T rules;

    @JsonProperty("created_at")
    private String createdAt;

    @JsonProperty("last_heartbeat")
    private String lastHeartbeat;

    public String getMatchmakingId() {
        return matchmakingId;
    }

    public void setMatchmakingId(String matchmakingId) {
        this.matchmakingId = matchmakingId;
    }

    public String getMatchmakingName() {
        return matchmakingName;
    }

    public void setMatchmakingName(String matchmakingName) {
        this.matchmakingName = matchmakingName;
    }

    public int getHostPlayerId() {
        return hostPlayerId;
    }

    public void setHostPlayerId(int hostPlayerId) {
        this.hostPlayerId = hostPlayerId;
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

    public boolean isStrictFull() {
        return strictFull;
    }

    public void setStrictFull(boolean strictFull) {
        this.strictFull = strictFull;
    }

    public boolean isJoinByRequests() {
        return joinByRequests;
    }

    public void setJoinByRequests(boolean joinByRequests) {
        this.joinByRequests = joinByRequests;
    }

    public boolean isHostSwitch() {
        return hostSwitch;
    }

    public void setHostSwitch(boolean hostSwitch) {
        this.hostSwitch = hostSwitch;
    }

    public boolean isCanLeaveRoom() {
        return canLeaveRoom;
    }

    public void setCanLeaveRoom(boolean canLeaveRoom) {
        this.canLeaveRoom = canLeaveRoom;
    }

    public boolean isRealtimeRoom() {
        return realtimeRoom;
    }

    public void setRealtimeRoom(boolean realtimeRoom) {
        this.realtimeRoom = realtimeRoom;
    }

    public boolean isHasPassword() {
        return hasPassword;
    }

    public void setHasPassword(boolean hasPassword) {
        this.hasPassword = hasPassword;
    }

    public String getHostName() {
        return hostName;
    }

    public void setHostName(String hostName) {
        this.hostName = hostName;
    }

    public String getRulesJson() {
        return rulesJson;
    }

    public void setRulesJson(String rulesJson) {
        this.rulesJson = rulesJson;
    }

    public T getRules() {
        return rules;
    }

    public void setRules(T rules) {
        this.rules = rules;
    }

    public String getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(String createdAt) {
        this.createdAt = createdAt;
    }

    public String getLastHeartbeat() {
        return lastHeartbeat;
    }

    public void setLastHeartbeat(String lastHeartbeat) {
        this.lastHeartbeat = lastHeartbeat;
    }
}
