package com.michitai.multiplayer.rooms;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for updating room password.
 */
public class RoomPasswordUpdateRequest {
    @JsonProperty("password")
    private String password;

    public RoomPasswordUpdateRequest(String password) {
        this.password = password;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }
}
