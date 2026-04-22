using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Players
{
    public class PlayerInfo<T> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Player_data { get; set; }



        public int Id { get; set; }
        public int Game_id { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public bool Is_online { get; set; }
        public DateTimeOffset? Last_login { get; set; }
        public DateTimeOffset Created_at { get; set; }
        public DateTimeOffset Updated_at { get; set; }



        [JsonIgnore]
        public T PlayerData
        {
            get
            {
                return Player_data.Deserialize<T>(Multiplayer.JsonOptions)!;
            }
        }
    }
}
