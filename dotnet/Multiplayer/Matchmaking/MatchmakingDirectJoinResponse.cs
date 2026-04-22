using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingDirectJoinResponse : ApiResponse<EMatchmakingJoinError>
    {
        public string Message { get; set; } = string.Empty;
        public string Matchmaking_id { get; set; } = string.Empty;
    }
}
