using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Detailed information about a matchmaking join request.
    /// Extends MatchmakingRequestBase with response details.
    /// </summary>
    public class MatchmakingRequestInfo : MatchmakingRequestBase
    {
        /// <summary>
        /// The ID of the player who responded to the request (null if not yet responded).
        /// </summary>
        public int? Responded_by { get; set; }

        /// <summary>
        /// The name of the player who responded to the request (null if not yet responded).
        /// </summary>
        public string? Responder_name { get; set; }

        /// <summary>
        /// Whether the matchmaking lobby requires approval for joining.
        /// </summary>
        public bool Join_by_requests { get; set; }
    }
}
