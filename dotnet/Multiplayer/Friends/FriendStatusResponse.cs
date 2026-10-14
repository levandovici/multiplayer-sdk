using Michitai.Multiplayer.Errors;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned by friend request/accept/decline/remove endpoints.
    /// </summary>
    public class FriendStatusResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// Resulting status — e.g. "pending", "accepted", "removed".
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// The other player's ID.
        /// </summary>
        public int Player_id { get; set; }
    }
}
