using System.Collections.Generic;
using Michitai.Multiplayer.Errors;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Response returned by the Steam-friends lookup: the subset of the
    /// submitted SteamIDs that hold a game account, each annotated with the
    /// local player's relationship to them.
    /// </summary>
    public class SteamFriendsLookupResponse : ApiResponse<EFriendsError>
    {
        /// <summary>
        /// Matching players with their relationship to the local player.
        /// </summary>
        public List<FriendInfo>? Players { get; set; }
    }
}
