using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    public class LeaderboardRequest
    {
        [JsonInclude]
        private string[] Sort_by { get; set; } = Array.Empty<string>();
        [JsonInclude]
        private int Limit { get; set; }


        public LeaderboardRequest(string[] sortBy, int limit)
        {
            this.Sort_by = sortBy;
            this.Limit = limit;
        }
    }
}
