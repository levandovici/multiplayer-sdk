using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    public class LeaderboardResponse<T> : ApiResponse<ELeaderboardError> where T : class, new()
    {
        public List<LeaderboardPlayer<T>> Leaderboard { get; set; } = new();
        public int Total { get; set; }
        public string[] Sort_by { get; set; } = Array.Empty<string>();
        public int Limit { get; set; }
    }
}
