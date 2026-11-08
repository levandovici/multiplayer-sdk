using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Regions
{
    /// <summary>
    /// Realtime (WS/UDP) endpoint advertised by a service node.
    /// Field names mirror the JSON wire format for JsonUtility.
    /// </summary>
    [System.Serializable]
    public class RealtimeEndpoint
    {
        /// <summary>Realtime WebSocket host.</summary>
        public string host = string.Empty;

        /// <summary>TLS port the wss:// proxy listens on.</summary>
        public int port = 443;

        /// <summary>WebSocket scheme — "wss" in production.</summary>
        public string protocol = "wss";

        /// <summary>UDP fast-lane port; 0 when the node runs TCP-only.</summary>
        public int udp_port;
    }

    /// <summary>
    /// One enabled API region from the discovery directory.
    /// </summary>
    [System.Serializable]
    public class ServiceNode
    {
        /// <summary>Short region code, e.g. "eu", "uk".</summary>
        public string region = string.Empty;

        /// <summary>Human label, e.g. "United Kingdom (London)".</summary>
        public string label = string.Empty;

        /// <summary>Full API base incl. /api — pass it as Client baseUrl.</summary>
        public string api_url = string.Empty;

        /// <summary>Realtime endpoint for this region; null when none is advertised.</summary>
        public RealtimeEndpoint? realtime;
    }
}
