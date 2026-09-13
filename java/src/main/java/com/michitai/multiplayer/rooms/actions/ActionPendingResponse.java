package com.michitai.multiplayer.rooms.actions;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing pending actions that need to be completed by the host.
 * Only the host can retrieve pending actions.
 *
 * @param <T> The type to deserialize action data into.
 */
public class ActionPendingResponse<T> extends ApiResponse {
    @JsonProperty("actions")
    private List<PendingAction<T>> actions;

    public List<PendingAction<T>> getActions() {
        return actions;
    }

    public void setActions(List<PendingAction<T>> actions) {
        this.actions = actions;
    }
}
