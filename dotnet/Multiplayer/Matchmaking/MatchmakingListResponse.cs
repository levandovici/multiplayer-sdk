using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response containing a list of available matchmaking lobbies.
    /// </summary>
    /// <typeparam name="T">The type to deserialize lobby rules into.</typeparam>
    public class MatchmakingListResponse<T> : ApiResponse<EMatchmakingListError> where T : class, new()
    {
        /// <summary>
        /// List of available matchmaking lobbies with their details.
        /// </summary>
        public List<MatchmakingLobby<T>> Lobbies { get; set; } = new();
    }
}
