using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player is successfully renamed.
    /// </summary>
    public class PlayerRenameResponse : ApiResponse<EPlayerRenameError>
    {
        /// <summary>
        /// Confirmation message for the name change.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The new name assigned to the player.
        /// </summary>
        public string New_name { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the player who was renamed.
        /// </summary>
        public int Player_id { get; set; }
    }
}
