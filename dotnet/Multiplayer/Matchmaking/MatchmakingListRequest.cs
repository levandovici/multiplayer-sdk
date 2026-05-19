using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingListRequest
    {
        public string? Search { get; set; }
        public int? Limit { get; set; }
    }
}
