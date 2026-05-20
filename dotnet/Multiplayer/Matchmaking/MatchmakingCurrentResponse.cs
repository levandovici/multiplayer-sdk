using Michitai.Multiplayer.Errors;
using Michitai.Multiplayer.Matchmaking.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Response containing the current matchmaking status for a player.
    /// Includes lobby information and pending join requests.
    /// </summary>
    /// <typeparam name="T">The type to deserialize lobby rules into.</typeparam>
    public class MatchmakingCurrentResponse<T> : ApiResponse<EMatchmakingCurrentError> where T : class, new()
    {
        /// <summary>
        /// Indicates whether the player is currently in a matchmaking lobby.
        /// </summary>
        public bool In_matchmaking { get; set; }

        /// <summary>
        /// The matchmaking lobby information if the player is in one.
        /// </summary>
        public MatchmakingInfo<T>? Matchmaking { get; set; }

        /// <summary>
        /// List of pending join requests (for host approval).
        /// </summary>
        public List<MatchmakingRequestBase> Pending_requests { get; set; } = new();
    }
}
