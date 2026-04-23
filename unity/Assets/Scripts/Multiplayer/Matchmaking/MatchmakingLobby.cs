using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Matchmaking
{
    [System.Serializable]
    public class MatchmakingLobby<T> where T : class, new()
    {
        [SerializeField]
        private string created_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string rules_json;      // Unity mode



        public string matchmaking_id;
        public string matchmaking_name;
        public int host_player_id;
        public int max_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool host_switch;
        public bool can_leave_room;
        public int current_players;
        public string host_name;



        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }



        public DateTimeOffset? CreatedAt
        {
            get
            {
                return ParseUtc(created_at);
            }
        }

        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return ParseUtc(last_heartbeat);
            }
        }
    }
}
