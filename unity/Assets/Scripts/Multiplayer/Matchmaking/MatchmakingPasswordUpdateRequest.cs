using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Internal request data for updating a matchmaking lobby password in Unity.
    /// </summary>
    [System.Serializable]
    internal class MatchmakingPasswordUpdateRequest
    {
        /// <summary>
        /// The new password for the lobby.
        /// </summary>
        public string password;

        /// <summary>
        /// Initializes a new MatchmakingPasswordUpdateRequest.
        /// </summary>
        /// <param name="password">The new password for the lobby.</param>
        public MatchmakingPasswordUpdateRequest(string password)
        {
            this.password = password;
        }
    }
}
