using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response containing the list of players in a matchmaking lobby.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    public class MatchmakingPlayersResponse<T> : ApiResponse<EMatchmakingPlayersError> where T : class, new()
    {
        /// <summary>
        /// List of players currently in the matchmaking lobby.
        /// </summary>
        public List<MatchmakingPlayer<T>> Players { get; set; } = new();
    }
}
