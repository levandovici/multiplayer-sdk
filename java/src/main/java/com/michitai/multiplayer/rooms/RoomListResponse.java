package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing a list of available game rooms.
 *
 * @param <T> The type to deserialize room rules into.
 */
public class RoomListResponse<T> extends ApiResponse {
    @JsonProperty("rooms")
    private List<RoomShort<T>> rooms;

    @JsonProperty("count")
    private int count;

    public List<RoomShort<T>> getRooms() {
        return rooms;
    }

    public void setRooms(List<RoomShort<T>> rooms) {
        this.rooms = rooms;
    }

    public int getCount() {
        return count;
    }

    public void setCount(int count) {
        this.count = count;
    }
}
