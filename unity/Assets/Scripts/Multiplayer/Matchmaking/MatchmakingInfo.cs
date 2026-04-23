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
    public class MatchmakingInfo<T> where T : class, new()
    {
        [SerializeField]
        private string rules_json;           // Unity mode
        [SerializeField]
        private string joined_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string lobby_heartbeat;
        [SerializeField]
        private string started_at;



        public string matchmaking_id;
        public string matchmaking_name;
        public bool is_host;
        public int max_players;
        public int current_players;
        public bool strict_full;
        public bool join_by_requests;
        public bool host_switch;
        public bool can_leave_room;
        public bool is_online;
        public bool is_started;



        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }

        public DateTimeOffset? JoinedAt
        {
            get
            {
                return ParseUtc(joined_at);
            }
        }

        public DateTimeOffset? LastHeartbeat
        {
            get
            {
                return ParseUtc(last_heartbeat);
            }
        }

        public DateTimeOffset? LobbyHeartbeat
        {
            get
            {
                return ParseUtc(lobby_heartbeat);
            }
        }

        public DateTimeOffset? StartedAt
        {
            get
            {
                return ParseUtc(started_at);
            }
        }
    }
}
