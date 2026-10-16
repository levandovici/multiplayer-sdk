using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Moderation
{
    /// <summary>
    /// Response from the text screening endpoint.
    /// </summary>
    public class CensorResponse : ApiResponse<ECensorError>
    {
        /// <summary>
        /// True when the text contains no listed word after normalization.
        /// </summary>
        public bool Clean { get; set; }

        /// <summary>
        /// The text with listed words masked and markup/control characters
        /// stripped — safe to display as-is.
        /// </summary>
        public string? Filtered { get; set; }

        /// <summary>
        /// Seconds until the rate-limit bucket refills (only on a 429 reply).
        /// </summary>
        [JsonPropertyName("retry_after")]
        public int RetryAfter { get; set; }
    }
}
