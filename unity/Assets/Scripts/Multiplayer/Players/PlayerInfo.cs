using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Detailed information about a player including custom data.
    /// Uses Unity's JsonUtility for deserialization of player data.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    [System.Serializable]
    public class PlayerInfo<T> where T : class, new()
    {
        [SerializeField]
        private string player_data_json;    // Unity mode

        /// <summary>
        /// The unique player ID.
        /// </summary>
        public int id;

        /// <summary>
        /// The ID of the game the player belongs to.
        /// </summary>
        public int game_id;

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string player_name;

        /// <summary>
        /// Indicates whether the player is currently online.
        /// </summary>
        public bool is_online;

        /// <summary>
        /// Timestamp of the player's last login.
        /// </summary>
        public string last_login;

        /// <summary>
        /// Timestamp of the player's last logout.
        /// </summary>
        public string last_logout;

        /// <summary>
        /// Timestamp of the player's last heartbeat.
        /// </summary>
        public string last_heartbeat;

        /// <summary>
        /// Timestamp when the player account was created.
        /// </summary>
        public string created_at;

        /// <summary>
        /// The deserialized player data object.
        /// </summary>
        public T PlayerData
        {
            get
            {
                return JsonUtility.FromJson<T>(player_data_json);
            }
        }
    }
}
