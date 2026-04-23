using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    [System.Serializable]
    public class MatchmakingStartResponse : ApiResponse<EMatchmakingStartError>
    {
        public string room_id;
        public string room_name;
        public int players_transferred;
        public string message;
    }
}
