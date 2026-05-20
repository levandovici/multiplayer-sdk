using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response returned when a player successfully joins a matchmaking lobby directly.
    /// </summary>
    [System.Serializable]
    public class MatchmakingDirectJoinResponse : ApiResponse<EMatchmakingJoinError>
    {
        /// <summary>
        /// Confirmation message for the join operation.
        /// </summary>
        public string message;

        /// <summary>
        /// The ID of the matchmaking lobby that was joined.
        /// </summary>
        public string matchmaking_id;
    }
}
