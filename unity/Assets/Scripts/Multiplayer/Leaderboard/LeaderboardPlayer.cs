using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Leaderboard
{
    [System.Serializable]
    public class LeaderboardPlayer<T> where T : class, new()
    {
        [SerializeField]
        private string player_data_json;     // Unity mode



        public int rank;
        public int player_id;
        public string player_name;



        public T PlayerData
        {
            get
            {
                return JsonUtility.FromJson<T>(player_data_json);
            }
        }
    }
}
