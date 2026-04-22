using Michitai.Multiplayer.Errors;
using Michitai.Multiplayer.Matchmaking.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingCurrentResponse<T> : ApiResponse<EMatchmakingCurrentError> where T : class, new()
    {
        public bool In_matchmaking { get; set; }
        public MatchmakingInfo<T>? Matchmaking { get; set; }
        public List<MatchmakingRequestBase> Pending_requests { get; set; } = new();
    }
}
