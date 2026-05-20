using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a new player is successfully registered.
    /// Contains the player's ID, private key for authentication, and game information.
    /// </summary>
    public class PlayerRegisterResponse : ApiResponse<EPlayerRegisterError>
    {
        /// <summary>
        /// The unique ID assigned to the newly registered player.
        /// </summary>
        public int Player_id { get; set; }

        /// <summary>
        /// The private key token used for player authentication.
        /// Keep this secure as it's required for all player-specific operations.
        /// </summary>
        public string Private_key { get; set; } = string.Empty;

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string Player_name { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the game the player was registered to.
        /// </summary>
        public int Game_id { get; set; }
    }
}
