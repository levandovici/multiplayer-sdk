package com.michitai.multiplayer.rooms.actions;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Information about a completed action in the room.
 * Contains action details, status, and response data.
 *
 * @param <T> The type to deserialize response data into.
 */
public class ActionInfo<T> {
    @JsonProperty("action_id")
    private String actionId;

    @JsonProperty("action_type")
    private String actionType;

    @JsonProperty("is_host")
    private boolean isHost;

    @JsonProperty("target_id")
    private int targetId;

    @JsonProperty("status")
    private String status;

    @JsonProperty("response_data_json")
    private String responseDataJson;

    @JsonProperty("response_data")
    private T responseData;

    @JsonProperty("processed_at")
    private String processedAt;

    public String getActionId() {
        return actionId;
    }

    public void setActionId(String actionId) {
        this.actionId = actionId;
    }

    public String getActionType() {
        return actionType;
    }

    public void setActionType(String actionType) {
        this.actionType = actionType;
    }

    public boolean isHost() {
        return isHost;
    }

    public void setHost(boolean host) {
        isHost = host;
    }

    public int getTargetId() {
        return targetId;
    }

    public void setTargetId(int targetId) {
        this.targetId = targetId;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getResponseDataJson() {
        return responseDataJson;
    }

    public void setResponseDataJson(String responseDataJson) {
        this.responseDataJson = responseDataJson;
    }

    public T getResponseData() {
        return responseData;
    }

    public void setResponseData(T responseData) {
        this.responseData = responseData;
    }

    public String getProcessedAt() {
        return processedAt;
    }

    public void setProcessedAt(String processedAt) {
        this.processedAt = processedAt;
    }

    /**
     * The parsed status of the action.
     *
     * @return The action status enum value.
     * @throws IllegalArgumentException if the status is unknown.
     */
    @JsonIgnore
    public ERoomActionStatus getActionStatus() {
        switch (status == null ? "" : status) {
            case "pending":
                return ERoomActionStatus.PENDING;
            case "processing":
                return ERoomActionStatus.PROCESSING;
            case "completed":
                return ERoomActionStatus.COMPLETED;
            case "failed":
                return ERoomActionStatus.FAILED;
            case "read":
                return ERoomActionStatus.READ;
            default:
                throw new IllegalArgumentException("Unknown action status: " + status);
        }
    }
}
