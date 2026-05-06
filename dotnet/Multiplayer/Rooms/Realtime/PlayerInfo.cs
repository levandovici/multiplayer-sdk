namespace Michitai.Multiplayer.Rooms.Realtime
{
    public class PlayerInfo
    {
        public int player_id { get; set; }
        public string player_name { get; set; }
        public string room_id { get; set; }
        public bool is_host { get; set; }
    }
}
