using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Response returned when a player successfully leaves a game room.
    /// </summary>
    [System.Serializable]
    public class RoomLeaveResponse : ApiResponse<ERoomLeaveError>
    {
        /// <summary>
        /// Confirmation message for leaving the room.
        /// </summary>
        public string message;
    }
}
