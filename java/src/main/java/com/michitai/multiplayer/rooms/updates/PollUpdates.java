package com.michitai.multiplayer.rooms.updates;

import com.michitai.multiplayer.rooms.actions.ERoomTargetPlayers;

/**
 * Request parameters for polling updates in a room.
 * Allows filtering updates by source and time.
 */
public class PollUpdates {
    private ERoomTargetPlayers fromPlayers;
    private int[] fromPlayersIds;
    private String lastUpdate;

    public PollUpdates(ERoomTargetPlayers fromPlayers, int[] fromPlayersIds, String lastUpdate) {
        this.fromPlayers = fromPlayers;
        this.fromPlayersIds = fromPlayersIds;
        this.lastUpdate = lastUpdate;
    }

    public PollUpdates(ERoomTargetPlayers fromPlayers) {
        this(fromPlayers, null, null);
    }

    public PollUpdates() {
        this(ERoomTargetPlayers.HOST, null, null);
    }

    public ERoomTargetPlayers getFromPlayers() {
        return fromPlayers;
    }

    public void setFromPlayers(ERoomTargetPlayers fromPlayers) {
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
