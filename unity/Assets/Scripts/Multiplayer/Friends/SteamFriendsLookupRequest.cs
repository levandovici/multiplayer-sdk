using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Internal request body mapping Steam friends to game accounts.
    /// </summary>
    [System.Serializable]
    internal class SteamFriendsLookupRequest
    {
        /// <summary>
        /// SteamID64 strings for the local user's Steam friends (max 100).
        /// </summary>
        public List<string> steam_ids;

        /// <summary>
        /// Initializes a new SteamFriendsLookupRequest.
        /// </summary>
        /// <param name="steamIds">SteamID64 strings to look up.</param>
        public SteamFriendsLookupRequest(List<string> steamIds)
        {
            this.steam_ids = steamIds;
        }
    }
}
