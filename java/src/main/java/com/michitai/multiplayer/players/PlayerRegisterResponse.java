package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response returned when a new player is successfully registered.
 * Contains the player's ID, private key for authentication, and game information.
 */
public class PlayerRegisterResponse extends ApiResponse {
    @JsonProperty("player_id")
    private int playerId;

    @JsonProperty("private_key")
    private String privateKey;

    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("game_id")
    private int gameId;

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }

    public String getPrivateKey() {
        return privateKey;
    }

    public void setPrivateKey(String privateKey) {
        this.privateKey = privateKey;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public int getGameId() {
        return gameId;
    }

    public void setGameId(int gameId) {
        this.gameId = gameId;
    }
}
