package com.michitai.multiplayer.rooms.updates;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.rooms.actions.ERoomTargetPlayers;

/**
 * Request for polling updates from specific players.
 */
public class PollUpdatesRequest {
    @JsonProperty("from_players")
    private String fromPlayers;

    @JsonProperty("from_players_ids")
    private int[] fromPlayersIds;

    @JsonProperty("last_update")
    private String lastUpdate;

    public PollUpdatesRequest(ERoomTargetPlayers fromPlayers, int[] fromPlayersIds, String lastUpdate) {
        this.fromPlayers = fromPlayers.name().toLowerCase();
        this.fromPlayersIds = fromPlayersIds;
        this.lastUpdate = lastUpdate;
    }

    public String getFromPlayers() {
        return fromPlayers;
    }

    public void setFromPlayers(String fromPlayers) {
        this.fromPlayers = fromPlayers;
    }

    public int[] getFromPlayersIds() {
        return fromPlayersIds;
    }

    public void setFromPlayersIds(int[] fromPlayersIds) {
        this.fromPlayersIds = fromPlayersIds;
    }

    public String getLastUpdate() {
        return lastUpdate;
    }

    public void setLastUpdate(String lastUpdate) {
        this.lastUpdate = lastUpdate;
    }
}
