using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Information about a completed action in the room.
    /// Contains action details, status, and response data.
    /// </summary>
    /// <typeparam name="T">The type of response data.</typeparam>
    public class ActionInfo<T> where T : class, new()
    {
        [JsonInclude]
        private string Status { get; set; } = string.Empty;

        /// <summary>
        /// The unique ID of the action.
        /// </summary>
        public string Action_id { get; set; } = string.Empty;

        /// <summary>
        /// The type of action that was performed.
        /// </summary>
        public string Action_type { get; set; } = string.Empty;

        /// <summary>
        /// Whether the action was sent by the host.
        /// </summary>
        public bool Is_host { get; set; }

        /// <summary>
        /// The response data associated with the action.
        /// </summary>
        public T? Response_data { get; set; }

        /// <summary>
        /// The parsed status of the action.
        /// </summary>
        public ERoomActionStatus ActionStatus
        {
            get
            {
                switch (Status)
                {
                    case "pending":
                        return ERoomActionStatus.Pending;
                    case "processing":
                        return ERoomActionStatus.Processing;
                    case "completed":
                        return ERoomActionStatus.Completed;
                    case "failed":
                        return ERoomActionStatus.Failed;
                    case "read":
                        return ERoomActionStatus.Read;
                    default:
                        throw new ArgumentException($"Unknown action status: {Status}");
                }
            }
        }
    }
}
