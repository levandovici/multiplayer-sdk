using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Regions
{
    /// <summary>
    /// Region discovery &amp; game resolution.
    ///
    /// <see cref="Discover"/> returns every enabled service node plus the
    /// server's geo guess. A game is pinned to exactly one node — chosen
    /// when the API key is created; <see cref="ResolveGame"/> asks the
    /// directory which node hosts a game. Player accounts live on that
    /// node only and never roam — pin the client's baseUrl to it.
    /// </summary>
    public static class Regions
    {
        /// <summary>
        /// Fetch the enabled service-node directory.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Node list plus the server's suggested_region hint.</returns>
        public static Task<DiscoverResponse> Discover(Client client, CancellationToken ct = default)
            => client.Send<DiscoverResponse>(HttpMethod.Get, client.Url(Endpoints.Discover), null, ct);

        /// <summary>
        /// Resolve which node hosts a game.
        ///
        /// Point every subsequent REST and realtime call at the returned
        /// node's <see cref="ServiceNode.api_url"/>: the game's players,
        /// rooms and leaderboards exist only there, and other nodes answer
        /// 421 with a home_api_url hint for the same resolution.
        /// </summary>
        /// <param name="client">The API client instance (queries the directory — use the central base URL).</param>
        /// <param name="apiKey">The game's public api_key.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Region code plus the pinned node.</returns>
        public static Task<GameNodeResponse> ResolveGame(Client client, string apiKey, CancellationToken ct = default)
            // The client attaches X-Api-Token on every request — POST with
            // no query string; the key never travels in a URL.
            => client.Send<GameNodeResponse>(HttpMethod.Post, client.Url(Endpoints.Discover), apiKey, ct);
    }
}
