package com.michitai.multiplayer.rooms.updates;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response returned when updates are successfully sent to target players.
 */
public class UpdatePlayersResponse extends ApiResponse {
    @JsonProperty("updates_sent")
    private int updatesSent;

    @JsonProperty("update_ids")
    private List<String> updateIds;

    @JsonProperty("target_players_ids")
    private List<Integer> targetPlayersIds;

    public int getUpdatesSent() {
        return updatesSent;
    }

    public void setUpdatesSent(int updatesSent) {
        this.updatesSent = updatesSent;
    }

    public List<String> getUpdateIds() {
        return updateIds;
    }

    public void setUpdateIds(List<String> updateIds) {
        this.updateIds = updateIds;
    }

    public List<Integer> getTargetPlayersIds() {
        return targetPlayersIds;
    }

    public void setTargetPlayersIds(List<Integer> targetPlayersIds) {
        this.targetPlayersIds = targetPlayersIds;
    }
}
