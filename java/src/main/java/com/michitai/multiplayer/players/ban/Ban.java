package com.michitai.multiplayer.players.ban;

import com.michitai.multiplayer.ApiResponse;

/**
 * Utility methods for ban-related operations.
 */
public final class Ban {
    private Ban() {
    }

    /**
     * Checks if an API response indicates the player is banned.
     *
     * @param response The API response to check.
     * @return True if the error message indicates the player is banned, false otherwise.
     */
    public static boolean isBanned(ApiResponse response) {
        return response != null
            && !response.isSuccess()
            && response.getError() != null
            && response.getError().contains("You are banned");
    }
}
