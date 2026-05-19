using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    [System.Serializable]
    internal class MatchmakingCreateRequest
    {
        public string matchmaking_name;
        public int max_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool host_switch;
        public bool can_leave_room;
        public bool realtime_room;
        public string password;
        public string player_data_json;     //Unity mode
        public string rules_json;           //Unity mode



        public MatchmakingCreateRequest(string matchmakingName, int maxPlayers, bool strictFull,
            bool joinByRequests, bool hostSwitch, bool canLeaveRoom, bool realtimeRoom, string password, string playerData, string rulesJson)
        {
            this.matchmaking_name = matchmakingName;
            this.max_players = maxPlayers;
            this.strict_full = strictFull;
            this.join_by_requests = joinByRequests;
            this.host_switch = hostSwitch;
            this.can_leave_room = canLeaveRoom;
            this.realtime_room = realtimeRoom;
            this.password = password;
            this.player_data_json = playerData;
            this.rules_json = rulesJson;
        }
    }
}
