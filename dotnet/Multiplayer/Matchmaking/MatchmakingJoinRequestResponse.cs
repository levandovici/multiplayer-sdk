using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingJoinRequestResponse : ApiResponse<EMatchmakingJoinError>
    {
        public string Request_id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
