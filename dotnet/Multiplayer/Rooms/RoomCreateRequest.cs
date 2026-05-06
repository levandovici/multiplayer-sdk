using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class RoomCreateRequest<TPlayerData, TRules>
    where TPlayerData : class, new() where TRules : class, new()
    {
        [JsonInclude]
        private string Room_name { get; set; } = string.Empty;
        [JsonInclude]
        private string? Password { get; set; }
        [JsonInclude]
        private int Max_players { get; set; }
        [JsonInclude]
        private bool Host_switch { get; set; }
        [JsonInclude]
        private bool Realtime { get; set; }
        [JsonInclude]
        private TPlayerData? Player_data { get; set; }
        [JsonInclude]
        private TRules? Rules { get; set; }



        public RoomCreateRequest(string room_name, int max_players, string? password = null,
            bool hostSwitch = false, bool realtime = false, TPlayerData? playerData = null, TRules? rules = null)
        {
            Room_name = room_name;
            Password = password;
            Max_players = max_players;
            Host_switch = hostSwitch;
            Realtime = realtime;
            Player_data = playerData;
            Rules = rules;
        }
    }
}
