using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Time
{
    /// <summary>
    /// Contains UTC offset information for time calculations in Unity.
    /// Stores offset details including hours, string representation, and original timestamps.
    /// Uses Unity's JsonUtility for serialization.
    /// </summary>
    [System.Serializable]
    public class TimeOffset
    {
        [SerializeField]
        private string original_utc;

        /// <summary>
        /// The UTC offset in hours (e.g., 3 for UTC+3, -5 for UTC-5).
        /// </summary>
        public int offset_hours;

        /// <summary>
        /// String representation of the UTC offset (e.g., "+03:00" or "-05:00").
        /// </summary>
        public string offset_string;

        /// <summary>
        /// Unix timestamp of the original UTC time.
        /// </summary>
        public long original_timestamp;

        /// <summary>
        /// The original UTC time before offset was applied (parsed from serialized string).
        /// </summary>
        public DateTimeOffset? OriginalUtc
        {
            get
            {
                return Time.ParseUtc(original_utc);
            }
        }
    }
}
