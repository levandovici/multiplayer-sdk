using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    /// <summary>
    /// Response returned when a player requests to join a matchmaking lobby.
    /// Contains the request ID for tracking.
    /// </summary>
    [System.Serializable]
    public class MatchmakingJoinRequestResponse : ApiResponse<EMatchmakingJoinError>
    {
        /// <summary>
        /// The unique ID of the join request.
        /// </summary>
        public string request_id;

        /// <summary>
        /// Confirmation message for the join request.
        /// </summary>
        public string message;
    }
}
