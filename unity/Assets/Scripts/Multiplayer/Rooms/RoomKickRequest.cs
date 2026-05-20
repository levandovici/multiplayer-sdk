using System;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Request data for kicking a player from a game room in Unity.
    /// </summary>
    [Serializable]
    public class RoomKickRequest
    {
        /// <summary>
        /// The ID of the player to kick.
        /// </summary>
        public int player_id;
    }
}
