package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response returned when a game is successfully started from matchmaking.
 * Contains the created room ID and transfer information.
 */
public class MatchmakingStartResponse extends ApiResponse {
    @JsonProperty("room_id")
    private String roomId;

    @JsonProperty("room_name")
    private String roomName;

    @JsonProperty("players_transferred")
    private int playersTransferred;

    @JsonProperty("message")
    private String message;

    public String getRoomId() {
        return roomId;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
    }

    public String getRoomName() {
        return roomName;
    }

    public void setRoomName(String roomName) {
        this.roomName = roomName;
    }

    public int getPlayersTransferred() {
        return playersTransferred;
    }

    public void setPlayersTransferred(int playersTransferred) {
        this.playersTransferred = playersTransferred;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }
}
