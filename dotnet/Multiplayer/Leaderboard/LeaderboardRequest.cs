using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    /// <summary>
    /// Request data for querying the leaderboard with sorting and limit options.
    /// </summary>
    public class LeaderboardRequest
    {
        [JsonInclude]
        private string[] Sort_by { get; set; } = Array.Empty<string>();
        [JsonInclude]
        private int Limit { get; set; }

        /// <summary>
        /// Initializes a new LeaderboardRequest.
        /// </summary>
        /// <param name="sortBy">Array of field names to sort by.</param>
        /// <param name="limit">Maximum number of results to return.</param>
        public LeaderboardRequest(string[] sortBy, int limit)
        {
            this.Sort_by = sortBy;
            this.Limit = limit;
        }
    }
}
