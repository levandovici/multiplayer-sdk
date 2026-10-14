using Michitai.Multiplayer.Errors;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned when responding to a lobby/room invitation. On accept
    /// the player is joined server-side — the invite bypasses the lobby or
    /// room password — and the destination id is returned.
    /// </summary>
    public class FriendInviteAcceptResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// Resulting status: "accepted" or "declined".
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// The invite that was answered.
        /// </summary>
        public string? Invite_id { get; set; }

        /// <summary>
        /// The joined matchmaking lobby ID (matchmaking invites only).
        /// </summary>
        public string? Matchmaking_id { get; set; }

        /// <summary>
        /// The joined room ID (room invites only).
        /// </summary>
        public string? Room_id { get; set; }
    }
}
