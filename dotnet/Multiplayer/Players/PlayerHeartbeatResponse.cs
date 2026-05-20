using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player heartbeat is successfully sent.
    /// Used to maintain the player's online status.
    /// </summary>
    public class PlayerHeartbeatResponse : ApiResponse<EPlayerHeartbeatError>
    {
        /// <summary>
        /// Confirmation message for the heartbeat.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of the last heartbeat received.
        /// </summary>
        public DateTimeOffset Last_heartbeat { get; set; }
    }
}
