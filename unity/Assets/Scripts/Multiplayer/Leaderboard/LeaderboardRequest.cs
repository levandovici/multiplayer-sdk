using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    [System.Serializable]
    internal class LeaderboardRequest
    {
        public string[] sort_by;
        public int limit;
    }
}
