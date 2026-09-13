package com.michitai.multiplayer.rooms.updates;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.rooms.actions.ERoomTargetPlayers;

/**
 * Request for sending updates to target players.
 */
public class UpdatePlayersRequest {
    @JsonProperty("target_players")
    private String targetPlayers;

    @JsonProperty("type")
    private String type;

    @JsonProperty("data")
    private String data;

    @JsonProperty("target_players_ids")
    private int[] targetPlayersIds;

    public UpdatePlayersRequest(ERoomTargetPlayers targetPlayers, String type, String data, int[] targetPlayersIds) {
        this.targetPlayers = targetPlayers.name().toLowerCase();
        this.type = type;
        this.data = data;
        this.targetPlayersIds = targetPlayersIds;
    }

    public String getTargetPlayers() {
        return targetPlayers;
    }

    public void setTargetPlayers(String targetPlayers) {
        this.targetPlayers = targetPlayers;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getData() {
        return data;
    }

    public void setData(String data) {
        this.data = data;
    }

    public int[] getTargetPlayersIds() {
        return targetPlayersIds;
    }

    public void setTargetPlayersIds(int[] targetPlayersIds) {
        this.targetPlayersIds = targetPlayersIds;
    }
}
