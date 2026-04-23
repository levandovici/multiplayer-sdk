using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Games
{
    [System.Serializable]
    public class PlayerShort
    {
        [SerializeField]
        private string last_login;
        [SerializeField]
        private string created_at;



        public int id;
        public string player_name;
        public bool is_online;



        public DateTimeOffset? LastLogin
        {
            get
            {
                return ParseUtc(last_login);
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
