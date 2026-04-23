using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Rooms.Updates
{
    [System.Serializable]
    public class PlayerUpdate
    {
        [SerializeField]
        private string created_at;



        public string update_id;
        public int from_player_id;
        public string type;
        public string data_json;    // Unity mode



        public DateTimeOffset? CreatedAt
        {
            get
            {
                return ParseUtc(created_at);
            }
        }
    }
}
