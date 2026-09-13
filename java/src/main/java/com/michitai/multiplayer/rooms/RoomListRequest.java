package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for listing available game rooms.
 */
public class RoomListRequest {
    @JsonProperty("search")
    private String search;

    @JsonProperty("limit")
    private Integer limit;

    public RoomListRequest(String search, Integer limit) {
        this.search = search;
        this.limit = limit;
    }

    public String getSearch() {
        return search;
    }

    public void setSearch(String search) {
        this.search = search;
    }

    public Integer getLimit() {
        return limit;
    }

    public void setLimit(Integer limit) {
        this.limit = limit;
    }
}
