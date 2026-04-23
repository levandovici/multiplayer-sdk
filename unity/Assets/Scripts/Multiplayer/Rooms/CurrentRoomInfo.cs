using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    public class CurrentRoomInfo<T> where T : class, new()
    {
        [SerializeField]
        private string joined_at;
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string room_created_at;
        [SerializeField]
        private string room_last_activity;
        [SerializeField]
        private string rules_json;          // Unity mode



        public string room_id;
        public string room_name;
        public bool is_host;
        public bool is_online;
        public int max_players;
        public int current_players;
        public bool has_password;
        public bool host_switch;
        public bool can_leave;
        public bool is_active;
        public string player_name;



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

        public DateTimeOffset? RoomCreatedAt
        {
            get
            {
                return ParseUtc(room_created_at);
            }
        }

        public DateTimeOffset? RoomLastActivity
        {
            get
            {
                return ParseUtc(room_last_activity);
            }
        }
    }
}
