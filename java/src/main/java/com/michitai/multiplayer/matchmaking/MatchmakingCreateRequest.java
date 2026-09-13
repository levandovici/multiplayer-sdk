package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for creating a new matchmaking lobby.
 */
public class MatchmakingCreateRequest {
    @JsonProperty("matchmaking_name")
    private String matchmakingName;

    @JsonProperty("max_players")
    private int maxPlayers;

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

    @JsonProperty("password")
    private String password;

    @JsonProperty("player_data")
    private String playerData;

    @JsonProperty("rules")
    private String rules;

    public MatchmakingCreateRequest(String matchmakingName, int maxPlayers, boolean strictFull, boolean joinByRequests,
            boolean hostSwitch, boolean canLeaveRoom, boolean realtimeRoom, String password, String playerData, String rules) {
        this.matchmakingName = matchmakingName;
        this.maxPlayers = maxPlayers;
        this.strictFull = strictFull;
        this.joinByRequests = joinByRequests;
        this.hostSwitch = hostSwitch;
        this.canLeaveRoom = canLeaveRoom;
        this.realtimeRoom = realtimeRoom;
        this.password = password;
        this.playerData = playerData;
        this.rules = rules;
    }

    public String getMatchmakingName() {
        return matchmakingName;
    }

    public void setMatchmakingName(String matchmakingName) {
        this.matchmakingName = matchmakingName;
    }

    public int getMaxPlayers() {
        return maxPlayers;
    }

    public void setMaxPlayers(int maxPlayers) {
        this.maxPlayers = maxPlayers;
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

    public String getRules() {
        return rules;
    }

    public void setRules(String rules) {
        this.rules = rules;
    }
}
