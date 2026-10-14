package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;
import java.util.List;

/**
 * Response containing pending lobby/room invites for the local player.
 */
public class FriendInvitesResponse extends ApiResponse {
    @JsonProperty("invites")
    private List<FriendInvite> invites;

    public List<FriendInvite> getInvites() {
        return invites;
    }

    public void setInvites(List<FriendInvite> invites) {
        this.invites = invites;
    }
}
