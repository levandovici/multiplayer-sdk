package com.michitai.multiplayer.rooms.updates;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing player updates that were targeted to the polling player.
 *
 * @param <T> The type to deserialize update data into.
 */
public class PollUpdatesResponse<T> extends ApiResponse {
    @JsonProperty("updates")
    private List<PlayerUpdate<T>> updates;

    @JsonProperty("last_update")
    private String lastUpdate;

    public List<PlayerUpdate<T>> getUpdates() {
        return updates;
    }

    public void setUpdates(List<PlayerUpdate<T>> updates) {
        this.updates = updates;
    }

    public String getLastUpdate() {
        return lastUpdate;
    }

    public void setLastUpdate(String lastUpdate) {
        this.lastUpdate = lastUpdate;
    }
}
