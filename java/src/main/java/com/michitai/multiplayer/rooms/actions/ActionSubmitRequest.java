package com.michitai.multiplayer.rooms.actions;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for submitting an action to target players.
 */
public class ActionSubmitRequest {
    @JsonProperty("target_players")
    private String targetPlayers;

    @JsonProperty("action_type")
    private String actionType;

    @JsonProperty("request_data")
    private String requestData;

    @JsonProperty("target_players_ids")
    private int[] targetPlayersIds;

    public ActionSubmitRequest(ERoomTargetPlayers targetPlayers, String actionType, String requestData, int[] targetPlayersIds) {
        this.targetPlayers = targetPlayers.name().toLowerCase();
        this.actionType = actionType;
        this.requestData = requestData;
        this.targetPlayersIds = targetPlayersIds;
    }

    public String getTargetPlayers() {
        return targetPlayers;
    }

    public void setTargetPlayers(String targetPlayers) {
        this.targetPlayers = targetPlayers;
    }

    public String getActionType() {
        return actionType;
    }

    public void setActionType(String actionType) {
        this.actionType = actionType;
    }

    public String getRequestData() {
        return requestData;
    }

    public void setRequestData(String requestData) {
        this.requestData = requestData;
    }

    public int[] getTargetPlayersIds() {
        return targetPlayersIds;
    }

    public void setTargetPlayersIds(int[] targetPlayersIds) {
        this.targetPlayersIds = targetPlayersIds;
    }
}
