using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Michitai.Multiplayer.Time.Time;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Public shape of a player in friends contexts: friend lists, pending
    /// requests, tag lookups and Steam-friend matches. Never contains
    /// player_data or credentials.
    /// </summary>
    [System.Serializable]
    public class FriendInfo
    {
        [SerializeField]
        private string requested_at;

        /// <summary>
        /// The player's unique ID.
        /// </summary>
        public int player_id;

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string player_name;

        /// <summary>
        /// The player's public friend tag in display form ("#XXXXXXXX"), or null.
        /// </summary>
        public string player_tag;

        /// <summary>
        /// Whether the player is currently online.
        /// </summary>
        public bool is_online;

        /// <summary>
        /// The player's SteamID64 for Steam-linked accounts, otherwise null.
        /// </summary>
        public string steam_id;

        /// <summary>
        /// Relationship to the local player where the endpoint provides one:
        /// "none", "friend", "outgoing", "incoming" or "self".
        /// </summary>
        public string relationship;

        /// <summary>
        /// Timestamp when the friend request was sent (incoming/outgoing lists only).
        /// </summary>
        public DateTimeOffset? RequestedAt
        {
            get
            {
                return ParseUtc(requested_at);
            }
        }
    }
}
