using Michitai.Multiplayer.Errors;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned by the invite endpoint.
    /// </summary>
    public class FriendInviteResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// The created invite's ID.
        /// </summary>
        public string? Invite_id { get; set; }

        /// <summary>
        /// Resulting status — "sent" on success.
        /// </summary>
        public string? Status { get; set; }
    }
}
