using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingCreateResponse : ApiResponse<EMatchmakingCreateError>
    {
        public string Matchmaking_id { get; set; } = string.Empty;
        public string Matchmaking_name { get; set; } = string.Empty;
        public int Max_players { get; set; }
        public bool Strict_full { get; set; }
        public bool Join_by_requests { get; set; }
        public bool Host_switch { get; set; }
        public bool Can_leave_room { get; set; }
        public bool Is_host { get; set; }
    }
}
