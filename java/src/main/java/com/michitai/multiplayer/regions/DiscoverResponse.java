package com.michitai.multiplayer.regions;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

import java.util.List;

/**
 * Enabled service nodes plus the server's geo suggestion.
 */
public class DiscoverResponse extends ApiResponse {
    @JsonProperty("nodes")
    private List<ServiceNode> nodes;

    @JsonProperty("suggested_region")
    private String suggestedRegion;

    /** Every enabled service node, ordered by server priority. */
    public List<ServiceNode> getNodes() {
        return nodes;
    }

    public void setNodes(List<ServiceNode> nodes) {
        this.nodes = nodes;
    }

    /** Server-side geo hint for the calling client; null when unknown. */
    public String getSuggestedRegion() {
        return suggestedRegion;
    }

    public void setSuggestedRegion(String suggestedRegion) {
        this.suggestedRegion = suggestedRegion;
    }
}
