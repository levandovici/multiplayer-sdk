using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    [System.Serializable]
    public class ActionSubmitRequest
    {
        public string target_players = ERoomTargetPlayers.All.ToString().ToLower();
        public int[] target_players_ids;
        public string action_type;
        public string request_data_json;    // Unity mode



        public ActionSubmitRequest(ERoomTargetPlayers targetPlayers, string type, string dataJson = null, int[] targetPlayerIds = null)
        {
            this.target_players = targetPlayers.ToString().ToLower();
            this.target_players_ids = targetPlayerIds;
            this.action_type = type;
            this.request_data_json = dataJson;
        }
    }
}
