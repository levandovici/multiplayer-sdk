using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response returned when a matchmaking lobby is successfully removed.
    /// Can only be called by the host before the game starts.
    /// </summary>
    [System.Serializable]
    public class MatchmakingRemoveResponse : ApiResponse<EMatchmakingRemoveError>
    {
        /// <summary>
        /// Confirmation message for the lobby removal.
        /// </summary>
        public string message;
    }
}
