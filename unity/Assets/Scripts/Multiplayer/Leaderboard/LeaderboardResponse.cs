using Michitai.Multiplayer;
using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    [System.Serializable]
    public class LeaderboardResponse<T> : ApiResponse<ELeaderboardError> where T : class, new()
    {
        public List<LeaderboardPlayer<T>> leaderboard = new();
        public int total;
        public string[] sort_by;
        public int limit;
    }
}
