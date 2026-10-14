using System.Collections.Generic;
using Michitai.Multiplayer.Errors;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned by the pending-invites endpoint.
    /// </summary>
    public class FriendInvitesResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// Pending lobby/room invitations addressed to the local player.
        /// </summary>
        public List<FriendInvite>? Invites { get; set; }
    }
}
