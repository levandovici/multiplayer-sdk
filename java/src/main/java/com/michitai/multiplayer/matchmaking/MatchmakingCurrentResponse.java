package com.michitai.multiplayer.matchmaking;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.JsonNode;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Response containing the current matchmaking status for a player.
 * Includes lobby information and pending join requests.
 *
 * @param <T> The type to deserialize lobby rules into.
 */
public class MatchmakingCurrentResponse<T> extends ApiResponse {
    @JsonProperty("in_matchmaking")
    private boolean inMatchmaking;

    @JsonProperty("matchmaking")
    private MatchmakingInfo<T> matchmaking;

    @JsonProperty("pending_requests")
    private List<MatchmakingRequestBase> pendingRequests;

    public boolean isInMatchmaking() {
        return inMatchmaking;
    }

    public void setInMatchmaking(boolean inMatchmaking) {
        this.inMatchmaking = inMatchmaking;
    }

    public MatchmakingInfo<T> getMatchmaking() {
        return matchmaking;
    }

    public void setMatchmaking(MatchmakingInfo<T> matchmaking) {
        this.matchmaking = matchmaking;
    }

    public List<MatchmakingRequestBase> getPendingRequests() {
        return pendingRequests;
    }

    public void setPendingRequests(List<MatchmakingRequestBase> pendingRequests) {
        this.pendingRequests = pendingRequests;
    }
}
