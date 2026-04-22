using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    public class MatchmakingPermissionRequest
    {
        [JsonInclude]
        private string Action { get; set; } = EMatchmakingRequestAction.Approve.ToString().ToLower();



        public MatchmakingPermissionRequest(EMatchmakingRequestAction action)
        {
            Action = action.ToString().ToLower();
        }
    }
}
