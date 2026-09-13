package com.michitai.multiplayer.rooms.actions;

/**
 * Request parameters for submitting an action to players in a room.
 * Allows targeting specific players with typed action data.
 *
 * @param <T> The type of request data.
 */
public class SubmitAction<T> {
    private ERoomTargetPlayers targetPlayers;
    private int[] targetPlayersIds;
    private String actionType;
    private T requestData;

    public SubmitAction(ERoomTargetPlayers targetPlayers, String actionType, T requestData, int[] targetPlayersIds) {
        this.targetPlayers = targetPlayers;
        this.targetPlayersIds = targetPlayersIds;
        this.actionType = actionType;
        this.requestData = requestData;
    }

    public SubmitAction(ERoomTargetPlayers targetPlayers, String actionType, T requestData) {
        this(targetPlayers, actionType, requestData, null);
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

    public String getActionType() {
        return actionType;
    }

    public void setActionType(String actionType) {
        this.actionType = actionType;
    }

    public T getRequestData() {
        return requestData;
    }

    public void setRequestData(T requestData) {
        this.requestData = requestData;
    }
}
