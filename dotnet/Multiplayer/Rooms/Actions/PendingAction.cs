using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Information about a pending action that needs to be completed by the host.
    /// Contains action details, sender information, and request data.
    /// </summary>
    /// <typeparam name="T">The type of request data.</typeparam>
    public class PendingAction<T> where T : class, new()
    {
        /// <summary>
        /// The unique ID of the action.
        /// </summary>
        public string Action_id { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the player who submitted the action.
        /// </summary>
        public int Player_id { get; set; }

        /// <summary>
        /// The ID of the target player.
        /// </summary>
        public int Target_id { get; set; }

        /// <summary>
        /// The type of action.
        /// </summary>
        public string Action_type { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the action was created.
        /// </summary>
        public DateTimeOffset Created_at { get; set; }

        /// <summary>
        /// The name of the player who submitted the action.
        /// </summary>
        public string Player_name { get; set; } = string.Empty;

        /// <summary>
        /// Whether the action was submitted by the host.
        /// </summary>
        public bool Is_host { get; set; }

        /// <summary>
        /// The request data associated with the action.
        /// </summary>
        public T? Request_data { get; set; }
    }
}
