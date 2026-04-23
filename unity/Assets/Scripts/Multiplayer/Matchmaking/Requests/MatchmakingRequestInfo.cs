using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    [System.Serializable]
    public class MatchmakingRequestInfo : MatchmakingRequestBase
    {
        public int responded_by;
        public string responder_name;
        public bool join_by_requests;
    }
}
