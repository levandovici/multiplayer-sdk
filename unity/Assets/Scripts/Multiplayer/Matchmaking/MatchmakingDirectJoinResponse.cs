using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    [System.Serializable]
    public class MatchmakingDirectJoinResponse : ApiResponse<EMatchmakingJoinError>
    {
        public string message;
        public string matchmaking_id;
    }
}
