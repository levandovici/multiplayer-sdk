using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    /// <summary>
    /// Detailed information about a matchmaking join request.
    /// Extends MatchmakingRequestBase with response details.
    /// </summary>
    [System.Serializable]
    public class MatchmakingRequestInfo : MatchmakingRequestBase
    {
        /// <summary>
        /// The ID of the player who responded to the request.
        /// </summary>
        public int responded_by;

        /// <summary>
        /// The name of the player who responded to the request.
        /// </summary>
        public string responder_name;

        /// <summary>
        /// Whether the matchmaking lobby requires approval for joining.
        /// </summary>
        public bool join_by_requests;
    }
}
