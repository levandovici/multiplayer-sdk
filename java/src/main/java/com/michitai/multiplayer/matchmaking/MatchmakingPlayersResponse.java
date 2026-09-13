package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing the list of players in a matchmaking lobby.
 *
 * @param <T> The type to deserialize player data into.
 */
public class MatchmakingPlayersResponse<T> extends ApiResponse {
    @JsonProperty("players")
    private List<MatchmakingPlayer<T>> players;

    @JsonProperty("last_updated")
    private String lastUpdated;

    public List<MatchmakingPlayer<T>> getPlayers() {
        return players;
    }

    public void setPlayers(List<MatchmakingPlayer<T>> players) {
        this.players = players;
    }

    public String getLastUpdated() {
        return lastUpdated;
    }

    public void setLastUpdated(String lastUpdated) {
        this.lastUpdated = lastUpdated;
    }
}
