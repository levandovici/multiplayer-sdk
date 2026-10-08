using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Request data for joining a matchmaking lobby (direct join or join
    /// request). Serializes to { password, player_data } — the shape the
    /// backend expects.
    /// </summary>
    /// <typeparam name="T">The type of optional player data to include.</typeparam>
    public class MatchmakingJoinRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string? Password { get; set; }
        [JsonInclude]
        private T? Player_data { get; set; }

        /// <summary>
        /// Initializes a new MatchmakingJoinRequest.
        /// </summary>
        /// <param name="password">Optional password for the lobby.</param>
        /// <param name="playerData">Optional player data to include.</param>
        public MatchmakingJoinRequest(string? password = null, T? playerData = null)
        {
            this.Password = password;
            this.Player_data = playerData;
        }
    }
}
