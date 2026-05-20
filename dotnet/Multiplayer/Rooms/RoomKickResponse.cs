using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Response returned when a player is successfully kicked from a game room.
    /// </summary>
    public class RoomKickResponse : ApiResponse<ERoomKickError>
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
