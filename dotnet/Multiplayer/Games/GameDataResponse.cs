using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Games
{
    /// <summary>
    /// Response containing global game data with typed deserialization support.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the game data into.</typeparam>
    public class GameDataResponse<T> : ApiResponse<EGameDataGameGetError> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Data { get; set; }

        /// <summary>
        /// The type of the game data (for identification purposes).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The unique game ID.
        /// </summary>
        public int Game_id { get; set; }

        /// <summary>
        /// The deserialized game data object.
        /// </summary>
        [JsonIgnore]
        public T GameData
        {
            get
            {
                return Data.Deserialize<T>(Client.JsonOptions)!;
            }
        }
    }
}
