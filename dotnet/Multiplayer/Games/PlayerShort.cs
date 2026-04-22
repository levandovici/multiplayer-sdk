using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Games
{
    public class PlayerShort
    {
        public int Id { get; set; }
        public string Player_name { get; set; } = string.Empty;
        public bool Is_online { get; set; }
        public DateTimeOffset? Last_login { get; set; }
        public DateTimeOffset Created_at { get; set; }
    }
}
