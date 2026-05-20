using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Response containing a player's data with typed deserialization support.
    /// Uses Unity's JsonUtility for deserialization.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    [System.Serializable]
    public class PlayerDataResponse<T> : ApiResponse<EGameDataPlayerGetError> where T : class, new()
    {
        [SerializeField]
        private string data_json;   // Unity mode

        /// <summary>
        /// The type of the player data (for identification purposes).
        /// </summary>
        public string type;

        /// <summary>
        /// The unique ID of the player.
        /// </summary>
        public int player_id;

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string player_name;

        /// <summary>
        /// The deserialized player data object using Unity's JsonUtility.
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
