using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Detailed information about the current game room.
    /// Contains room configuration, player counts, status, and rules.
    /// </summary>
    /// <typeparam name="T">The type to deserialize room rules into.</typeparam>
    public class CurrentRoomInfo<T> where T : class, new()
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
        /// Whether the current player is the host.
        /// </summary>
        public bool Is_host { get; set; }

        /// <summary>
        /// Whether the room is currently online/active.
        /// </summary>
        public bool Is_online { get; set; }

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
        /// Whether the room is currently active.
        /// </summary>
        public bool Is_active { get; set; }

        /// <summary>
        /// The current player's name.
        /// </summary>
        public string Player_name { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the current player joined the room.
        /// </summary>
        public DateTimeOffset Joined_at { get; set; }

        /// <summary>
        /// Timestamp of the last heartbeat from the current player.
        /// </summary>
        public DateTimeOffset Last_heartbeat { get; set; }

        /// <summary>
        /// Timestamp when the room was created.
        /// </summary>
        public DateTimeOffset Room_created_at { get; set; }

        /// <summary>
        /// Timestamp of the last activity in the room.
        /// </summary>
        public DateTimeOffset Room_last_activity { get; set; }

        /// <summary>
        /// The room rules deserialized into the specified type.
        /// </summary>
        public T? Rules { get; set; }
    }
}
