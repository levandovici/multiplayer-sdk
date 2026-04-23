using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    [System.Serializable]
    internal class UpdatePlayersRequest
    {
        public string target_players = ERoomTargetPlayers.All.ToString().ToLower();
        public int[] target_players_ids;
        public string type;
        public string data_json;          // must be a JSON string for Unity



        public UpdatePlayersRequest(ERoomTargetPlayers targetPlayers, string type, string dataJson = null, int[] targetPlayerIds = null)
        {
            this.target_players = targetPlayers.ToString().ToLower();
            this.target_players_ids = targetPlayerIds;
            this.type = type;
            this.data_json = dataJson;
        }
    }
}
