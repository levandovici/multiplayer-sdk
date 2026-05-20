using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Games
{
    /// <summary>
    /// Response containing game data with typed deserialization support in Unity.
    /// Uses Unity's JsonUtility for deserialization.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the game data into.</typeparam>
    [System.Serializable]
    public class GameDataResponse<T> : ApiResponse<EGameDataGameGetError> where T : class, new()
    {
        [SerializeField]
        private string data_json;

        /// <summary>
        /// The type identifier for the game data.
        /// </summary>
        public string type;

        /// <summary>
        /// The ID of the game.
        /// </summary>
        public int game_id;

        /// <summary>
        /// The deserialized game data object (parsed from JSON string).
        /// </summary>
        public T Data
        {
            get
            {
                return JsonUtility.FromJson<T>(data_json);
            }
        }
    }
}
