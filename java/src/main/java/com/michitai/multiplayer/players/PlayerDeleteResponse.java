package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response returned when a player account deletion is scheduled.
 * The account is frozen immediately and purged permanently after the
 * grace window unless restored — see Players.restoreAccount.
 */
public class PlayerDeleteResponse extends ApiResponse {
    @JsonProperty("message")
    private String message;

    @JsonProperty("player_id")
    private int playerId;

    /**
     * Confirmation message from the server.
     */
    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    /**
     * The ID of the deleted player.
     */
    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }
}
