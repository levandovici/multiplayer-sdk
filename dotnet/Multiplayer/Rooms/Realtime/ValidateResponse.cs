namespace Michitai.Multiplayer.Rooms.Realtime
{
    public class ValidateResponse
    {
        public bool success { get; set; }
        public bool valid { get; set; }
        public bool already_used { get; set; }
        public PlayerInfo player_info { get; set; }
    }
}
