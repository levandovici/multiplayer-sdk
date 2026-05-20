using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    /// <summary>
    /// Request data for responding to a matchmaking join request with approve/reject action.
    /// </summary>
    public class MatchmakingPermissionRequest
    {
        [JsonInclude]
        private string Action { get; set; } = EMatchmakingRequestAction.Approve.ToString().ToLower();

        /// <summary>
        /// Initializes a new MatchmakingPermissionRequest.
        /// </summary>
        /// <param name="action">The action to take (Approve or Reject).</param>
        public MatchmakingPermissionRequest(EMatchmakingRequestAction action)
        {
            Action = action.ToString().ToLower();
        }
    }
}
