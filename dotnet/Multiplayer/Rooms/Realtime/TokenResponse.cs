namespace Michitai.Multiplayer.Rooms.Realtime
{
    public class TokenResponse
    {
        public bool success { get; set; }
        public string token { get; set; }
        public PlayerInfo player_info { get; set; }
        public RealtimeServerInfo realtime_server { get; set; }
        public string error { get; set; }
    }
}
