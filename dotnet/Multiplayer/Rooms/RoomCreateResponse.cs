using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class RoomCreateResponse : ApiResponse<ERoomCreateError>
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public bool Realtime { get; set; }
        public bool Is_host { get; set; }
    }
}
