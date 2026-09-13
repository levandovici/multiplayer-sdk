package com.michitai.multiplayer.rooms.actions;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Information about a pending action awaiting completion.
 *
 * @param <T> The type to deserialize action request data into.
 */
public class PendingAction<T> {
    @JsonProperty("action_id")
    private String actionId;

    @JsonProperty("player_id")
    private int playerId;

    @JsonProperty("target_id")
    private int targetId;

    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("is_host")
    private boolean isHost;

    @JsonProperty("action_type")
    private String actionType;

    @JsonProperty("request_data_json")
    private String requestDataJson;

    @JsonProperty("request_data")
    private T requestData;

    @JsonProperty("created_at")
    private String createdAt;

    public String getActionId() {
        return actionId;
    }

    public void setActionId(String actionId) {
        this.actionId = actionId;
    }

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }

    public int getTargetId() {
        return targetId;
    }

    public void setTargetId(int targetId) {
        this.targetId = targetId;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public boolean isHost() {
        return isHost;
    }

    public void setHost(boolean host) {
        isHost = host;
    }

    public String getActionType() {
        return actionType;
    }

    public void setActionType(String actionType) {
        this.actionType = actionType;
    }

    public String getRequestDataJson() {
        return requestDataJson;
    }

    public void setRequestDataJson(String requestDataJson) {
        this.requestDataJson = requestDataJson;
    }

    public T getRequestData() {
        return requestData;
    }

    public void setRequestData(T requestData) {
        this.requestData = requestData;
    }

    public String getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(String createdAt) {
        this.createdAt = createdAt;
    }
}
