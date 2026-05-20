using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Time
{
    /// <summary>
    /// Response containing the current server time in UTC.
    /// </summary>
    [System.Serializable]
    public class ServerTimeResponse : ApiResponse<ETimeError>
    {
        [SerializeField]
        private string utc;

        /// <summary>
        /// Unix timestamp of the current server time.
        /// </summary>
        public long timestamp;

        /// <summary>
        /// Human-readable format of the server time.
        /// </summary>
        public string readable;

        /// <summary>
        /// The current server time in UTC (parsed from string).
        /// </summary>
        public DateTimeOffset? Utc
        {
            get
            {
                return Time.ParseUtc(utc);
            }
        }
    }
}
