using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    [System.Serializable]
    public class ServerTimeWithOffsetResponse : ServerTimeResponse
    {
        public TimeOffset offset;
    }
}
