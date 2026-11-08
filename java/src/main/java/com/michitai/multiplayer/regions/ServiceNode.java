package com.michitai.multiplayer.regions;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * One enabled API region from the discovery directory.
 */
public class ServiceNode {
    @JsonProperty("region")
    private String region;

    @JsonProperty("label")
    private String label;

    @JsonProperty("api_url")
    private String apiUrl;

    @JsonProperty("realtime")
    private RealtimeEndpoint realtime;

    /** Short region code, e.g. "eu", "uk". */
    public String getRegion() {
        return region;
    }

    public void setRegion(String region) {
        this.region = region;
    }

    /** Human label, e.g. "United Kingdom (London)". */
    public String getLabel() {
        return label;
    }

    public void setLabel(String label) {
        this.label = label;
    }

    /** Full API base incl. /api — pass it as the Client baseUrl. */
    public String getApiUrl() {
        return apiUrl;
    }

    public void setApiUrl(String apiUrl) {
        this.apiUrl = apiUrl;
    }

    /** Realtime endpoint for this region; null when none is advertised. */
    public RealtimeEndpoint getRealtime() {
        return realtime;
    }

    public void setRealtime(RealtimeEndpoint realtime) {
        this.realtime = realtime;
    }
}
