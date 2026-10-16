using Michitai.Multiplayer;
using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Moderation
{
    /// <summary>
    /// Response from the text screening endpoint in Unity.
    /// </summary>
    [System.Serializable]
    public class CensorResponse : ApiResponse<ECensorError>
    {
        /// <summary>
        /// True when the text contains no listed word after normalization.
        /// </summary>
        public bool clean;

        /// <summary>
        /// The text with listed words masked and markup/control characters
        /// stripped — safe to display as-is.
        /// </summary>
        public string filtered;

        /// <summary>
        /// Seconds until the rate-limit bucket refills (only on a 429 reply).
        /// </summary>
        public int retry_after;
    }
}
