using Michitai.Multiplayer.Errors;
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
    /// Response containing a player's data with typed deserialization support.
    /// </summary>
    /// <typeparam name="T">The type to deserialize player data into.</typeparam>
    public class PlayerDataResponse<T> : ApiResponse<EGameDataPlayerGetError> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Data { get; set; }

        /// <summary>
        /// The type of the player data (for identification purposes).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The unique ID of the player.
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
                return Data.Deserialize<T>(Client.JsonOptions)!;
            }
        }
    }
}
