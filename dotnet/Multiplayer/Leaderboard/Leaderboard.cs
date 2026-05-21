using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    /// <summary>
    /// Provides methods for querying and retrieving leaderboard rankings.
    /// </summary>
    public class Leaderboard
    {
        /// <summary>
        /// Retrieves the leaderboard with specified sorting and limit.
        /// </summary>
        /// <typeparam name="T">The type to deserialize player data into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="sortBy">Array of field names to sort by (e.g., ["level", "wins"]).</param>
        /// <param name="limit">Maximum number of results to return (1-100, default: 10).</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the leaderboard entries with rankings.</returns>
        public static Task<LeaderboardResponse<T>> GetLeaderboardAsync<T>(Client client, string[] sortBy, int limit = 10, CancellationToken ct = default) where T : class, new()
            => client.Send<LeaderboardResponse<T>>(HttpMethod.Post, client.Url(Endpoints.Leaderboard), new LeaderboardRequest(sortBy, limit), ct);
    }
}
