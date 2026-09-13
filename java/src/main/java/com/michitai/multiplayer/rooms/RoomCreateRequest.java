package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for creating a new game room.
 */
public class RoomCreateRequest {
    @JsonProperty("room_name")
    private String roomName;

    @JsonProperty("password")
    private String password;

    @JsonProperty("max_players")
    private int maxPlayers;

    @JsonProperty("host_switch")
    private boolean hostSwitch;

    @JsonProperty("realtime")
    private boolean realtime;

    @JsonProperty("player_data")
    private String playerData;

    @JsonProperty("rules")
    private String rules;

    public RoomCreateRequest(String roomName, String password, int maxPlayers, boolean hostSwitch, boolean realtime, String playerData, String rules) {
        this.roomName = roomName;
        this.password = password;
        this.maxPlayers = maxPlayers;
        this.hostSwitch = hostSwitch;
        this.realtime = realtime;
        this.playerData = playerData;
        this.rules = rules;
    }

    public String getRoomName() {
        return roomName;
    }

    public void setRoomName(String roomName) {
        this.roomName = roomName;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public int getMaxPlayers() {
        return maxPlayers;
    }

    public void setMaxPlayers(int maxPlayers) {
        this.maxPlayers = maxPlayers;
    }

    public boolean isHostSwitch() {
        return hostSwitch;
    }

    public void setHostSwitch(boolean hostSwitch) {
        this.hostSwitch = hostSwitch;
    }

    public boolean isRealtime() {
        return realtime;
    }

    public void setRealtime(boolean realtime) {
        this.realtime = realtime;
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
