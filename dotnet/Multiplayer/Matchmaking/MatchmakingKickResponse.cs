using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response returned when a player is successfully kicked from a matchmaking lobby.
    /// </summary>
    public class MatchmakingKickResponse : ApiResponse<EMatchmakingKickError>
    {
        /// <summary>
        /// Confirmation message for the kick operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the player who was kicked.
        /// </summary>
        public int KickedPlayerId { get; set; }
    }
}
