using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    [System.Serializable]
    public class MatchmakingJoinRequestResponse : ApiResponse<EMatchmakingJoinError>
    {
        public string request_id;
        public string message;
    }
}
