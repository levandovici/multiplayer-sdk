package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Short room information for room listings.
 *
 * @param <T> The type to deserialize room rules into.
 */
public class RoomShort<T> {
    @JsonProperty("room_id")
    private String roomId;

    @JsonProperty("room_name")
    private String roomName;

    @JsonProperty("current_players")
    private int currentPlayers;

    @JsonProperty("max_players")
    private int maxPlayers;

    @JsonProperty("has_password")
    private boolean hasPassword;

    @JsonProperty("host_switch")
    private boolean hostSwitch;

    @JsonProperty("can_leave")
    private boolean canLeave;

    @JsonProperty("realtime")
    private boolean realtime;

    @JsonProperty("rules_json")
    private String rulesJson;

    @JsonProperty("rules")
    private T rules;

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

    public int getCurrentPlayers() {
        return currentPlayers;
    }

    public void setCurrentPlayers(int currentPlayers) {
        this.currentPlayers = currentPlayers;
    }

    public int getMaxPlayers() {
        return maxPlayers;
    }

    public void setMaxPlayers(int maxPlayers) {
        this.maxPlayers = maxPlayers;
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
}
