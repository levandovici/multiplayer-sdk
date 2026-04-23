using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    [System.Serializable]
    public class MatchmakingCreateResponse : ApiResponse<EMatchmakingCreateError>
    {
        public string matchmaking_id;
        public string matchmaking_name;
        public int max_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool host_switch;
        public bool can_leave_room;
        public bool is_host;
    }
}
