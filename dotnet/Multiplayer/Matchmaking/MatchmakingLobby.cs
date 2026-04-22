using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingLobby<T> where T : class, new()
    {
        public string Matchmaking_id { get; set; } = string.Empty;
        public string Matchmaking_name { get; set; } = string.Empty;
        public int Host_player_id { get; set; }
        public int Max_players { get; set; }
        public bool Strict_full { get; set; }
        public bool Join_by_requests { get; set; }
        public bool Host_switch { get; set; }
        public bool Can_leave_room { get; set; }
        public DateTimeOffset Created_at { get; set; }
        public DateTimeOffset Last_heartbeat { get; set; }
        public int Current_players { get; set; }
        public string Host_name { get; set; } = string.Empty;
        public T? Rules { get; set; }
    }
}
