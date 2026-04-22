using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    public class MatchmakingPermissionResponse : ApiResponse<EMatchmakingResponseError>
    {
        public string Message { get; set; } = string.Empty;
        public string Request_id { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }
}
