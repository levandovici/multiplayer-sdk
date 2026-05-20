using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Games
{
    /// <summary>
    /// Minimal player information returned in player lists.
    /// Contains basic player details without full player data.
    /// </summary>
    public class PlayerShort
    {
        /// <summary>
        /// The unique player ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string Player_name { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the player is currently online.
        /// </summary>
        public bool Is_online { get; set; }

        /// <summary>
        /// Timestamp of the player's last login.
        /// </summary>
        public DateTimeOffset? Last_login { get; set; }

        /// <summary>
        /// Timestamp when the player account was created.
        /// </summary>
        public DateTimeOffset Created_at { get; set; }
    }
}
