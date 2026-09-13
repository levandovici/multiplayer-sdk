package com.michitai.multiplayer.leaderboard;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for querying the leaderboard with sorting and limit options.
 */
public class LeaderboardRequest {
    @JsonProperty("sort_by")
    private String[] sortBy;

    @JsonProperty("limit")
    private int limit;

    public LeaderboardRequest(String[] sortBy, int limit) {
        this.sortBy = sortBy;
        this.limit = limit;
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
