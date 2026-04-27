using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingRequestBase
    {
        public string Request_id { get; set; } = string.Empty;
        public string Matchmaking_id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset Requested_at { get; set; }
        public DateTimeOffset? Responded_at { get; set; }
    }
}
