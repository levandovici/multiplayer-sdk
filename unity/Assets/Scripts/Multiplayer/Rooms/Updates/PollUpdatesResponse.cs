using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Response containing player updates that were targeted to the polling player.
    /// </summary>
    [System.Serializable]
    public class PollUpdatesResponse : ApiResponse<ERoomUpdatesPollError>
    {
        /// <summary>
        /// List of player updates received.
        /// </summary>
        public List<PlayerUpdate> updates = new();

        /// <summary>
        /// Timestamp of the last update.
        /// </summary>
        public string last_update;
    }
}
