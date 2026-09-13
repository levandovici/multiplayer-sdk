package com.michitai.multiplayer.rooms.realtime;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Information about the sender of a realtime message.
 * Contains minimal sender details for realtime communication.
 */
public class SenderInfo {
    @JsonProperty("is_host")
    private boolean isHost;

    @JsonProperty("game_player_id")
    private int gamePlayerId;

    @JsonProperty("player_name")
    private String playerName;

    public boolean isHost() {
        return isHost;
    }

    public void setHost(boolean host) {
        isHost = host;
    }

    public int getGamePlayerId() {
        return gamePlayerId;
    }

    public void setGamePlayerId(int gamePlayerId) {
        this.gamePlayerId = gamePlayerId;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }
}
