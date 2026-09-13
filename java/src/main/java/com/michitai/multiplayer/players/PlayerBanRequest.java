package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for banning a player.
 */
public class PlayerBanRequest {
    @JsonProperty("player_id")
    private int playerId;

    @JsonProperty("ban_duration")
    private String banDuration;

    @JsonProperty("ban_reason")
    private String banReason;

    public PlayerBanRequest(int playerId, String banDuration, String banReason) {
        this.playerId = playerId;
        this.banDuration = banDuration;
        this.banReason = banReason;
    }

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }

    public String getBanDuration() {
        return banDuration;
    }

    public void setBanDuration(String banDuration) {
        this.banDuration = banDuration;
    }

    public String getBanReason() {
        return banReason;
    }

    public void setBanReason(String banReason) {
        this.banReason = banReason;
    }
}
