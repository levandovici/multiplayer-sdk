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
    public class GameDataResponse<T> : ApiResponse<EGameDataGameGetError> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Data { get; set; }



        public string Type { get; set; } = string.Empty;
        public int Game_id { get; set; }



        [JsonIgnore]
        public T GameData
        {
            get
            {
                return Data.Deserialize<T>(Multiplayer.JsonOptions)!;
            }
        }
    }
}
