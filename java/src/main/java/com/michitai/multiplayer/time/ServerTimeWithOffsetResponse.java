package com.michitai.multiplayer.time;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response containing the adjusted time with offset information.
 */
public class ServerTimeWithOffsetResponse extends ApiResponse {
    @JsonProperty("utc")
    private String utc;

    @JsonProperty("timestamp")
    private long timestamp;

    @JsonProperty("readable")
    private String readable;

    @JsonProperty("utc_offset")
    private int utcOffset;

    @JsonProperty("offset_readable")
    private String offsetReadable;

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

    public int getUtcOffset() {
        return utcOffset;
    }

    public void setUtcOffset(int utcOffset) {
        this.utcOffset = utcOffset;
    }

    public String getOffsetReadable() {
        return offsetReadable;
    }

    public void setOffsetReadable(String offsetReadable) {
        this.offsetReadable = offsetReadable;
    }
}
