package com.michitai.multiplayer.rooms.updates;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Information about a player update received in a room.
 * Contains update details, sender information, and data.
 *
 * @param <T> The type to deserialize update data into.
 */
public class PlayerUpdate<T> {
    @JsonProperty("update_id")
    private String updateId;

    @JsonProperty("from_player_id")
    private int fromPlayerId;

    @JsonProperty("type")
    private String type;

    @JsonProperty("data_json")
    private String dataJson;

    @JsonProperty("data")
    private T data;

    @JsonProperty("created_at")
    private String createdAt;

    public String getUpdateId() {
        return updateId;
    }

    public void setUpdateId(String updateId) {
        this.updateId = updateId;
    }

    public int getFromPlayerId() {
        return fromPlayerId;
    }

    public void setFromPlayerId(int fromPlayerId) {
        this.fromPlayerId = fromPlayerId;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getDataJson() {
        return dataJson;
    }

    public void setDataJson(String dataJson) {
        this.dataJson = dataJson;
    }

    public T getData() {
        return data;
    }

    public void setData(T data) {
        this.data = data;
    }

    public String getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(String createdAt) {
        this.createdAt = createdAt;
    }
}
