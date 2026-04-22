using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class CurrentRoomResponse<T> : ApiResponse<ERoomCurrentError> where T : class, new()
    {
        public bool In_room { get; set; }
        public CurrentRoomInfo<T>? Room { get; set; }
        public List<object>? Pending_actions { get; set; }
        public List<object>? Pending_updates { get; set; }
    }
}
