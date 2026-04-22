using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class RoomPlayer<T> where T : class, new()
    {
        public int Player_id { get; set; }
        public bool Is_local { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public bool Is_host { get; set; }
        public bool Is_online { get; set; }
        public DateTimeOffset Last_heartbeat { get; set; }
        public T? Player_data { get; set; }
    }
}
