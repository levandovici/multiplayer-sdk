package com.michitai.multiplayer.moderation;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Request data for screening a text through the server-side moderation filter.
 */
public class CensorRequest {
    @JsonProperty("text")
    private String text;

    public CensorRequest(String text) {
        this.text = text;
    }

    public String getText() {
        return text;
    }

    public void setText(String text) {
        this.text = text;
    }
}
