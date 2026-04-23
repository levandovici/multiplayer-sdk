using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    [System.Serializable]
    internal class MatchmakingPermissionRequest
    {
        public string action = EMatchmakingRequestAction.Approve.ToString().ToLower();



        public MatchmakingPermissionRequest(string action)
        {
            this.action = action;
        }
    }
}
