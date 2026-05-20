using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    /// <summary>
    /// Information about a player on the leaderboard.
    /// Contains rank, player details, and custom data.
    /// Uses System.Text.Json for deserialization of player data.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    public class LeaderboardPlayer<T> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Player_data { get; set; }

        /// <summary>
        /// The player's rank on the leaderboard.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// The player's unique ID.
        /// </summary>
        public int Player_id { get; set; }

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string Player_name { get; set; } = string.Empty;

        /// <summary>
        /// The deserialized player data object.
        /// </summary>
        [JsonIgnore]
        public T PlayerData
        {
            get
            {
                return Player_data.Deserialize<T>(Client.JsonOptions)!;
            }
        }
    }
}
