package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.annotation.JsonProperty;
import java.util.List;

/**
 * Request body for looking up game accounts linked to a set of SteamIDs.
 */
public class SteamFriendsLookupRequest {
    @JsonProperty("steam_ids")
    private List<String> steamIds;

    public SteamFriendsLookupRequest(List<String> steamIds) {
        this.steamIds = steamIds;
    }

    public List<String> getSteamIds() {
        return steamIds;
    }

    public void setSteamIds(List<String> steamIds) {
        this.steamIds = steamIds;
    }
}
