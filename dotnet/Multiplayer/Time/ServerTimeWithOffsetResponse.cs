using Michitai.Multiplayer.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    /// <summary>
    /// Response containing the server time with a specified UTC offset.
    /// Extends ServerTimeResponse with offset information.
    /// </summary>
    public class ServerTimeWithOffsetResponse : ServerTimeResponse
    {
        /// <summary>
        /// The UTC offset information applied to the time.
        /// </summary>
        public TimeOffset? Offset { get; set; }
    }
}
