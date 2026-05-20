using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Games
{
    /// <summary>
    /// Shortened player information for listing players in Unity.
    /// Contains essential player data with timestamp parsing support.
    /// </summary>
    [System.Serializable]
    public class PlayerShort
    {
        [SerializeField]
        private string last_login;
        [SerializeField]
        private string created_at;

        /// <summary>
        /// The unique ID of the player.
        /// </summary>
        public int id;

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string player_name;

        /// <summary>
        /// Whether the player is currently online.
        /// </summary>
        public bool is_online;

        /// <summary>
        /// Timestamp of the player's last login (parsed from string).
        /// </summary>
        public DateTimeOffset? LastLogin
        {
            get
            {
                return ParseUtc(last_login);
            }
        }

        /// <summary>
        /// Timestamp when the player account was created (parsed from string).
        /// </summary>
        public DateTimeOffset? CreatedAt
        {
            get
            {
                return ParseUtc(created_at);
            }
        }
    }
}
