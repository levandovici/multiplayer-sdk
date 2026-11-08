package com.michitai.multiplayer.regions;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Result of discover.php?api_key=… — the node a game is pinned to.
 */
public class GameNodeResponse extends ApiResponse {
    @JsonProperty("region")
    private String region;

    @JsonProperty("node")
    private ServiceNode node;

    /** Region code the game is pinned to. */
    public String getRegion() {
        return region;
    }

    public void setRegion(String region) {
        this.region = region;
    }

    /** The pinned node (api_url + realtime endpoint); null when unavailable. */
    public ServiceNode getNode() {
        return node;
    }

    public void setNode(ServiceNode node) {
        this.node = node;
    }
}
