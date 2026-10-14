package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response for friend request/accept/decline/remove operations.
 */
public class FriendStatusResponse extends ApiResponse {
    /** "pending", "accepted", "declined" or "removed". */
    @JsonProperty("status")
    private String status;

    @JsonProperty("player_id")
    private int playerId;

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }
    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }
}
