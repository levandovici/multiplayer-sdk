package com.michitai.multiplayer.rooms.realtime;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response containing the WebSocket token for realtime communication.
 * Includes player and server connection information.
 */
public class TokenResponse extends ApiResponse {
    @JsonProperty("token")
    private String token;

    @JsonProperty("player_info")
    private PlayerInfo playerInfo;

    @JsonProperty("realtime_server")
    private RealtimeServerInfo realtimeServer;

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public PlayerInfo getPlayerInfo() {
        return playerInfo;
    }

    public void setPlayerInfo(PlayerInfo playerInfo) {
        this.playerInfo = playerInfo;
    }

    public RealtimeServerInfo getRealtimeServer() {
        return realtimeServer;
    }

    public void setRealtimeServer(RealtimeServerInfo realtimeServer) {
        this.realtimeServer = realtimeServer;
    }
}
