using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player account deletion is scheduled.
    /// The account is frozen immediately and purged permanently after the
    /// grace window unless restored — see RestoreAccountAsync.
    /// </summary>
    [System.Serializable]
    public class PlayerDeleteResponse : ApiResponse<EPlayerLoginError>
    {
        /// <summary>
        /// Confirmation message from the server.
        /// </summary>
        public string message;

        /// <summary>
        /// The ID of the deleted player.
        /// </summary>
        public int player_id;
    }
}
