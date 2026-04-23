using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    public class RoomLeaveResponse : ApiResponse<ERoomLeaveError>
    {
        public string message;
    }
}
