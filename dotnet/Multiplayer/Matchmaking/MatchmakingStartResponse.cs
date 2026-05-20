using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response returned when a game is successfully started from matchmaking.
    /// Contains the created room ID and transfer information.
    /// </summary>
    public class MatchmakingStartResponse : ApiResponse<EMatchmakingStartError>
    {
        /// <summary>
        /// The ID of the created game room.
        /// </summary>
        public string Room_id { get; set; } = string.Empty;

        /// <summary>
        /// The name of the created game room.
        /// </summary>
        public string Room_name { get; set; } = string.Empty;

        /// <summary>
        /// The number of players transferred from matchmaking to the room.
        /// </summary>
        public int Players_transferred { get; set; }

        /// <summary>
        /// Confirmation message for the game start.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
