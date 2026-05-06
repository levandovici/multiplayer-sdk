using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class RoomShort<T> where T : class, new()
    {
        public string Room_id { get; set; } = string.Empty;
        public string Room_name { get; set; } = string.Empty;
        public int Max_players { get; set; }
        public int Current_players { get; set; }
        public bool Has_password { get; set; }
        public bool Host_switch { get; set; }
        public bool Can_leave { get; set; }
        public bool Realtime { get; set; }
        public T? Rules { get; set; }
    }
}
