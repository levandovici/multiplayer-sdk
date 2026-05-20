using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Information about a player in a game room.
    /// Contains player details and connection status.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    public class RoomPlayer<T> where T : class, new()
    {
        /// <summary>
        /// The player's unique ID.
        /// </summary>
        public int Player_id { get; set; }

        /// <summary>
        /// Whether this player is the local player.
        /// </summary>
        public bool Is_local { get; set; }

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string Player_name { get; set; } = string.Empty;

        /// <summary>
        /// Whether the player is the room host.
        /// </summary>
        public bool Is_host { get; set; }

        /// <summary>
        /// Whether the player is currently online.
        /// </summary>
        public bool Is_online { get; set; }

        /// <summary>
        /// Timestamp of the player's last heartbeat.
        /// </summary>
        public DateTimeOffset Last_heartbeat { get; set; }

        /// <summary>
        /// The player's custom data deserialized into the specified type.
        /// </summary>
        public T? Player_data { get; set; }
    }
}
