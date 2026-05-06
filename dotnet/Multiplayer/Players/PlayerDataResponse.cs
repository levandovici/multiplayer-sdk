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
    public class PlayerDataResponse<T> : ApiResponse<EGameDataPlayerGetError> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Data { get; set; }



        public string Type { get; set; } = string.Empty;
        public int Player_id { get; set; }
        public string Player_name { get; set; } = string.Empty;



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
