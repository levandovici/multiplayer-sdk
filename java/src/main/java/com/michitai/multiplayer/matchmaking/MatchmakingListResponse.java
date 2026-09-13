package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing a list of available matchmaking lobbies.
 *
 * @param <T> The type to deserialize lobby rules into.
 */
public class MatchmakingListResponse<T> extends ApiResponse {
    @JsonProperty("lobbies")
    private List<MatchmakingLobby<T>> lobbies;

    @JsonProperty("count")
    private int count;

    public List<MatchmakingLobby<T>> getLobbies() {
        return lobbies;
    }

    public void setLobbies(List<MatchmakingLobby<T>> lobbies) {
        this.lobbies = lobbies;
    }

    public int getCount() {
        return count;
    }

    public void setCount(int count) {
        this.count = count;
    }
}
