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
    [System.Serializable]
    public class MatchmakingStartResponse : ApiResponse<EMatchmakingStartError>
    {
        /// <summary>
        /// The ID of the created game room.
        /// </summary>
        public string room_id;

        /// <summary>
        /// The name of the created game room.
        /// </summary>
        public string room_name;

        /// <summary>
        /// The number of players transferred from matchmaking to the room.
        /// </summary>
        public int players_transferred;

        /// <summary>
        /// Confirmation message for the game start.
        /// </summary>
        public string message;
    }
}
