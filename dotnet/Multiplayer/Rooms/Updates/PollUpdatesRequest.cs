using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    internal class PollUpdatesRequest
    {
        [JsonInclude]
        private string From_players { get; set; } = ERoomTargetPlayers.Host.ToString().ToLower();
        [JsonInclude]
        private int[]? From_players_ids { get; set; }
        [JsonInclude]
        private string? Last_update { get; set; }


        public PollUpdatesRequest(ERoomTargetPlayers fromPlayers = ERoomTargetPlayers.Host, int[]? fromPlayersIds = null, string? lastUpdate = null)
        {
            From_players = fromPlayers.ToString().ToLower();
            From_players_ids = fromPlayersIds;
            Last_update = lastUpdate;
        }
    }
}
