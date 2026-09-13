package com.michitai.multiplayer.time;

import com.michitai.multiplayer.Client;

import java.io.IOException;

/**
 * Provides methods for querying server time information.
 */
public class Time {
    /**
     * Retrieves the current server time in UTC.
     *
     * @param client The API client instance.
     * @return Response containing the server UTC time, timestamp, and readable format.
     * @throws IOException if the request fails.
     */
    public static ServerTimeResponse getServerTime(Client client) throws IOException {
        return client.get(client.url(Endpoints.TIME), ServerTimeResponse.class);
    }

    /**
     * Retrieves the server time with a specified UTC offset.
     *
     * @param client The API client instance.
     * @param utcOffset The UTC offset in hours (e.g., 3 for UTC+3, -5 for UTC-5).
     * @return Response containing the adjusted time with offset information.
     * @throws IOException if the request fails.
     */
    public static ServerTimeWithOffsetResponse getServerTimeWithOffset(Client client, int utcOffset) throws IOException {
        String offsetParam = String.format("&utc=%+d", utcOffset);
        return client.get(client.url(Endpoints.TIME, offsetParam), ServerTimeWithOffsetResponse.class);
    }
}
