package com.michitai.multiplayer.friends;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.michitai.multiplayer.Client;

import java.io.IOException;
import java.util.List;

/**
 * Provides methods for the friends system: tag lookups, friend requests,
 * Steam-friend import, and lobby/room invitations. Invited friends join
 * without the lobby password — the invite itself is the server's
 * authorization.
 */
public class Friends {
    private static final ObjectMapper objectMapper = Client.JSON_MAPPER;

    /**
     * Gets the local player's friends list, pending requests, and own player tag.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @return Response containing friends, incoming/outgoing requests and the local player_tag.
     * @throws IOException if the request fails.
     */
    public static FriendListResponse getFriends(Client client, String playerToken) throws IOException {
        return client.send("GET", client.url(Endpoints.FRIENDS_LIST), null, FriendListResponse.class, playerToken, false);
    }

    /**
     * Looks up a player by friend tag ("#XXXXXXXX" or the 8-char body).
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param playerTag The tag to look up, with or without the '#' prefix.
     * @return Response with the player and the local player's relationship to them.
     * @throws IOException if the request fails.
     */
    public static FriendLookupResponse lookupPlayerByTag(Client client, String playerToken, String playerTag) throws IOException {
        return client.send("POST", client.url(Endpoints.FRIENDS_LOOKUP), new FriendTargetRequest(null, playerTag), FriendLookupResponse.class, playerToken, false);
    }

    /**
     * Sends a friend request by tag. If the target already sent one, it is
     * accepted automatically (status "accepted").
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param playerTag The target's friend tag.
     * @return Response with the resulting status ("pending" or "accepted").
     * @throws IOException if the request fails.
     */
    public static FriendStatusResponse sendFriendRequestByTag(Client client, String playerToken, String playerTag) throws IOException {
        return client.send("POST", client.url(Endpoints.FRIENDS_REQUEST), new FriendTargetRequest(null, playerTag), FriendStatusResponse.class, playerToken, false);
    }

    /**
     * Sends a friend request by player ID (e.g. from a Steam-friend lookup).
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param playerId The target player's ID.
     * @return Response with the resulting status ("pending" or "accepted").
     * @throws IOException if the request fails.
     */
    public static FriendStatusResponse sendFriendRequest(Client client, String playerToken, int playerId) throws IOException {
        return client.send("POST", client.url(Endpoints.FRIENDS_REQUEST), new FriendTargetRequest(playerId, null), FriendStatusResponse.class, playerToken, false);
    }

    /**
     * Accepts a pending friend request.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param playerId The requesting player's ID.
     * @return Response with the resulting status.
     * @throws IOException if the request fails.
     */
    public static FriendStatusResponse acceptFriendRequest(Client client, String playerToken, int playerId) throws IOException {
        return client.send("POST", client.url(Endpoints.FRIENDS_ACCEPT), new FriendTargetRequest(playerId, null), FriendStatusResponse.class, playerToken, false);
    }

    /**
     * Declines a pending friend request.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param playerId The requesting player's ID.
     * @return Response with the resulting status.
     * @throws IOException if the request fails.
     */
    public static FriendStatusResponse declineFriendRequest(Client client, String playerToken, int playerId) throws IOException {
        return client.send("POST", client.url(Endpoints.FRIENDS_DECLINE), new FriendTargetRequest(playerId, null), FriendStatusResponse.class, playerToken, false);
    }

    /**
     * Removes a friend.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param playerId The friend's player ID.
     * @return Response with the resulting status.
     * @throws IOException if the request fails.
     */
    public static FriendStatusResponse removeFriend(Client client, String playerToken, int playerId) throws IOException {
        return client.send("POST", client.url(Endpoints.FRIENDS_REMOVE), new FriendTargetRequest(playerId, null), FriendStatusResponse.class, playerToken, false);
    }

    /**
     * Looks up game accounts linked to the given SteamIDs — for importing
     * Steam friends into the backend friends list.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param steamIds SteamID64 strings (max 100).
     * @return Response with the matching players.
     * @throws IOException if the request fails.
     */
    public static SteamFriendsLookupResponse lookupSteamFriends(Client client, String playerToken, List<String> steamIds) throws IOException {
        return client.send("POST", client.url(Endpoints.FRIENDS_STEAM), new SteamFriendsLookupRequest(steamIds), SteamFriendsLookupResponse.class, playerToken, false);
    }

    /**
     * Invites a friend into the local player's current lobby or room.
     * The invitee bypasses password and host approval.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param playerId The friend to invite.
     * @return Response with the invite ID.
     * @throws IOException if the request fails.
     */
    public static FriendInviteResponse inviteFriend(Client client, String playerToken, int playerId) throws IOException {
        return client.send("POST", client.url(Endpoints.FRIENDS_INVITE), new FriendTargetRequest(playerId, null), FriendInviteResponse.class, playerToken, false);
    }

    /**
     * Lists pending lobby/room invites for the local player.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @return Response with pending invites.
     * @throws IOException if the request fails.
     */
    public static FriendInvitesResponse getInvites(Client client, String playerToken) throws IOException {
        return client.send("GET", client.url(Endpoints.FRIENDS_INVITES), null, FriendInvitesResponse.class, playerToken, false);
    }

    /**
     * Accepts a lobby/room invite and joins it directly — no password or host
     * approval required.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param inviteId The invite to accept.
     * @param playerData Optional player data to carry into the lobby/room.
     * @return Response with the joined matchmakingId or roomId.
     * @throws IOException if the request fails.
     */
    public static FriendInviteAcceptResponse acceptInvite(Client client, String playerToken, String inviteId, Object playerData) throws IOException {
        String playerDataJson = playerData != null ? objectMapper.writeValueAsString(playerData) : null;
        return client.send("POST", client.url(Endpoints.FRIENDS_INVITES_ACCEPT),
            new FriendInviteRespondRequest(inviteId, playerDataJson), FriendInviteAcceptResponse.class, playerToken, false);
    }

    /**
     * Accepts a lobby/room invite without player data.
     */
    public static FriendInviteAcceptResponse acceptInvite(Client client, String playerToken, String inviteId) throws IOException {
        return acceptInvite(client, playerToken, inviteId, null);
    }

    /**
     * Declines a lobby/room invite.
     *
     * @param client The API client instance.
     * @param playerToken The player's authentication token.
     * @param inviteId The invite to decline.
     * @return Response with the resulting status.
     * @throws IOException if the request fails.
     */
    public static FriendInviteAcceptResponse declineInvite(Client client, String playerToken, String inviteId) throws IOException {
        return client.send("POST", client.url(Endpoints.FRIENDS_INVITES_DECLINE),
            new FriendInviteRespondRequest(inviteId, null), FriendInviteAcceptResponse.class, playerToken, false);
    }
}
