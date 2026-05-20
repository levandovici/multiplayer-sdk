using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    /// <summary>
    /// Internal request data for responding to a matchmaking join request with approve/reject action in Unity.
    /// </summary>
    [System.Serializable]
    internal class MatchmakingPermissionRequest
    {
        /// <summary>
        /// The action to take (approve or reject).
        /// </summary>
        public string action = EMatchmakingRequestAction.Approve.ToString().ToLower();

        /// <summary>
        /// Initializes a new MatchmakingPermissionRequest.
        /// </summary>
        /// <param name="action">The action to take (approve or reject).</param>
        public MatchmakingPermissionRequest(string action)
        {
            this.action = action;
        }
    }
}
