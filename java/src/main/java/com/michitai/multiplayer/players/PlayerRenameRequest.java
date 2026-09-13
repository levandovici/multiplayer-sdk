package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for renaming a player.
 */
public class PlayerRenameRequest {
    @JsonProperty("new_name")
    private String newName;

    public PlayerRenameRequest(String newName) {
        this.newName = newName;
    }

    public String getNewName() {
        return newName;
    }

    public void setNewName(String newName) {
        this.newName = newName;
    }
}
