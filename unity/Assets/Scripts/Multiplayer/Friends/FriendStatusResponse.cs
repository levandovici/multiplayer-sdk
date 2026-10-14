using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned by friend request/accept/decline/remove calls.
    /// </summary>
    [System.Serializable]
    public class FriendStatusResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// Resulting status: "pending", "accepted" or "declined".
        /// </summary>
        public string status;

        /// <summary>
        /// The other player's ID.
        /// </summary>
        public int player_id;
    }
}
