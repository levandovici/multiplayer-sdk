using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Regions
{
    /// <summary>
    /// Realtime (WS/UDP) endpoint advertised by a service node.
    /// </summary>
    public class RealtimeEndpoint
    {
        /// <summary>Realtime WebSocket host.</summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>TLS port the wss:// proxy listens on.</summary>
        public int Port { get; set; } = 443;

        /// <summary>WebSocket scheme — "wss" in production.</summary>
        public string Protocol { get; set; } = "wss";

        /// <summary>UDP fast-lane port; null when the node runs TCP-only.</summary>
        public int? Udp_port { get; set; }
    }

    /// <summary>
    /// One enabled API region from the discovery directory.
    /// </summary>
    public class ServiceNode
    {
        /// <summary>Short region code, e.g. "eu", "uk".</summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>Human label, e.g. "United Kingdom (London)".</summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>Full API base incl. /api — pass it as Client baseUrl.</summary>
        public string Api_url { get; set; } = string.Empty;

        /// <summary>Realtime endpoint for this region; null when none is advertised.</summary>
        public RealtimeEndpoint? Realtime { get; set; }
    }
}
