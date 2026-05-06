using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    public class LeaderboardPlayer<T> where T : class, new()
    {
        [JsonInclude]
        private JsonElement Player_data { get; set; }



        public int Rank { get; set; }
        public int Player_id { get; set; }
        public string Player_name { get; set; } = string.Empty;



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
