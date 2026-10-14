using Michitai.Multiplayer.Errors;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned by the tag lookup endpoint.
    /// </summary>
    public class FriendLookupResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// The player found by tag, when any.
        /// </summary>
        public FriendInfo? Player { get; set; }

        /// <summary>
        /// The local player's relationship to the found player:
        /// "none", "friend", "outgoing", "incoming" or "self".
        /// </summary>
        public string? Relationship { get; set; }
    }
}
