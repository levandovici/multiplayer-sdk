package com.michitai.multiplayer.time;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Contains UTC offset information for time calculations.
 * Stores offset details including hours, string representation, and original timestamps.
 */
public class TimeOffset {
    @JsonProperty("offset_hours")
    private int offsetHours;

    @JsonProperty("offset_string")
    private String offsetString;

    @JsonProperty("original_utc")
    private String originalUtc;

    @JsonProperty("original_timestamp")
    private long originalTimestamp;

    public int getOffsetHours() {
        return offsetHours;
    }

    public void setOffsetHours(int offsetHours) {
        this.offsetHours = offsetHours;
    }

    public String getOffsetString() {
        return offsetString;
    }

    public void setOffsetString(String offsetString) {
        this.offsetString = offsetString;
    }

    public String getOriginalUtc() {
        return originalUtc;
    }

    public void setOriginalUtc(String originalUtc) {
        this.originalUtc = originalUtc;
    }

    public long getOriginalTimestamp() {
        return originalTimestamp;
    }

    public void setOriginalTimestamp(long originalTimestamp) {
        this.originalTimestamp = originalTimestamp;
    }
}
