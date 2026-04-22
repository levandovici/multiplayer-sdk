using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class RoomPlayersResponse<T> : ApiResponse<ERoomPlayersError> where T : class, new()
    {
        public List<RoomPlayer<T>> Players { get; set; } = new();
        public string Last_updated { get; set; } = string.Empty;
    }
}
