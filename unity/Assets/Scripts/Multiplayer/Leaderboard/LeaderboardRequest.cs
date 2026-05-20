using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    /// <summary>
    /// Internal request data for querying the leaderboard with sorting and limit options in Unity.
    /// </summary>
    [System.Serializable]
    internal class LeaderboardRequest
    {
        /// <summary>
        /// Array of field names to sort by.
        /// </summary>
        public string[] sort_by;

        /// <summary>
        /// Maximum number of results to return.
        /// </summary>
        public int limit;



        public LeaderboardRequest(string[] sort_by, int limit)
        {
            this.sort_by = sort_by;
            this.limit = limit;
        }
    }
}
