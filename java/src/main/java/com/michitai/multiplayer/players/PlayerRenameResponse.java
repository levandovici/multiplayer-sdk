package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response confirming a player name change.
 */
public class PlayerRenameResponse extends ApiResponse {
    @JsonProperty("message")
    private String message;

    @JsonProperty("new_name")
    private String newName;

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public String getNewName() {
        return newName;
    }

    public void setNewName(String newName) {
        this.newName = newName;
    }
}
