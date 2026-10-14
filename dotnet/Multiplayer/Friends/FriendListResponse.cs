using System.Collections.Generic;
using Michitai.Multiplayer.Errors;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned by the friends list endpoint. Contains the local
    /// player's tag, accepted friends, and pending requests in both directions.
    /// </summary>
    public class FriendListResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// The local player's own public friend tag in display form ("#XXXXXXXX").
        /// </summary>
        public string? Player_tag { get; set; }

        /// <summary>
        /// Accepted friends, online first.
        /// </summary>
        public List<FriendInfo>? Friends { get; set; }

        /// <summary>
        /// Pending friend requests addressed to the local player.
        /// </summary>
        public List<FriendInfo>? Incoming { get; set; }

        /// <summary>
        /// Pending friend requests the local player sent.
        /// </summary>
        public List<FriendInfo>? Outgoing { get; set; }
    }
}
