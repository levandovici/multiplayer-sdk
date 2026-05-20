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
    /// Internal request data for banning a player from the game.
    /// </summary>
    internal class PlayerBanRequest
    {
        [JsonInclude]
        internal required int Player_id { get; set; }
        [JsonInclude]
        internal required string Ban_duration { get; set; }
        [JsonInclude]
        internal string? Ban_reason { get; set; }

        /// <summary>
        /// Initializes a new PlayerBanRequest.
        /// </summary>
        /// <param name="playerId">The ID of the player to ban.</param>
        /// <param name="banDuration">The duration of the ban.</param>
        /// <param name="banReason">Optional reason for the ban.</param>
        [SetsRequiredMembers]
        public PlayerBanRequest(int playerId, EBanTime banDuration, string? banReason = null)
        {
            this.Player_id = playerId;
            this.Ban_duration = banDuration.ToString().ToLower();
            this.Ban_reason = banReason;
        }
    }
}
