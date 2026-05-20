using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Request data for registering a new player.
    /// Contains player name and optional player data.
    /// </summary>
    /// <typeparam name="T">The type of player data to include.</typeparam>
    internal class PlayerRegisterRequest<T> where T : class, new()
    {
        [JsonInclude]
        internal required string Player_name { get; set; }
        [JsonInclude]
        private T? Player_data { get; set; }

        /// <summary>
        /// Initializes a new PlayerRegisterRequest.
        /// </summary>
        /// <param name="playerName">The player's display name.</param>
        /// <param name="playerData">Optional player data to include.</param>
        [SetsRequiredMembers]
        public PlayerRegisterRequest(string playerName, T? playerData = null)
        {
            this.Player_name = playerName;
            this.Player_data = playerData;
        }
    }
}
