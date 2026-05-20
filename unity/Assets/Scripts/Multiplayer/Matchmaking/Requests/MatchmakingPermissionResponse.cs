using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    /// <summary>
    /// Response returned when a host responds to a join request (approve/reject).
    /// </summary>
    [System.Serializable]
    public class MatchmakingPermissionResponse : ApiResponse<EMatchmakingResponseError>
    {
        /// <summary>
        /// Confirmation message for the response action.
        /// </summary>
        public string message;

        /// <summary>
        /// The ID of the join request that was responded to.
        /// </summary>
        public string request_id;

        /// <summary>
        /// The action taken (Approve or Reject).
        /// </summary>
        public string action;
    }
}
