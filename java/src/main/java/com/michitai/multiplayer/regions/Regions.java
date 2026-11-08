package com.michitai.multiplayer.regions;

import com.michitai.multiplayer.Client;

import java.io.IOException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;

/**
 * Region discovery &amp; game resolution.
 *
 * {@link #discover} returns every enabled service node plus the server's geo
 * guess. A game is pinned to exactly one node — chosen when the API key is
 * created; {@link #resolveGame} asks the directory which node hosts a game.
 * Player accounts live on that node only and never roam — pin the client's
 * baseUrl to it.
 */
public class Regions {
    private Regions() {
    }

    /**
     * Fetch the enabled service-node directory.
     *
     * @param client The API client instance.
     * @return Node list plus the server's suggested_region hint.
     * @throws IOException if the request fails.
     */
    public static DiscoverResponse discover(Client client) throws IOException {
        return client.get(client.url(Endpoints.DISCOVER), DiscoverResponse.class);
    }

    /**
     * Resolve which node hosts a game.
     *
     * Point every subsequent REST and realtime call at the returned node's
     * apiUrl: the game's players, rooms and leaderboards exist only there,
     * and other nodes answer 421 with a home_api_url hint for the same
     * resolution.
     *
     * @param client The API client instance (queries the directory — use the central base URL).
     * @param apiKey The game's public api_key.
     * @return Region code plus the pinned node.
     * @throws IOException if the request fails.
     */
    public static GameNodeResponse resolveGame(Client client, String apiKey) throws IOException {
        // The client attaches X-Api-Token on every request — POST with no
        // query string; the key never travels in a URL.
        return client.post(client.url(Endpoints.DISCOVER), apiKey, GameNodeResponse.class);
    }
}
