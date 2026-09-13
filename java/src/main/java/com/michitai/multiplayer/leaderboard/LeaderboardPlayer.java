package com.michitai.multiplayer.leaderboard;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Information about a player on the leaderboard.
 * Contains rank, player details, and custom data.
 *
 * @param <T> The type to deserialize player data into.
 */
public class LeaderboardPlayer<T> {
    @JsonProperty("rank")
    private int rank;

    @JsonProperty("player_id")
    private int playerId;

    @JsonProperty("player_name")
    private String playerName;

    @JsonProperty("player_data_json")
    private String playerDataJson;

    @JsonProperty("player_data")
    private T playerData;

    public int getRank() {
        return rank;
    }

    public void setRank(int rank) {
        this.rank = rank;
    }

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

    public String getPlayerDataJson() {
        return playerDataJson;
    }

    public void setPlayerDataJson(String playerDataJson) {
        this.playerDataJson = playerDataJson;
    }

    public T getPlayerData() {
        return playerData;
    }

    public void setPlayerData(T playerData) {
        this.playerData = playerData;
    }
}
