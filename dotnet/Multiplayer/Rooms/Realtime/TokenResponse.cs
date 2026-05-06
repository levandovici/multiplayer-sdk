using Michitai.Multiplayer;

namespace Michitai.Multiplayer.Rooms.Realtime
{
    public class TokenResponse : ApiResponse
    {
        public string token { get; set; }
        public PlayerInfo player_info { get; set; }
        public RealtimeServerInfo realtime_server { get; set; }
    }
}
