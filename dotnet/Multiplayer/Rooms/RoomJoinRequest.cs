using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class RoomJoinRequest<T> where T : class, new()
    {
        [JsonInclude]
        private string? Password { get; set; }
        [JsonInclude]
        private T? Player_data { get; set; }



        public RoomJoinRequest(string? password = null, T? playerData = null)
        {
            this.Password = password;
            this.Player_data = playerData;
        }
    }
}
