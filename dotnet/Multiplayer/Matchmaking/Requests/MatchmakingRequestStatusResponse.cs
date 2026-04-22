using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    public class MatchmakingRequestStatusResponse : ApiResponse<EMatchmakingStatusError>
    {
        public MatchmakingRequestInfo Request { get; set; } = new();
    }
}
