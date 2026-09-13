package com.michitai.multiplayer.rooms.actions;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response returned when actions are successfully submitted to target players.
 * Contains the action IDs and target player information.
 */
public class ActionSubmitResponse extends ApiResponse {
    @JsonProperty("actions_sent")
    private int actionsSent;

    @JsonProperty("action_ids")
    private List<String> actionIds;

    @JsonProperty("target_players_ids")
    private List<Integer> targetPlayersIds;

    public int getActionsSent() {
        return actionsSent;
    }

    public void setActionsSent(int actionsSent) {
        this.actionsSent = actionsSent;
    }

    public List<String> getActionIds() {
        return actionIds;
    }

    public void setActionIds(List<String> actionIds) {
        this.actionIds = actionIds;
    }

    public List<Integer> getTargetPlayersIds() {
        return targetPlayersIds;
    }

    public void setTargetPlayersIds(List<Integer> targetPlayersIds) {
        this.targetPlayersIds = targetPlayersIds;
    }
}
