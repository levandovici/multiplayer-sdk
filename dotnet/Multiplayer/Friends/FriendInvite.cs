using System;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// A pending lobby/room invitation addressed to the local player.
    /// </summary>
    public class FriendInvite
    {
        /// <summary>
        /// The invite's unique ID — pass to AcceptInviteAsync/DeclineInviteAsync.
        /// </summary>
        public string Invite_id { get; set; } = string.Empty;

        /// <summary>
        /// What the invite joins: "matchmaking" (a lobby) or "room" (a running game).
        /// </summary>
        public string Kind { get; set; } = string.Empty;

        /// <summary>
        /// Display name of the lobby or room.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The friend who sent the invite.
        /// </summary>
        public FriendInfo? From_player { get; set; }

        /// <summary>
        /// Timestamp when the invite was created.
        /// </summary>
        public DateTimeOffset? Created_at { get; set; }
    }
}
