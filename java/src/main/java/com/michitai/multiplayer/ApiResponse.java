package com.michitai.multiplayer;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Base class for all API responses.
 * Provides success status and error information.
 */
public abstract class ApiResponse {
    @JsonProperty("success")
    private boolean success;

    @JsonProperty("error")
    private String error;

    public boolean isSuccess() {
        return success;
    }

    public void setSuccess(boolean success) {
        this.success = success;
    }

    public String getError() {
        return error;
    }

    public void setError(String error) {
        this.error = error;
    }
}
