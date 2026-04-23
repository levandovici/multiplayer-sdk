using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    [System.Serializable]
    public class MatchmakingPermissionResponse : ApiResponse<EMatchmakingResponseError>
    {
        public string message;
        public string request_id;
        public string action;
    }
}
