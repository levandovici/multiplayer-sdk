using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    internal class RoomJoinRequest
    {
        public string password;
        public string player_data_json;    // Unity mode



        public RoomJoinRequest(string password, string playerData)
        {
            this.password = password;
            this.player_data_json = playerData;
        }
    }
}
