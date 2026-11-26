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

    @JsonProperty("deletion_pending")
    private boolean deletionPending;

    @JsonProperty("purge_after")
    private String purgeAfter;

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

    /**
     * True when the authenticated player's account is inside the deletion
     * grace window — every player-scoped endpoint then fails with this flag
     * set. Restore via Players.restoreAccount before getPurgeAfter.
     */
    public boolean isDeletionPending() {
        return deletionPending;
    }

    public void setDeletionPending(boolean deletionPending) {
        this.deletionPending = deletionPending;
    }

    /**
     * UTC timestamp (ISO 8601) after which an account pending deletion is
     * permanently purged unless restored. Null/empty when not pending.
     */
    public String getPurgeAfter() {
        return purgeAfter;
    }

    public void setPurgeAfter(String purgeAfter) {
        this.purgeAfter = purgeAfter;
    }
}
