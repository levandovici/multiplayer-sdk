using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class PendingAction<T> where T : class, new()
    {
        public string Action_id { get; set; } = string.Empty;
        public int Player_id { get; set; }
        public int Target_id { get; set; }
        public string Action_type { get; set; } = string.Empty;
        public DateTimeOffset Created_at { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public bool Is_host { get; set; }
        public T? Request_data { get; set; }
    }
}
