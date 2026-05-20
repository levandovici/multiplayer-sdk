using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player is successfully banned.
    /// Contains details about the ban including duration and expiration.
    /// </summary>
    public class PlayerBanResponse : ApiResponse<ECommonError>
    {
        /// <summary>
        /// Confirmation message for the ban.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The unique ID of the ban record.
        /// </summary>
        public string Ban_id { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the banned player.
        /// </summary>
        public int Player_id { get; set; }

        /// <summary>
        /// The duration of the ban (e.g., "1 hour", "1 day", "permanent").
        /// </summary>
        public string Ban_duration { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the ban expires (empty if permanent).
        /// </summary>
        public string Banned_until { get; set; } = string.Empty;
    }
}
