using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Minimal information about a game room.
    /// Contains basic room details without full player information.
    /// </summary>
    /// <typeparam name="T">The type to deserialize room rules into.</typeparam>
    public class RoomShort<T> where T : class, new()
    {
        /// <summary>
        /// The unique ID of the room.
        /// </summary>
        public string Room_id { get; set; } = string.Empty;

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string Room_name { get; set; } = string.Empty;

        /// <summary>
        /// Maximum number of players allowed.
        /// </summary>
        public int Max_players { get; set; }

        /// <summary>
        /// Current number of players in the room.
        /// </summary>
        public int Current_players { get; set; }

        /// <summary>
        /// Whether the room has a password set.
        /// </summary>
        public bool Has_password { get; set; }

        /// <summary>
        /// Whether host switching is allowed.
        /// </summary>
        public bool Host_switch { get; set; }

        /// <summary>
        /// Whether players can leave the room.
        /// </summary>
        public bool Can_leave { get; set; }

        /// <summary>
        /// Whether the room supports realtime communication.
        /// </summary>
        public bool Realtime { get; set; }

        /// <summary>
        /// The room rules deserialized into the specified type.
        /// </summary>
        public T? Rules { get; set; }
    }
}
