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
    public class RoomPlayer<T> where T : class, new()
    {
        [SerializeField]
        private string last_heartbeat;
        [SerializeField]
        private string player_data_json;    //Unity mode



        public int player_id;
        public bool is_local;
        public string player_name;
        public bool is_host;
        public bool is_online;



        public T PlayerData
        {
            get
            {
                return JsonUtility.FromJson<T>(player_data_json);
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
