package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing the list of players in a game room.
 *
 * @param <T> The type to deserialize player data into.
 */
public class RoomPlayersResponse<T> extends ApiResponse {
    @JsonProperty("players")
    private List<RoomPlayer<T>> players;

    @JsonProperty("last_updated")
    private String lastUpdated;

    public List<RoomPlayer<T>> getPlayers() {
        return players;
    }

    public void setPlayers(List<RoomPlayer<T>> players) {
        this.players = players;
    }

    public String getLastUpdated() {
        return lastUpdated;
    }

    public void setLastUpdated(String lastUpdated) {
        this.lastUpdated = lastUpdated;
    }
}
