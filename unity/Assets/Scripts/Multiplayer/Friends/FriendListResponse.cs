using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned by the friends list endpoint. Contains the local
    /// player's tag, accepted friends, and pending requests in both directions.
    /// </summary>
    [System.Serializable]
    public class FriendListResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// The local player's own public friend tag in display form ("#XXXXXXXX").
        /// </summary>
        public string player_tag;

        /// <summary>
        /// Accepted friends, online first.
        /// </summary>
        public List<FriendInfo> friends;

        /// <summary>
        /// Pending friend requests addressed to the local player.
        /// </summary>
        public List<FriendInfo> incoming;

        /// <summary>
        /// Pending friend requests the local player sent.
        /// </summary>
        public List<FriendInfo> outgoing;
    }
}
