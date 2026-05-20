using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Response returned when a room heartbeat is successfully sent.
    /// Used to maintain the player's presence in the room.
    /// </summary>
    public class HeartbeatResponse : ApiResponse<ERoomHeartbeatError>
    {
        /// <summary>
        /// Status message indicating the heartbeat was received.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
