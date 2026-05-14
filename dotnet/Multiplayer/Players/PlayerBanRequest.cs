using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    internal class PlayerBanRequest
    {
        [JsonInclude]
        internal required int Player_id { get; set; }
        [JsonInclude]
        internal required string Ban_duration { get; set; }
        [JsonInclude]
        internal string? Ban_reason { get; set; }

        [SetsRequiredMembers]
        public PlayerBanRequest(int playerId, EBanTime banDuration, string? banReason = null)
        {
            this.Player_id = playerId;
            this.Ban_duration = banDuration.ToString().ToLower();
            this.Ban_reason = banReason;
        }
    }
}
