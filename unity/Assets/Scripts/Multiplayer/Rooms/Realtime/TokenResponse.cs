using Michitai.Multiplayer;

namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Response containing the WebSocket token for realtime communication.
    /// Includes player and server connection information.
    /// </summary>
    [System.Serializable]
    public class TokenResponse : ApiResponse
    {
        /// <summary>
        /// The WebSocket authentication token.
        /// </summary>
        public string token;

        /// <summary>
        /// Information about the authenticated player.
        /// </summary>
        public PlayerInfo player_info;

        /// <summary>
        /// Information about the realtime WebSocket server.
        /// </summary>
        public RealtimeServerInfo realtime_server;
    }
}
