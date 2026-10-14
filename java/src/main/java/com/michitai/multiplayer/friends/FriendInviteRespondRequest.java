package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request body for accepting or declining a lobby/room invite.
 */
public class FriendInviteRespondRequest {
    @JsonProperty("invite_id")
    private String inviteId;

    @JsonProperty("player_data_json")
    private String playerDataJson;

    public FriendInviteRespondRequest(String inviteId, String playerDataJson) {
        this.inviteId = inviteId;
        this.playerDataJson = playerDataJson;
    }

    public String getInviteId() {
        return inviteId;
    }

    public void setInviteId(String inviteId) {
        this.inviteId = inviteId;
    }
    public String getPlayerDataJson() {
        return playerDataJson;
    }

    public void setPlayerDataJson(String playerDataJson) {
        this.playerDataJson = playerDataJson;
    }
}
