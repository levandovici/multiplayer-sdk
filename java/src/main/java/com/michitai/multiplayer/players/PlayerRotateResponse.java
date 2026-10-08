package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response returned when a player's private key is successfully rotated.
 * The previous token is invalidated; store the new key returned here.
 */
public class PlayerRotateResponse extends ApiResponse {
    @JsonProperty("private_key")
    private String privateKey;

    @JsonProperty("player_id")
    private int playerId;

    /**
     * The new private key token. Shown only once - persist it client-side.
     */
    public String getPrivateKey() {
        return privateKey;
    }

    public void setPrivateKey(String privateKey) {
        this.privateKey = privateKey;
    }

    /**
     * The ID of the player whose key was rotated.
     */
    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }
}
