using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    internal class UpdatePlayersRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string Target_players { get; set; } = ERoomTargetPlayers.All.ToString().ToLower();
        [JsonInclude]
        private int[]? Target_players_ids { get; set; }
        [JsonInclude]
        private string Type { get; set; } = string.Empty;
        [JsonInclude]
        private T? Data { get; set; } = new();



        public UpdatePlayersRequest(ERoomTargetPlayers targetPlayers, string type, T? data = null, int[]? targetPlayersIds = null)
        {
            Target_players = targetPlayers.ToString().ToLower();
            Target_players_ids = targetPlayersIds;
            Type = type;
            Data = data;
        }
    }
}
