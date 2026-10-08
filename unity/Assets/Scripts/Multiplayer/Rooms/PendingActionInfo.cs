using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// A pending action queued for this player (the processing host), as
    /// returned by the room "current" endpoint.
    /// </summary>
    [System.Serializable]
    public class PendingActionInfo
    {
        /// <summary>
        /// The unique ID of the action.
        /// </summary>
        public string action_id;

        /// <summary>
        /// The type of action.
        /// </summary>
        public string action_type;

        /// <summary>
        /// The request payload as a JSON string (Unity mode).
        /// </summary>
        public string request_data_json;

        /// <summary>
        /// The action status (pending / processing).
        /// </summary>
        public string status;

        [SerializeField]
        private string created_at;

        [SerializeField]
        private string processed_at;

        /// <summary>
        /// Timestamp when the action was created.
        /// </summary>
        public DateTimeOffset? CreatedAt
        {
            get { return ParseUtc(created_at); }
        }

        /// <summary>
        /// Timestamp when the action was processed, if applicable.
        /// </summary>
        public DateTimeOffset? ProcessedAt
        {
            get { return ParseUtc(processed_at); }
        }
    }
}
