package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response for accepting or declining a lobby/room invite. On accept,
 * matchmakingId or roomId identifies the joined target.
 */
public class FriendInviteAcceptResponse extends ApiResponse {
    @JsonProperty("status")
    private String status;

    @JsonProperty("invite_id")
    private String inviteId;

    @JsonProperty("matchmaking_id")
    private String matchmakingId;

    @JsonProperty("room_id")
    private String roomId;

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }
    public String getInviteId() {
        return inviteId;
    }

    public void setInviteId(String inviteId) {
        this.inviteId = inviteId;
    }
    public String getMatchmakingId() {
        return matchmakingId;
    }

    public void setMatchmakingId(String matchmakingId) {
        this.matchmakingId = matchmakingId;
    }
    public String getRoomId() {
        return roomId;
    }

    public void setRoomId(String roomId) {
        this.roomId = roomId;
    }
}
