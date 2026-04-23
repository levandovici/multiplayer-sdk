using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    [System.Serializable]
    public class PlayerDataResponse<T> : ApiResponse<EGameDataPlayerGetError> where T : class, new()
    {
        [SerializeField]
        private string data_json;   // Unity mode



        public string type;
        public int player_id;
        public string player_name;



        public T Data
        {
            get
            {
                return JsonUtility.FromJson<T>(data_json);
            }
        }
    }
}
