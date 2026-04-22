using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    public class ServerTimeResponse : ApiResponse<ETimeError>
    {
        public DateTimeOffset Utc { get; set; }
        public long Timestamp { get; set; }
        public string Readable { get; set; } = string.Empty;
    }
}
