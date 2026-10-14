package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;
import java.util.List;

/**
 * Response for a SteamID lookup — players linked to the given Steam accounts.
 */
public class SteamFriendsLookupResponse extends ApiResponse {
    @JsonProperty("players")
    private List<FriendInfo> players;

    public List<FriendInfo> getPlayers() {
        return players;
    }

    public void setPlayers(List<FriendInfo> players) {
        this.players = players;
    }
}
