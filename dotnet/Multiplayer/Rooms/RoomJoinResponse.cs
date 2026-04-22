using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class RoomJoinResponse : ApiResponse<ERoomJoinError>
    {
        public string Room_id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
