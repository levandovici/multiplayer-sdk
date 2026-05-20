using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Request data for listing matchmaking lobbies with optional filtering.
    /// </summary>
    public class MatchmakingListRequest
    {
        /// <summary>
        /// Optional search term to filter lobbies by name.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Optional maximum number of lobbies to return.
        /// </summary>
        public int? Limit { get; set; }
    }
}
