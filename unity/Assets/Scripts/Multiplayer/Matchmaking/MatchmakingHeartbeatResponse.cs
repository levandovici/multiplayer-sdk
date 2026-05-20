using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response returned when a matchmaking heartbeat is successfully sent.
    /// Used to maintain the player's presence in the lobby.
    /// </summary>
    [System.Serializable]
    public class MatchmakingHeartbeatResponse : ApiResponse<EMatchmakingHeartbeatError>
    {
        /// <summary>
        /// Status message indicating the heartbeat was received.
        /// </summary>
        public string status;
    }
}
