using Michitai.Multiplayer.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Regions
{
    /// <summary>
    /// Enabled service nodes plus the server's geo suggestion.
    /// </summary>
    [System.Serializable]
    public class DiscoverResponse : ApiResponse<ECommonError>
    {
        /// <summary>Every enabled service node, ordered by server priority.</summary>
        public List<ServiceNode> nodes = new();

        /// <summary>Server-side geo hint for the calling client; null/empty when unknown.</summary>
        public string? suggested_region;
    }

    /// <summary>
    /// Result of discover.php?api_key=… — the node a game is pinned to.
    /// Field names mirror the JSON wire format for JsonUtility.
    /// </summary>
    [System.Serializable]
    public class GameNodeResponse : ApiResponse<ECommonError>
    {
        /// <summary>Region code the game is pinned to.</summary>
        public string? region;

        /// <summary>The pinned node (api_url + realtime endpoint); null when unavailable.</summary>
        public ServiceNode? node;
    }
}
