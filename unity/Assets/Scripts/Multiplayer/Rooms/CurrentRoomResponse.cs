using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    public class CurrentRoomResponse<T> : ApiResponse<ERoomCurrentError> where T : class, new()
    {
        public bool in_room;
        public CurrentRoomInfo<T> room;
        public List<string> pending_actions_json;   // raw JSON strings
        public List<string> pending_updates_json;
    }
}
