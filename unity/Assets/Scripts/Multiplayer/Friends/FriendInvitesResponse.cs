using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned when listing pending lobby/room invitations.
    /// </summary>
    [System.Serializable]
    public class FriendInvitesResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// Pending invites addressed to the local player.
        /// </summary>
        public List<FriendInvite> invites;
    }
}
