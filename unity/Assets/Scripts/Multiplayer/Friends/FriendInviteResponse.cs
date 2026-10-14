using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned when sending a lobby/room invitation to a friend.
    /// </summary>
    [System.Serializable]
    public class FriendInviteResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// The created (or reused pending) invite's ID.
        /// </summary>
        public string invite_id;

        /// <summary>
        /// Invite status — always "pending" on success.
        /// </summary>
        public string status;
    }
}
