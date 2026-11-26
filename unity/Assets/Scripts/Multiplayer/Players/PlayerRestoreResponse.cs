using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a pending account deletion is cancelled inside
    /// the grace window. The account is fully active again; the token was
    /// never invalidated.
    /// </summary>
    [System.Serializable]
    public class PlayerRestoreResponse : ApiResponse<EPlayerLoginError>
    {
        /// <summary>
        /// Confirmation message from the server.
        /// </summary>
        public string message;

        /// <summary>
        /// The ID of the restored player.
        /// </summary>
        public int player_id;
    }
}
