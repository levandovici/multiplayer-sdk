package com.michitai.multiplayer.leaderboard;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing the leaderboard entries with rankings.
 *
 * @param <T> The type to deserialize player data into.
 */
public class LeaderboardResponse<T> extends ApiResponse {
    @JsonProperty("leaderboard")
    private List<LeaderboardPlayer<T>> leaderboard;

    @JsonProperty("total")
    private int total;

    @JsonProperty("sort_by")
    private String[] sortBy;

    @JsonProperty("limit")
    private int limit;

    public List<LeaderboardPlayer<T>> getLeaderboard() {
        return leaderboard;
    }

    public void setLeaderboard(List<LeaderboardPlayer<T>> leaderboard) {
        this.leaderboard = leaderboard;
    }

    public int getTotal() {
        return total;
    }

    public void setTotal(int total) {
        this.total = total;
    }

    public String[] getSortBy() {
        return sortBy;
    }

    public void setSortBy(String[] sortBy) {
        this.sortBy = sortBy;
    }

    public int getLimit() {
        return limit;
    }

    public void setLimit(int limit) {
        this.limit = limit;
    }
}
