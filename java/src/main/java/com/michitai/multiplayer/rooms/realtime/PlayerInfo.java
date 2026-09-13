package com.michitai.multiplayer.rooms.realtime;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Player information for realtime WebSocket connections.
 * Contains minimal player details for realtime communication.
 */
public class PlayerInfo {
    @JsonProperty("player_id")
    private int playerId;

    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("room_id")
    private String roomId;

    @JsonProperty("is_host")
    private boolean isHost;

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public String getRoomId() {
        return roomId;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
    }

    public boolean isHost() {
        return isHost;
    }

    public void setHost(boolean host) {
        isHost = host;
    }
}
