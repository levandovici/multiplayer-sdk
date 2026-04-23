using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms
{
    [System.Serializable]
    public class RoomShort<T> where T : class, new()
    {
        [SerializeField]
        private string rules_json;   // Unity mode



        public string room_id;
        public string room_name;
        public int max_players;
        public int current_players;
        public bool has_password;
        public bool host_switch;
        public bool can_leave;



        public T Rules
        {
            get
            {
                return JsonUtility.FromJson<T>(rules_json);
            }
        }
    }
}
