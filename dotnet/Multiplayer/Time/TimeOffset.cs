using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    public class TimeOffset
    {
        public int Offset_hours { get; set; }
        public string Offset_string { get; set; } = string.Empty;
        public DateTimeOffset Original_utc { get; set; }
        public long Original_timestamp { get; set; }
    }
}
