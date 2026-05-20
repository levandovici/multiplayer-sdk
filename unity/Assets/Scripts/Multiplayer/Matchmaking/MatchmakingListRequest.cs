using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Request data for listing matchmaking lobbies with optional filtering in Unity.
    /// </summary>
    public class MatchmakingListRequest
    {
        /// <summary>
        /// Search term to filter lobbies by name.
        /// </summary>
        public string search = "";

        /// <summary>
        /// Maximum number of lobbies to return.
        /// </summary>
        public int limit = 20;
    }
}
