using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Games
{
    [System.Serializable]
    public class GameDataResponse<T> : ApiResponse<EGameDataGameGetError> where T : class, new()
    {
        [SerializeField]
        private string data_json;   // Unity mode



        public string type;
        public int game_id;



        public T Data
        {
            get
            {
                return JsonUtility.FromJson<T>(data_json);
            }
        }
    }
}
