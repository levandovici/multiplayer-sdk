using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Internal request body for joining a matchmaking lobby (direct join or
    /// join request). Uses serialized JSON string for player data (Unity mode),
    /// matching the backend's expected { password, player_data_json } shape.
    /// </summary>
    [System.Serializable]
    internal class MatchmakingJoinRequest
    {
        /// <summary>
        /// The lobby password (only required for password-protected lobbies).
        /// </summary>
        public string password;

        /// <summary>
        /// Serialized JSON string of player data (Unity mode).
        /// </summary>
        public string player_data_json;

        /// <summary>
        /// Initializes a new MatchmakingJoinRequest.
        /// </summary>
        /// <param name="password">The lobby password.</param>
        /// <param name="playerData">Serialized JSON string of player data.</param>
        public MatchmakingJoinRequest(string password, string playerData)
        {
            this.password = password;
            this.player_data_json = playerData;
        }
    }
}
