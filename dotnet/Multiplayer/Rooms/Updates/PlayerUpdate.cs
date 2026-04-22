using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    public class PlayerUpdate<T> where T : class, new()
    {
        public string Update_id { get; set; } = string.Empty;
        public int From_player_id { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTimeOffset Created_at { get; set; }
        public T? Data { get; set; }
    }
}
