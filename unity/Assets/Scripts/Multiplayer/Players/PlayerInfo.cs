using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    [System.Serializable]
    public class PlayerInfo<T> where T : class, new()
    {
        [SerializeField]
        private string player_data_json;    // Unity mode



        public int id;
        public int game_id;
        public string player_name;
        public bool is_online;
        public string last_login;
        public string last_logout;
        public string last_heartbeat;
        public string created_at;



        public T PlayerData
        {
            get
            {
                return JsonUtility.FromJson<T>(player_data_json);
            }
        }
    }
}
