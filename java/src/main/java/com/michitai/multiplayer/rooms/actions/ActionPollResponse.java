package com.michitai.multiplayer.rooms.actions;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing completed actions that were targeted to the polling player.
 *
 * @param <T> The type to deserialize action response data into.
 */
public class ActionPollResponse<T> extends ApiResponse {
    @JsonProperty("actions")
    private List<ActionInfo<T>> actions;

    public List<ActionInfo<T>> getActions() {
        return actions;
    }

    public void setActions(List<ActionInfo<T>> actions) {
        this.actions = actions;
    }
}
