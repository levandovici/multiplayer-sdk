package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response for a player-tag lookup.
 */
public class FriendLookupResponse extends ApiResponse {
    @JsonProperty("player")
    private FriendInfo player;

    @JsonProperty("relationship")
    private String relationship;

    public FriendInfo getPlayer() {
        return player;
    }

    public void setPlayer(FriendInfo player) {
        this.player = player;
    }
    public String getRelationship() {
        return relationship;
    }

    public void setRelationship(String relationship) {
        this.relationship = relationship;
    }
}
