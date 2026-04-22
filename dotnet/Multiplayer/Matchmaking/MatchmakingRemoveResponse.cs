using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingRemoveResponse : ApiResponse<EMatchmakingRemoveError>
    {
        public string Message { get; set; } = string.Empty;
    }
}
