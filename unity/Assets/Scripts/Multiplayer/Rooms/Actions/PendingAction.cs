using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Rooms.Actions
{
    [System.Serializable]
    public class PendingAction<T> where T : class, new()
    {
        [SerializeField]
        private string request_data_json;    // Unity mode
        [SerializeField]
        private string created_at;


        public string action_id;
        public int player_id;
        public int target_id;
        public string action_type;
        public string player_name;
        public bool is_host;



        public T RequestData
        {
            get
            {
                return JsonUtility.FromJson<T>(request_data_json);
            }
        }

        public DateTimeOffset? CreatedAt
        {
            get
            {
                return ParseUtc(created_at);
            }
        }
    }
}
