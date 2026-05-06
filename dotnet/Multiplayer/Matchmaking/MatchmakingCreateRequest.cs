using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    public class MatchmakingCreateRequest<TPlayerData, TRules>
    where TPlayerData : class where TRules : class, new()
    {
        [JsonInclude]
        private string Matchmaking_name { get; set; } = string.Empty;
        [JsonInclude]
        private int Max_players { get; set; }
        [JsonInclude]
        private bool Strict_full { get; set; }
        [JsonInclude]
        private bool Join_by_requests { get; set; }
        [JsonInclude]
        private bool Host_switch { get; set; }
        [JsonInclude]
        private bool Can_leave_room { get; set; }
        [JsonInclude]
        private bool Realtime_room { get; set; }
        [JsonInclude]
        private TPlayerData? Player_data { get; set; }
        [JsonInclude]
        private TRules? Rules { get; set; }



        public MatchmakingCreateRequest(string matchmakingName, int maxPlayers, bool strictFull,
            bool joinByRequests = false, bool hostSwitch = false, bool canLeaveRoom = false, bool realtimeRoom = false,
             TPlayerData? playerData = null, TRules? rules = null)
        {
            this.Matchmaking_name = matchmakingName;
            this.Max_players = maxPlayers;
            this.Strict_full = strictFull;
            this.Join_by_requests = joinByRequests;
            this.Host_switch = hostSwitch;
            this.Can_leave_room = canLeaveRoom;
            this.Realtime_room = realtimeRoom;
            this.Player_data = playerData;
            this.Rules = rules;
        }
    }
}
