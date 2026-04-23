using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    internal class RoomCreateRequest
    {
        public string room_name;
        public string password;
        public int max_players;
        public bool host_switch;
        public string player_data_json;     // Unity mode
        public string rules_json;           // Unity mode



        public RoomCreateRequest(string roomName, string password, int maxPlayers,
            bool hostSwitch, string playerData, string rulesJson)
        {
            this.room_name = roomName;
            this.password = password;
            this.max_players = maxPlayers;
            this.host_switch = hostSwitch;
            this.player_data_json = playerData;
            this.rules_json = rulesJson;
        }
    }
}
