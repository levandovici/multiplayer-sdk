package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * A pending lobby or room invitation received from a friend.
 */
public class FriendInvite {
    @JsonProperty("invite_id")
    private String inviteId;

    /** "matchmaking" or "room". */
    @JsonProperty("kind")
    private String kind;

    /** Lobby or room display name. */
    @JsonProperty("name")
    private String name;

    @JsonProperty("from_player")
    private FriendInfo fromPlayer;

    @JsonProperty("created_at")
    private String createdAt;

    public String getInviteId() {
        return inviteId;
    }

    public void setInviteId(String inviteId) {
        this.inviteId = inviteId;
    }
    public String getKind() {
        return kind;
    }

    public void setKind(String kind) {
        this.kind = kind;
    }
    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }
    public FriendInfo getFromPlayer() {
        return fromPlayer;
    }

    public void setFromPlayer(FriendInfo fromPlayer) {
        this.fromPlayer = fromPlayer;
    }
    public String getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(String createdAt) {
        this.createdAt = createdAt;
    }
}
