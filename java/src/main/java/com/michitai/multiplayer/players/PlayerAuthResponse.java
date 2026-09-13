package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response returned when a player is successfully authenticated.
 * Contains the player's full information including typed player data.
 *
 * @param <T> The type to deserialize player data into.
 */
public class PlayerAuthResponse<T> extends ApiResponse {
    @JsonProperty("player")
    private PlayerInfo<T> player;

    public PlayerInfo<T> getPlayer() {
        return player;
    }

    public void setPlayer(PlayerInfo<T> player) {
        this.player = player;
    }
}
