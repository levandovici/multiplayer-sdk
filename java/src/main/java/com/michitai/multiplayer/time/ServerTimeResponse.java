package com.michitai.multiplayer.time;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response containing the server UTC time, timestamp, and readable format.
 */
public class ServerTimeResponse extends ApiResponse {
    @JsonProperty("utc")
    private String utc;

    @JsonProperty("timestamp")
    private long timestamp;

    @JsonProperty("readable")
    private String readable;

    public String getUtc() {
        return utc;
    }

    public void setUtc(String utc) {
        this.utc = utc;
    }

    public long getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(long timestamp) {
        this.timestamp = timestamp;
    }

    public String getReadable() {
        return readable;
    }

    public void setReadable(String readable) {
        this.readable = readable;
    }
}
