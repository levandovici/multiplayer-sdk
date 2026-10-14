using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned when responding to a lobby/room invitation. On accept
    /// the player is joined server-side — the invite bypasses the lobby or
    /// room password — and the destination id is returned.
    /// </summary>
    [System.Serializable]
    public class FriendInviteAcceptResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// Resulting status: "accepted" or "declined".
        /// </summary>
        public string status;

        /// <summary>
        /// The invite that was answered.
        /// </summary>
        public string invite_id;

        /// <summary>
        /// The joined matchmaking lobby ID (matchmaking invites only).
        /// </summary>
        public string matchmaking_id;

        /// <summary>
        /// The joined room ID (room invites only).
        /// </summary>
        public string room_id;
    }
}
