using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned when looking up a player by friend tag.
    /// </summary>
    [System.Serializable]
    public class FriendLookupResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// The found player, or null when no account uses that tag.
        /// </summary>
        public FriendInfo player;

        /// <summary>
        /// The local player's relationship to the found player:
        /// "none", "self", "friend", "outgoing" or "incoming".
        /// </summary>
        public string relationship;
    }
}
