using Michitai.Multiplayer.Errors;
using System;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player is successfully banned.
    /// Contains details about the ban including duration and expiration.
    /// </summary>
    [Serializable]
    public class PlayerBanResponse : ApiResponse<ECommonError>
    {
        /// <summary>
        /// Confirmation message for the ban.
        /// </summary>
        public string message;

        /// <summary>
        /// The unique ID of the ban record.
        /// </summary>
        public string ban_id;

        /// <summary>
        /// The ID of the banned player.
        /// </summary>
        public int player_id;

        /// <summary>
        /// The duration of the ban (e.g., "1 hour", "1 day", "permanent").
        /// </summary>
        public string ban_duration;

        /// <summary>
        /// The timestamp when the ban expires (empty if permanent).
        /// </summary>
        public string banned_until;
    }
}
