package com.michitai.multiplayer.moderation;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.michitai.multiplayer.ApiResponse;

/**
 * Response from the text screening endpoint.
 */
public class CensorResponse extends ApiResponse {
    @JsonProperty("clean")
    private boolean clean;

    @JsonProperty("filtered")
    private String filtered;

    @JsonProperty("retry_after")
    private Integer retryAfter;

    /**
     * @return true when the text contains no listed word after normalization.
     */
    public boolean isClean() {
        return clean;
    }

    public void setClean(boolean clean) {
        this.clean = clean;
    }

    /**
     * @return the text with listed words masked and markup/control
     *         characters stripped — safe to display as-is.
     */
    public String getFiltered() {
        return filtered;
    }

    public void setFiltered(String filtered) {
        this.filtered = filtered;
    }

    /**
     * @return seconds until the rate-limit bucket refills (only on a 429 reply).
     */
    public Integer getRetryAfter() {
        return retryAfter;
    }

    public void setRetryAfter(Integer retryAfter) {
        this.retryAfter = retryAfter;
    }
}
