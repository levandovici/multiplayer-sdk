using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingKickResponse : ApiResponse<EMatchmakingKickError>
    {
        public string Message { get; set; } = string.Empty;
        public int KickedPlayerId { get; set; }
    }
}
