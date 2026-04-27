using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingRequestInfo : MatchmakingRequestBase
    {

        public int? Responded_by { get; set; }
        public string? Responder_name { get; set; }
        public bool Join_by_requests { get; set; }
    }
}
