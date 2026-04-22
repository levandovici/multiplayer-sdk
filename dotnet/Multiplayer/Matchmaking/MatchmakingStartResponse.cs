using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingStartResponse : ApiResponse<EMatchmakingStartError>
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public int Players_transferred { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
