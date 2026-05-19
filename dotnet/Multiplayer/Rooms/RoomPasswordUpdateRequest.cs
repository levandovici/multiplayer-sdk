using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class RoomPasswordUpdateRequest
    {
        [JsonInclude]
        public string? Password { get; set; }

        public RoomPasswordUpdateRequest(string? password = null)
        {
            this.Password = password;
        }
    }
}
