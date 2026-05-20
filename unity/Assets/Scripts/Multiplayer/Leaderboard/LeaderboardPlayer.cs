using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Leaderboard
{
    /// <summary>
    /// Information about a player on the leaderboard.
    /// Contains rank, player details, and custom data.
    /// Uses Unity's JsonUtility for deserialization of player data.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    [System.Serializable]
    public class LeaderboardPlayer<T> where T : class, new()
    {
        [SerializeField]
        private string player_data_json;     // Unity mode

        /// <summary>
        /// The player's rank on the leaderboard.
        /// </summary>
        public int rank;

        /// <summary>
        /// The player's unique ID.
        /// </summary>
        public int player_id;

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string player_name;

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
