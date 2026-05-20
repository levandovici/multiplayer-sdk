using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    /// <summary>
    /// Base class for matchmaking join request information.
    /// Contains common properties for all request types including IDs, status, and timestamps.
    /// </summary>
    public class MatchmakingRequestBase
    {
        /// <summary>
        /// The unique ID of the join request.
        /// </summary>
        public string Request_id { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the matchmaking lobby the request is for.
        /// </summary>
        public string Matchmaking_id { get; set; } = string.Empty;

        /// <summary>
        /// The current status of the request (e.g., "Pending", "Approved", "Rejected").
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the request was made.
        /// </summary>
        public DateTimeOffset Requested_at { get; set; }

        /// <summary>
        /// Timestamp when the request was responded to (null if not yet responded).
        /// </summary>
        public DateTimeOffset? Responded_at { get; set; }
    }
}
