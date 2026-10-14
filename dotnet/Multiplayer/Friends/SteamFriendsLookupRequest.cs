using System.Collections.Generic;

namespace Michitai.Multiplayer.Friends
{
    /// <summary>
    /// Request body for the Steam-friends lookup — SteamID64 strings to map
    /// onto game accounts (max 100).
    /// </summary>
    internal class SteamFriendsLookupRequest
    {
        public List<string>? Steam_ids { get; set; }

        public SteamFriendsLookupRequest(List<string> steamIds)
        {
            this.Steam_ids = steamIds;
        }
    }
}
