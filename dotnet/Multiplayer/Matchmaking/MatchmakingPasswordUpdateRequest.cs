using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Request data for updating a matchmaking lobby password.
    /// </summary>
    public class MatchmakingPasswordUpdateRequest
    {
        [JsonInclude]
        public string? Password { get; set; }

        /// <summary>
        /// Initializes a new MatchmakingPasswordUpdateRequest.
        /// </summary>
        /// <param name="password">The new password for the lobby, or null to remove password.</param>
        public MatchmakingPasswordUpdateRequest(string? password = null)
        {
            this.Password = password;
        }
    }
}
