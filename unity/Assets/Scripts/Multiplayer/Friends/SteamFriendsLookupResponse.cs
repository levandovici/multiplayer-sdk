using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned by the Steam-friends lookup: the subset of the
    /// submitted SteamIDs that hold a game account, each annotated with the
    /// local player's relationship to them.
    /// </summary>
    [System.Serializable]
    public class SteamFriendsLookupResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// Matching players with their relationship to the local player.
        /// </summary>
        public List<FriendInfo> players;
    }
}
