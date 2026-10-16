using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Moderation
{
    /// <summary>
    /// Internal request data for screening a text through the server-side
    /// moderation filter in Unity.
    /// </summary>
    [System.Serializable]
    internal class CensorRequest
    {
        /// <summary>
        /// The text to screen (max 1000 characters).
        /// </summary>
        public string text;



        public CensorRequest(string text)
        {
            this.text = text;
        }
    }
}
