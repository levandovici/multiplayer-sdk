using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    public class HeartbeatResponse : ApiResponse<ERoomHeartbeatError>
    {
        public string status;
    }
}
