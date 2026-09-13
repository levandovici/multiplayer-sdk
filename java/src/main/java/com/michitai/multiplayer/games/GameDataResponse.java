package com.michitai.multiplayer.games;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response containing game data with typed deserialization support.
 * Uses Jackson for deserialization.
 *
 * @param <T> The type to deserialize the game data into.
 */
public class GameDataResponse<T> extends ApiResponse {
    @JsonProperty("type")
    private String type;

    @JsonProperty("game_id")
    private int gameId;

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

    public int getGameId() {
        return gameId;
    }

    public void setGameId(int gameId) {
        this.gameId = gameId;
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
