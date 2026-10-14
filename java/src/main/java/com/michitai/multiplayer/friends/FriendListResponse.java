package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;
import java.util.List;

/**
 * Response containing the local player's tag, friends, and pending requests.
 */
public class FriendListResponse extends ApiResponse {
    @JsonProperty("player_tag")
    private String playerTag;

    @JsonProperty("friends")
    private List<FriendInfo> friends;

    @JsonProperty("incoming")
    private List<FriendInfo> incoming;

    @JsonProperty("outgoing")
    private List<FriendInfo> outgoing;

    public String getPlayerTag() {
        return playerTag;
    }

    public void setPlayerTag(String playerTag) {
        this.playerTag = playerTag;
    }
    public List<FriendInfo> getFriends() {
        return friends;
    }

    public void setFriends(List<FriendInfo> friends) {
        this.friends = friends;
    }
    public List<FriendInfo> getIncoming() {
        return incoming;
    }

    public void setIncoming(List<FriendInfo> incoming) {
        this.incoming = incoming;
    }
    public List<FriendInfo> getOutgoing() {
        return outgoing;
    }

    public void setOutgoing(List<FriendInfo> outgoing) {
        this.outgoing = outgoing;
    }
}
