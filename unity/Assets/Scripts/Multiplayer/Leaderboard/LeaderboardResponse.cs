using Michitai.Multiplayer;
using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    /// <summary>
    /// Response containing the leaderboard entries with rankings.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    [System.Serializable]
    public class LeaderboardResponse<T> : ApiResponse<ELeaderboardError> where T : class, new()
    {
        /// <summary>
        /// List of leaderboard entries with player rankings.
        /// </summary>
        public List<LeaderboardPlayer<T>> leaderboard = new();

        /// <summary>
        /// Total number of entries in the leaderboard.
        /// </summary>
        public int total;

        /// <summary>
        /// Array of field names used for sorting.
        /// </summary>
        public string[] sort_by;

        /// <summary>
        /// The limit applied to the number of results.
        /// </summary>
        public int limit;
    }
}
