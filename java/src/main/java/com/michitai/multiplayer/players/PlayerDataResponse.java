package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response containing a player's data with typed deserialization support.
 * Uses Jackson for deserialization.
 *
 * @param <T> The type to deserialize player data into.
 */
public class PlayerDataResponse<T> extends ApiResponse {
    @JsonProperty("type")
    private String type;

    @JsonProperty("player_id")
    private int playerId;

    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("data_json")
    private String dataJson;

    @JsonProperty("data")
    private T data;

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
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
}
