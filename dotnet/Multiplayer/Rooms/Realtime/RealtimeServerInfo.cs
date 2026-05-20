namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Information about the realtime WebSocket server.
    /// Contains connection details for establishing WebSocket connections.
    /// </summary>
    public class RealtimeServerInfo
    {
        /// <summary>
        /// The server hostname or IP address.
        /// </summary>
        public string host { get; set; }

        /// <summary>
        /// The server port number.
        /// </summary>
        public int port { get; set; }

        /// <summary>
        /// The WebSocket protocol (e.g., "ws" or "wss").
        /// </summary>
        public string protocol { get; set; }
    }
}
