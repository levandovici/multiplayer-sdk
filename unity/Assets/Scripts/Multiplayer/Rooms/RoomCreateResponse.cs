using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    public class RoomCreateResponse : ApiResponse<ERoomCreateError>
    {
        public string room_id;
        public string room_name;
        public bool is_host;
    }
}
