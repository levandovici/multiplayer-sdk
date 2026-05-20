using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Response containing comprehensive information about the current game room.
    /// Includes room details, players, and pending actions/updates.
    /// </summary>
    /// <typeparam name="T">The type to deserialize room rules into.</typeparam>
    public class CurrentRoomResponse<T> : ApiResponse<ERoomCurrentError> where T : class, new()
    {
        /// <summary>
        /// Indicates whether the player is currently in a room.
        /// </summary>
        public bool In_room { get; set; }

        /// <summary>
        /// The room information if the player is in one.
        /// </summary>
        public CurrentRoomInfo<T>? Room { get; set; }

        /// <summary>
        /// List of pending actions waiting for completion.
        /// </summary>
        public List<object>? Pending_actions { get; set; }

        /// <summary>
        /// List of pending updates waiting to be polled.
        /// </summary>
        public List<object>? Pending_updates { get; set; }
    }
}
