using System;
using System.Text.Json.Serialization;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Public shape of a player in friends contexts: friend lists, pending
    /// requests, tag lookups and Steam-friend matches. Never contains
    /// player data or credentials.
    /// </summary>
    public class FriendInfo
    {
        /// <summary>
        /// The player's unique ID.
        /// </summary>
        public int Player_id { get; set; }

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string Player_name { get; set; } = string.Empty;

        /// <summary>
        /// The player's public friend tag in display form ("#XXXXXXXX"), or null.
        /// </summary>
        public string? Player_tag { get; set; }

        /// <summary>
        /// Whether the player is currently online.
        /// </summary>
        public bool Is_online { get; set; }

        /// <summary>
        /// The player's SteamID64 for Steam-linked accounts, otherwise null.
        /// </summary>
        public string? Steam_id { get; set; }

        /// <summary>
        /// Relationship to the local player where the endpoint provides one:
        /// "none", "friend", "outgoing", "incoming" or "self".
        /// </summary>
        public string? Relationship { get; set; }

        /// <summary>
        /// Timestamp when the friend request was sent (incoming/outgoing lists only).
        /// </summary>
        public DateTimeOffset? Requested_at { get; set; }
    }
}
