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

    @JsonProperty("auth")
    private String auth;

    @JsonProperty("account")
    private String account;

    @JsonProperty("steam_id")
    private String steamId;

    /** The player's public friend tag in display form ("#XXXXXXXX"). */
    @JsonProperty("player_tag")
    private String playerTag;


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

    public String getAuth() {
        return auth;
    }

    public void setAuth(String auth) {
        this.auth = auth;
    }

    public String getAccount() {
        return account;
    }

    public void setAccount(String account) {
        this.account = account;
    }

    public String getSteamId() {
        return steamId;
    }

    public void setSteamId(String steamId) {
        this.steamId = steamId;
    }

    public String getPlayerTag() {
        return playerTag;
    }

    public void setPlayerTag(String playerTag) {
        this.playerTag = playerTag;
    }
}
