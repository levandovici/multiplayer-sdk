using Michitai.Multiplayer.Errors;
using System;

namespace Michitai.Multiplayer.Players.Ban
{
    /// <summary>
    /// Response containing ban information for a player
    /// </summary>
    public class BanResponse : ApiResponse<ECommonError>
    {
        /// <summary>
        /// The unique ID of the ban
        /// </summary>
        public string Ban_id { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the banned player
        /// </summary>
        public int Player_id { get; set; }

        /// <summary>
        /// The duration of the ban (e.g., "1 hour", "1 day", "permanent")
        /// </summary>
        public string Ban_duration { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the ban expires (empty if permanent)
        /// </summary>
        public string Banned_until { get; set; } = string.Empty;

        /// <summary>
        /// The reason for the ban
        /// </summary>
        public string? Ban_reason { get; set; }

        /// <summary>
        /// Checks if this response indicates the player is banned
        /// </summary>
        public bool IsBanned => !Success && (Error?.Contains("You are banned") ?? false);
    }
}
