using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Detailed information about a player including custom data.
    /// Uses System.Text.Json for deserialization of player data.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    public class PlayerInfo<T> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Player_data { get; set; }

        /// <summary>
        /// The unique player ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the game the player belongs to.
        /// </summary>
        public int Game_id { get; set; }

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string Player_name { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the player is currently online.
        /// </summary>
        public bool Is_online { get; set; }

        /// <summary>
        /// Timestamp of the player's last login.
        /// </summary>
        public DateTimeOffset? Last_login { get; set; }

        /// <summary>
        /// Timestamp when the player account was created.
        /// </summary>
        public DateTimeOffset Created_at { get; set; }

        /// <summary>
        /// Timestamp when the player account was last updated.
        /// </summary>
        public DateTimeOffset Updated_at { get; set; }

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
