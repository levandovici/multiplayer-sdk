using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Moderation
{
    /// <summary>
    /// Request data for screening a text through the server-side moderation filter.
    /// </summary>
    public class CensorRequest
    {
        [JsonInclude]
        private string Text { get; set; }

        /// <summary>
        /// Initializes a new CensorRequest.
        /// </summary>
        /// <param name="text">The text to screen (max 1000 characters).</param>
        public CensorRequest(string text)
        {
            this.Text = text;
        }
    }
}
