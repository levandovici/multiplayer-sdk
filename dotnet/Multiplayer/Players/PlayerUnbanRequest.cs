using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    internal class PlayerUnbanRequest
    {
        [JsonInclude]
        internal required int Player_id { get; set; }

        [SetsRequiredMembers]
        public PlayerUnbanRequest(int playerId)
        {
            this.Player_id = playerId;
        }
    }
}
