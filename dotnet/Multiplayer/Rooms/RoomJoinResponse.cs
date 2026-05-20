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
    public class RoomJoinResponse : ApiResponse<ERoomJoinError>
    {
        /// <summary>
        /// The ID of the game room that was joined.
        /// </summary>
        public string Room_id { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation message for the join operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
