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
    public class MatchmakingJoinRequestResponse : ApiResponse<EMatchmakingJoinError>
    {
        /// <summary>
        /// The unique ID of the join request.
        /// </summary>
        public string Request_id { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation message for the join request.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
