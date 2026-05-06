using Michitai.Multiplayer;

namespace Michitai.Multiplayer.Rooms.Realtime
{
    [System.Serializable]
    public class TokenResponse : ApiResponse
    {
        public string token;
        public PlayerInfo player_info;
        public RealtimeServerInfo realtime_server;
    }
}
