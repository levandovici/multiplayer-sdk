package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Public information about a player in a friends context — never contains
 * tokens or player data.
 */
public class FriendInfo {
    @JsonProperty("player_id")
    private int playerId;

    @JsonProperty("player_name")
    private String playerName;

    /** The player's public friend tag in display form ("#XXXXXXXX"). */
    @JsonProperty("player_tag")
    private String playerTag;

    @JsonProperty("is_online")
    private boolean isOnline;

    @JsonProperty("steam_id")
    private String steamId;

    /**
     * Relationship to the local player: "friend", "incoming", "outgoing" or
     * null when unrelated.
     */
    @JsonProperty("relationship")
    private String relationship;

    @JsonProperty("requested_at")
    private String requestedAt;

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
    public String getPlayerTag() {
        return playerTag;
    }

    public void setPlayerTag(String playerTag) {
        this.playerTag = playerTag;
    }
    public boolean isIsOnline() {
        return isOnline;
    }

    public void setIsOnline(boolean isOnline) {
        this.isOnline = isOnline;
    }
    public String getSteamId() {
        return steamId;
    }

    public void setSteamId(String steamId) {
        this.steamId = steamId;
    }
    public String getRelationship() {
        return relationship;
    }

    public void setRelationship(String relationship) {
        this.relationship = relationship;
    }
    public String getRequestedAt() {
        return requestedAt;
    }

    public void setRequestedAt(String requestedAt) {
        this.requestedAt = requestedAt;
    }
}
