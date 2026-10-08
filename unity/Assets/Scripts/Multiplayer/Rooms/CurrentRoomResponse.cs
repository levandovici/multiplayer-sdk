using Michitai.Multiplayer.Errors;
using Michitai.Multiplayer.Rooms.Updates;
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
    [System.Serializable]
    public class CurrentRoomResponse<T> : ApiResponse<ERoomCurrentError> where T : class, new()
    {
        /// <summary>
        /// Indicates whether the player is currently in a room.
        /// </summary>
        public bool in_room;

        /// <summary>
        /// The room information if the player is in one.
        /// </summary>
        public CurrentRoomInfo<T> room;

        /// <summary>
        /// Pending actions queued for this player (host processing queue).
        /// </summary>
        public List<PendingActionInfo> pending_actions;

        /// <summary>
        /// Pending updates addressed to this player.
        /// </summary>
        public List<PlayerUpdate> pending_updates;
    }
}
