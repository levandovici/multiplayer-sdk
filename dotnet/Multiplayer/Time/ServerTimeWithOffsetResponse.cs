using Michitai.Multiplayer.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    public class ServerTimeWithOffsetResponse : ServerTimeResponse
    {
        public TimeOffset? Offset { get; set; }
    }
}
