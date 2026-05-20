using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player is successfully logged out.
    /// </summary>
    public class PlayerLogoutResponse : ApiResponse<EPlayerLogoutError>
    {
        /// <summary>
        /// Confirmation message for the logout.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the player was logged out.
        /// </summary>
        public DateTimeOffset? Last_logout { get; set; }
    }
}
