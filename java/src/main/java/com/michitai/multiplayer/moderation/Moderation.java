package com.michitai.multiplayer.moderation;

import com.michitai.multiplayer.Client;

import java.io.IOException;

/**
 * Provides methods for screening player-visible text through the
 * server-side moderation filter. The filter normalizes leetspeak,
 * confusable scripts, separators and repeated-letter evasion before
 * matching, so "w e e d" or "w33d" are caught the same as "weed".
 */
public class Moderation {
    /**
     * Screens text and returns whether it is clean plus a filtered copy.
     *
     * @param client The API client instance.
     * @param text The text to screen (max 1000 characters).
     * @return Response with clean (no listed word found) and filtered (masked display text).
     * @throws IOException if the request fails.
     */
    public static CensorResponse censor(Client client, String text) throws IOException {
        return client.post(client.url(Endpoints.CENSOR), new CensorRequest(text), CensorResponse.class);
    }
}
