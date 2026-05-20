using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Internal request data for registering a new player in Unity.
    /// Uses serialized JSON string for player data.
    /// </summary>
    [System.Serializable]
    internal class PlayerRegisterRequest
    {
        /// <summary>
        /// The player's display name.
        /// </summary>
        public string player_name;

        /// <summary>
        /// Serialized JSON string of player data (Unity mode).
        /// </summary>
        public string player_data_json;

        /// <summary>
        /// Initializes a new PlayerRegisterRequest.
        /// </summary>
        /// <param name="playerName">The player's display name.</param>
        /// <param name="playerDataJson">Serialized JSON string of player data.</param>
        public PlayerRegisterRequest(string playerName, string playerDataJson)
        {
            this.player_name = playerName;
            this.player_data_json = playerDataJson;
        }
    }
}
