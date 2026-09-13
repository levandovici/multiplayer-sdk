package com.michitai.multiplayer;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Standard success response for API operations that don't return specific data.
 * Includes a message and timestamp for confirmation.
 */
public class SuccessResponse extends ApiResponse {
    @JsonProperty("message")
    private String message;

    @JsonProperty("updated_at")
    private String updatedAt;

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public String getUpdatedAt() {
        return updatedAt;
    }

    public void setUpdatedAt(String updatedAt) {
        this.updatedAt = updatedAt;
    }
}
