package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Comprehensive information about a matchmaking lobby.
 *
 * @param <T> The type to deserialize lobby rules into.
 */
public class MatchmakingInfo<T> {
    @JsonProperty("matchmaking_id")
    private String matchmakingId;

    @JsonProperty("matchmaking_name")
    private String matchmakingName;

    @JsonProperty("is_host")
    private boolean isHost;

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

    @JsonProperty("rules_json")
    private String rulesJson;

    @JsonProperty("rules")
    private T rules;

    @JsonProperty("joined_at")
    private String joinedAt;

    @JsonProperty("is_online")
    private boolean isOnline;

    @JsonProperty("last_heartbeat")
    private String lastHeartbeat;

    @JsonProperty("lobby_heartbeat")
    private String lobbyHeartbeat;

    @JsonProperty("is_started")
    private boolean isStarted;

    @JsonProperty("started_at")
    private String startedAt;

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

    public boolean isHost() {
        return isHost;
    }

    public void setHost(boolean host) {
        isHost = host;
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

    public String getJoinedAt() {
        return joinedAt;
    }

    public void setJoinedAt(String joinedAt) {
        this.joinedAt = joinedAt;
    }

    public boolean isOnline() {
        return isOnline;
    }

    public void setOnline(boolean online) {
        isOnline = online;
    }

    public String getLastHeartbeat() {
        return lastHeartbeat;
    }

    public void setLastHeartbeat(String lastHeartbeat) {
        this.lastHeartbeat = lastHeartbeat;
    }

    public String getLobbyHeartbeat() {
        return lobbyHeartbeat;
    }

    public void setLobbyHeartbeat(String lobbyHeartbeat) {
        this.lobbyHeartbeat = lobbyHeartbeat;
    }

    public boolean isStarted() {
        return isStarted;
    }

    public void setStarted(boolean started) {
        isStarted = started;
    }

    public String getStartedAt() {
        return startedAt;
    }

    public void setStartedAt(String startedAt) {
        this.startedAt = startedAt;
    }
}
