using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    [System.Serializable]
    public class MatchmakingCurrentResponse<T> : ApiResponse<EMatchmakingCurrentError> where T : class, new()
    {
        public bool in_matchmaking;
        public MatchmakingInfo<T> matchmaking;
        public List<MatchmakingRequestBase> pending_requests;
    }
}
