using Michitai.Multiplayer.Errors;
using System;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player is successfully unbanned.
    /// </summary>
    [Serializable]
    public class PlayerUnbanResponse : ApiResponse<ECommonError>
    {
        /// <summary>
        /// Confirmation message for the unban.
        /// </summary>
        public string message;

        /// <summary>
        /// The ID of the player who was unbanned.
        /// </summary>
        public int player_id;
    }
}
