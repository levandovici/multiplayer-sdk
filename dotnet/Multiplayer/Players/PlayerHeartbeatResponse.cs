using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    public class PlayerHeartbeatResponse : ApiResponse<EPlayerHeartbeatError>
    {
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset Last_heartbeat { get; set; }
    }
}
