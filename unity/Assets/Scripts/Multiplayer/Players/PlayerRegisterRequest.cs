using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    [System.Serializable]
    internal class PlayerRegisterRequest
    {
        public string player_name;
        public string player_data_json;



        public PlayerRegisterRequest(string playerName, string playerDataJson)
        {
            this.player_name = playerName;
            this.player_data_json = playerDataJson;
        }
    }
}
