using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    /// <summary>
    /// Response containing the current server time in UTC.
    /// </summary>
    public class ServerTimeResponse : ApiResponse<ETimeError>
    {
        /// <summary>
        /// The current server time in UTC.
        /// </summary>
        public DateTimeOffset Utc { get; set; }

        /// <summary>
        /// Unix timestamp of the current server time.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Human-readable format of the server time.
        /// </summary>
        public string Readable { get; set; } = string.Empty;
    }
}
