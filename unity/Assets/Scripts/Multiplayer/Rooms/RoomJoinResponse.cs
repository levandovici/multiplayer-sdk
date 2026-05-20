using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Response returned when a player successfully joins a game room.
    /// </summary>
    [System.Serializable]
    public class RoomJoinResponse : ApiResponse<ERoomJoinError>
    {
        /// <summary>
        /// The ID of the game room that was joined.
        /// </summary>
        public string room_id;

        /// <summary>
        /// Confirmation message for the join operation.
        /// </summary>
        public string message;
    }
}
