using Michitai.Multiplayer.Errors;
using System;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Response returned when a player is successfully kicked from a game room.
    /// </summary>
    [Serializable]
    public class RoomKickResponse : ApiResponse<ERoomKickError>
    {
        /// <summary>
        /// Confirmation message for the kick operation.
        /// </summary>
        public string message;

        /// <summary>
        /// The ID of the player who was kicked.
        /// </summary>
        public int kicked_player_id;
    }
}
