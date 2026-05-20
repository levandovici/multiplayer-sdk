using Michitai.Multiplayer.Errors;
using System;
using UnityEngine;

namespace Michitai.Multiplayer.Players.Ban
{
    /// <summary>
    /// Response containing detailed ban information for a player in Unity.
    /// </summary>
    [System.Serializable]
    public class BanResponse : ApiResponse<ECommonError>
    {
        /// <summary>
        /// The unique ID of the ban record.
        /// </summary>
        public string ban_id = string.Empty;

        /// <summary>
        /// The ID of the banned player.
        /// </summary>
        public int player_id;

        /// <summary>
        /// The duration of the ban (e.g., "1 hour", "1 day", "permanent").
        /// </summary>
        public string ban_duration = string.Empty;

        /// <summary>
        /// The timestamp when the ban expires (empty if permanent).
        /// </summary>
        public string banned_until = string.Empty;

        /// <summary>
        /// The reason for the ban, if provided.
        /// </summary>
        public string ban_reason;

        /// <summary>
        /// Checks if this response indicates the player is banned.
        /// </summary>
        public bool IsBanned => !success && (!string.IsNullOrEmpty(error) && error.Contains("You are banned"));
    }
}
