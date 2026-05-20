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
    /// Internal request data for unbanning a player from the game.
    /// </summary>
    internal class PlayerUnbanRequest
    {
        [JsonInclude]
        internal required int Player_id { get; set; }

        /// <summary>
        /// Initializes a new PlayerUnbanRequest.
        /// </summary>
        /// <param name="playerId">The ID of the player to unban.</param>
        [SetsRequiredMembers]
        public PlayerUnbanRequest(int playerId)
        {
            this.Player_id = playerId;
        }
    }
}
