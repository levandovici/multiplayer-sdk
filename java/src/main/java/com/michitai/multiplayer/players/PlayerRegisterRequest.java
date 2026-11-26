package com.michitai.multiplayer.players;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request for registering a new player.
 */
public class PlayerRegisterRequest {
    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("player_data")
    private String playerData;

    @JsonProperty("steam_ticket")
    private String steamTicket;

    @JsonProperty("steam_identity")
    private String steamIdentity;

    @JsonProperty("google_auth_code")
    private String googleAuthCode;

    @JsonProperty("google_integrity_token")
    private String googleIntegrityToken;

    @JsonProperty("restore")
    private boolean restore;

    public PlayerRegisterRequest(String playerName, String playerData) {
        this(playerName, playerData, null, null);
    }

    public PlayerRegisterRequest(String playerName, String playerData, String steamTicket, String steamIdentity) {
        this(playerName, playerData, steamTicket, steamIdentity, null, null);
    }

    public PlayerRegisterRequest(String playerName, String playerData, String steamTicket, String steamIdentity,
            String googleAuthCode, String googleIntegrityToken) {
        this(playerName, playerData, steamTicket, steamIdentity, googleAuthCode, googleIntegrityToken, false);
    }

    public PlayerRegisterRequest(String playerName, String playerData, String steamTicket, String steamIdentity,
            String googleAuthCode, String googleIntegrityToken, boolean restore) {
        this.playerName = playerName;
        this.playerData = playerData;
        this.steamTicket = steamTicket;
        this.steamIdentity = steamIdentity;
        this.googleAuthCode = googleAuthCode;
        this.googleIntegrityToken = googleIntegrityToken;
        this.restore = restore;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public String getPlayerData() {
        return playerData;
    }

    public void setPlayerData(String playerData) {
        this.playerData = playerData;
    }

    public String getSteamTicket() {
        return steamTicket;
    }

    public void setSteamTicket(String steamTicket) {
        this.steamTicket = steamTicket;
    }

    public String getSteamIdentity() {
        return steamIdentity;
    }

    public void setSteamIdentity(String steamIdentity) {
        this.steamIdentity = steamIdentity;
    }

    public String getGoogleAuthCode() {
        return googleAuthCode;
    }

    public void setGoogleAuthCode(String googleAuthCode) {
        this.googleAuthCode = googleAuthCode;
    }

    public String getGoogleIntegrityToken() {
        return googleIntegrityToken;
    }

    public void setGoogleIntegrityToken(String googleIntegrityToken) {
        this.googleIntegrityToken = googleIntegrityToken;
    }

    public boolean isRestore() {
        return restore;
    }

    public void setRestore(boolean restore) {
        this.restore = restore;
    }
}
