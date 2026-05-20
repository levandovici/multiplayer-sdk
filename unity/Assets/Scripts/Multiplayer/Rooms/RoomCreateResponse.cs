using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Response returned when a game room is successfully created.
    /// Contains the room ID and configuration details.
    /// </summary>
    [System.Serializable]
    public class RoomCreateResponse : ApiResponse<ERoomCreateError>
    {
        /// <summary>
        /// The unique ID of the created game room.
        /// </summary>
        public string room_id;

        /// <summary>
        /// The name of the game room.
        /// </summary>
        public string room_name;

        /// <summary>
        /// Whether the room supports realtime communication.
        /// </summary>
        public bool realtime;

        /// <summary>
        /// Whether the player who created the room is the host.
        /// </summary>
        public bool is_host;
    }
}
