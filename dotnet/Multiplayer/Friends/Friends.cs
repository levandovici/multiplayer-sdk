using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Provides methods for the friends system: tag lookups, friend requests,
    /// Steam-friend import, and lobby/room invitations. Invited friends join
    /// without the lobby password — the invite itself is the server's
    /// authorization.
    /// </summary>
    public static class Friends
    {
        /// <summary>
        /// Gets the local player's friends list, pending requests, and own player tag.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response containing friends, incoming/outgoing requests and the local player_tag.</returns>
        public static Task<FriendListResponse> GetFriendsAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<FriendListResponse>(HttpMethod.Get, client.Url(Endpoints.FriendsList), null, ct, playerToken: playerToken);

        /// <summary>
        /// Looks up a player by friend tag ("#XXXXXXXX" or the 8-char body).
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="playerTag">The tag to look up, with or without the '#' prefix.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response with the player and the local player's relationship to them.</returns>
        public static Task<FriendLookupResponse> LookupPlayerByTagAsync(Client client, string playerToken, string playerTag, CancellationToken ct = default)
            => client.Send<FriendLookupResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsLookup), new FriendTargetRequest(playerTag: playerTag), ct, playerToken: playerToken);

        /// <summary>
        /// Sends a friend request by tag. If the target already sent one, it is
        /// accepted automatically (status "accepted").
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="playerTag">The target's friend tag.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response with the resulting status ("pending" or "accepted").</returns>
        public static Task<FriendStatusResponse> SendFriendRequestByTagAsync(Client client, string playerToken, string playerTag, CancellationToken ct = default)
            => client.Send<FriendStatusResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsRequest), new FriendTargetRequest(playerTag: playerTag), ct, playerToken: playerToken);

        /// <summary>
        /// Sends a friend request by player ID (e.g. from a Steam-friend lookup
        /// or a lobby player list).
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="playerId">The target player's ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response with the resulting status ("pending" or "accepted").</returns>
        public static Task<FriendStatusResponse> SendFriendRequestAsync(Client client, string playerToken, int playerId, CancellationToken ct = default)
            => client.Send<FriendStatusResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsRequest), new FriendTargetRequest(playerId: playerId), ct, playerToken: playerToken);

        /// <summary>
        /// Accepts a pending incoming friend request.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="playerId">The requester's player ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response confirming the friendship.</returns>
        public static Task<FriendStatusResponse> AcceptFriendRequestAsync(Client client, string playerToken, int playerId, CancellationToken ct = default)
            => client.Send<FriendStatusResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsAccept), new FriendTargetRequest(playerId: playerId), ct, playerToken: playerToken);

        /// <summary>
        /// Declines a pending incoming friend request.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="playerId">The requester's player ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response confirming the decline.</returns>
        public static Task<FriendStatusResponse> DeclineFriendRequestAsync(Client client, string playerToken, int playerId, CancellationToken ct = default)
            => client.Send<FriendStatusResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsDecline), new FriendTargetRequest(playerId: playerId), ct, playerToken: playerToken);

        /// <summary>
        /// Removes a friend or cancels a pending request in either direction.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="playerId">The other player's ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response confirming the removal.</returns>
        public static Task<FriendStatusResponse> RemoveFriendAsync(Client client, string playerToken, int playerId, CancellationToken ct = default)
            => client.Send<FriendStatusResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsRemove), new FriendTargetRequest(playerId: playerId), ct, playerToken: playerToken);

        /// <summary>
        /// Maps Steam friends to game accounts. Collect SteamID64 strings from
        /// the local Steam friends list and submit them here — the response
        /// tells which Steam friends already have accounts and the relationship
        /// each one has to the local player.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="steamIds">SteamID64 strings (max 100 per call).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response with matching players and their relationship.</returns>
        public static Task<SteamFriendsLookupResponse> LookupSteamFriendsAsync(Client client, string playerToken, List<string> steamIds, CancellationToken ct = default)
            => client.Send<SteamFriendsLookupResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsSteam), new SteamFriendsLookupRequest(steamIds), ct, playerToken: playerToken);

        /// <summary>
        /// Invites a friend into the lobby or room the local player is
        /// currently in. Friends only — call GetFriendsAsync for candidates.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="playerId">The friend to invite.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response with the invite id.</returns>
        public static Task<FriendInviteResponse> InviteFriendAsync(Client client, string playerToken, int playerId, CancellationToken ct = default)
            => client.Send<FriendInviteResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsInvite), new FriendTargetRequest(playerId: playerId), ct, playerToken: playerToken);

        /// <summary>
        /// Lists pending lobby/room invitations addressed to the local player.
        /// Poll this alongside the heartbeat while waiting in the menu.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response with pending invites.</returns>
        public static Task<FriendInvitesResponse> GetInvitesAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<FriendInvitesResponse>(HttpMethod.Get, client.Url(Endpoints.FriendsInvites), null, ct, playerToken: playerToken);

        /// <summary>
        /// Accepts an invite — joins the lobby or room server-side, bypassing
        /// its password. The response carries the joined destination id.
        /// </summary>
        /// <typeparam name="T">The type of player data to join with.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="inviteId">The invite to accept.</param>
        /// <param name="playerData">Optional player data to join with.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response with the joined matchmaking_id or room_id.</returns>
        public static Task<FriendInviteAcceptResponse> AcceptInviteAsync<T>(Client client, string playerToken, string inviteId, T? playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<FriendInviteAcceptResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsInvitesAccept),
                new FriendInviteRespondRequest<T>(inviteId, playerData), ct, playerToken: playerToken);

        /// <summary>
        /// Declines an invite.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="inviteId">The invite to decline.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Response confirming the decline.</returns>
        public static Task<FriendInviteAcceptResponse> DeclineInviteAsync(Client client, string playerToken, string inviteId, CancellationToken ct = default)
            => client.Send<FriendInviteAcceptResponse>(HttpMethod.Post, client.Url(Endpoints.FriendsInvitesDecline),
                new FriendInviteRespondRequest<object>(inviteId), ct, playerToken: playerToken);
    }
}
