package com.michitai.multiplayer.games;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing a list of all players in the game.
 */
public class PlayerListResponse extends ApiResponse {
    @JsonProperty("count")
    private int count;

    @JsonProperty("players")
    private List<PlayerShort> players;

    public int getCount() {
        return count;
    }

    public void setCount(int count) {
        this.count = count;
    }

    public List<PlayerShort> getPlayers() {
        return players;
    }

    public void setPlayers(List<PlayerShort> players) {
        this.players = players;
    }
}
