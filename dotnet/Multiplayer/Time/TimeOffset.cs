using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    /// <summary>
    /// Contains UTC offset information for time calculations.
    /// Stores offset details including hours, string representation, and original timestamps.
    /// </summary>
    public class TimeOffset
    {
        /// <summary>
        /// The UTC offset in hours (e.g., 3 for UTC+3, -5 for UTC-5).
        /// </summary>
        public int Offset_hours { get; set; }

        /// <summary>
        /// String representation of the UTC offset (e.g., "+03:00" or "-05:00").
        /// </summary>
        public string Offset_string { get; set; } = string.Empty;

        /// <summary>
        /// The original UTC time before offset was applied.
        /// </summary>
        public DateTimeOffset Original_utc { get; set; }

        /// <summary>
        /// Unix timestamp of the original UTC time.
        /// </summary>
        public long Original_timestamp { get; set; }
    }
}
