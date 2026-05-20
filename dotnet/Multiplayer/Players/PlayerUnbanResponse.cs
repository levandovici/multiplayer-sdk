using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player is successfully unbanned.
    /// </summary>
    public class PlayerUnbanResponse : ApiResponse<ECommonError>
    {
        /// <summary>
        /// Confirmation message for the unban.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the player who was unbanned.
        /// </summary>
        public int Player_id { get; set; }
    }
}
