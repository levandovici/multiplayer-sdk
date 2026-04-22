using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class ActionSubmitRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string Target_players { get; set; } = ERoomTargetPlayers.Host.ToString().ToLower();
        [JsonInclude]
        private int[]? Target_players_ids { get; set; }
        [JsonInclude]
        private string Action_type { get; set; } = string.Empty;
        [JsonInclude]
        private T? Request_data { get; set; }



        public ActionSubmitRequest(ERoomTargetPlayers targetPlayers, string type, T? data = null, int[]? targetPlayersIds = null)
        {
            this.Target_players = targetPlayers.ToString().ToLower();
            this.Target_players_ids = targetPlayersIds;
            this.Action_type = type;
            this.Request_data = data;
        }
    }
}
