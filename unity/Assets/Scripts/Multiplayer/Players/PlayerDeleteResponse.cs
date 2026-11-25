using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response returned when a player account is successfully deleted.
    /// Deletion is irreversible: the token is dead and every linked record
    /// (friends, room membership, matchmaking, stats) is gone with it.
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
