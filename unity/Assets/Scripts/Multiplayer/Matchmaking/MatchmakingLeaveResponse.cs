using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response returned when a player successfully leaves a matchmaking lobby.
    /// </summary>
    [System.Serializable]
    public class MatchmakingLeaveResponse : ApiResponse<EMatchmakingLeaveError>
    {
        /// <summary>
        /// Confirmation message for leaving the lobby.
        /// </summary>
        public string message;
    }
}
