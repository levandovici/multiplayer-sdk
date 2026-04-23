using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    public class RoomPlayersResponse<T> : ApiResponse<ERoomPlayersError> where T : class, new()
    {
        public List<RoomPlayer<T>> players = new();
        public string last_updated;
    }
}
