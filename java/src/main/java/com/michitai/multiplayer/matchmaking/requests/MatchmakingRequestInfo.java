package com.michitai.multiplayer.matchmaking.requests;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.matchmaking.MatchmakingRequestBase;

/**
 * Information about a matchmaking join request, including responder details.
 */
public class MatchmakingRequestInfo extends MatchmakingRequestBase {
    @JsonProperty("responded_by")
    private Integer respondedBy;

    @JsonProperty("responder_name")
    private String responderName;

    @JsonProperty("join_by_requests")
    private boolean joinByRequests;

    public Integer getRespondedBy() {
        return respondedBy;
    }

    public void setRespondedBy(Integer respondedBy) {
        this.respondedBy = respondedBy;
    }

    public String getResponderName() {
        return responderName;
    }

    public void setResponderName(String responderName) {
        this.responderName = responderName;
    }

    public boolean isJoinByRequests() {
        return joinByRequests;
    }

    public void setJoinByRequests(boolean joinByRequests) {
        this.joinByRequests = joinByRequests;
    }
}
