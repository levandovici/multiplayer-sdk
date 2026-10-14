package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request body for friend operations that target a player by ID or tag.
 */
public class FriendTargetRequest {
    @JsonProperty("player_id")
    private Integer playerId;

    @JsonProperty("player_tag")
    private String playerTag;

    public FriendTargetRequest(Integer playerId, String playerTag) {
        this.playerId = playerId;
        this.playerTag = playerTag;
    }

    public Integer getPlayerId() {
        return playerId;
    }

    public void setPlayerId(Integer playerId) {
        this.playerId = playerId;
    }
    public String getPlayerTag() {
        return playerTag;
    }

    public void setPlayerTag(String playerTag) {
        this.playerTag = playerTag;
    }
}
