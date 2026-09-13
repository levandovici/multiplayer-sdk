package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing comprehensive information about the current game room.
 * Includes room details, players, and pending actions/updates.
 *
 * @param <T> The type to deserialize room rules into.
 */
public class CurrentRoomResponse<T> extends ApiResponse {
    @JsonProperty("in_room")
    private boolean inRoom;

    @JsonProperty("room")
    private CurrentRoomInfo<T> room;

    @JsonProperty("pending_actions")
    private List<Object> pendingActions;

    @JsonProperty("pending_updates")
    private List<Object> pendingUpdates;

    public boolean isInRoom() {
        return inRoom;
    }

    public void setInRoom(boolean inRoom) {
        this.inRoom = inRoom;
    }

    public CurrentRoomInfo<T> getRoom() {
        return room;
    }

    public void setRoom(CurrentRoomInfo<T> room) {
        this.room = room;
    }

    public List<Object> getPendingActions() {
        return pendingActions;
    }

    public void setPendingActions(List<Object> pendingActions) {
        this.pendingActions = pendingActions;
    }

    public List<Object> getPendingUpdates() {
        return pendingUpdates;
    }

    public void setPendingUpdates(List<Object> pendingUpdates) {
        this.pendingUpdates = pendingUpdates;
    }
}
