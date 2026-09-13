package com.michitai.multiplayer.rooms.updates;

import com.michitai.multiplayer.rooms.actions.ERoomTargetPlayers;

/**
 * Request parameters for sending updates to players in a room.
 * Allows targeting specific players with typed data.
 *
 * @param <T> The type of update data.
 */
public class UpdatePlayers<T> {
    private ERoomTargetPlayers targetPlayers;
    private int[] targetPlayersIds;
    private String type;
    private T data;

    public UpdatePlayers(ERoomTargetPlayers targetPlayers, String type, T data, int[] targetPlayersIds) {
        this.targetPlayers = targetPlayers;
        this.targetPlayersIds = targetPlayersIds;
        this.type = type;
        this.data = data;
    }

    public UpdatePlayers(ERoomTargetPlayers targetPlayers, String type, T data) {
        this(targetPlayers, type, data, null);
    }

    public ERoomTargetPlayers getTargetPlayers() {
        return targetPlayers;
    }

    public void setTargetPlayers(ERoomTargetPlayers targetPlayers) {
        this.targetPlayers = targetPlayers;
    }

    public int[] getTargetPlayersIds() {
        return targetPlayersIds;
    }

    public void setTargetPlayersIds(int[] targetPlayersIds) {
        this.targetPlayersIds = targetPlayersIds;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public T getData() {
        return data;
    }

    public void setData(T data) {
        this.data = data;
    }
}
