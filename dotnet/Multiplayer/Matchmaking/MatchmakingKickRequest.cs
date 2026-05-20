using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Request data for kicking a player from a matchmaking lobby.
    /// </summary>
    public class MatchmakingKickRequest
    {
        /// <summary>
        /// The ID of the player to kick.
        /// </summary>
        public int Player_id { get; set; }
    }
}
