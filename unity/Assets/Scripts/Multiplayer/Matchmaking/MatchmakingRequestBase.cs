using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Base class for matchmaking join request information.
    /// Contains common properties for all request types.
    /// Uses Unity's JsonUtility for timestamp parsing.
    /// </summary>
    [System.Serializable]
    public class MatchmakingRequestBase
    {
        [SerializeField]
        private string requested_at;
        [SerializeField]
        private string responded_at;

        /// <summary>
        /// The unique ID of the join request.
        /// </summary>
        public string request_id;

        /// <summary>
        /// The ID of the matchmaking lobby the request is for.
        /// </summary>
        public string matchmaking_id;

        /// <summary>
        /// The current status of the request (e.g., "Pending", "Approved", "Rejected").
        /// </summary>
        public string status;

        /// <summary>
        /// Timestamp when the request was made.
        /// </summary>
        public DateTimeOffset? RequestedAt
        {
            get
            {
                return ParseUtc(requested_at);
            }
        }

        /// <summary>
        /// Timestamp when the request was responded to.
        /// </summary>
        public DateTimeOffset? RespondedAt
        {
            get
            {
                return ParseUtc(responded_at);
            }
        }
    }
}
