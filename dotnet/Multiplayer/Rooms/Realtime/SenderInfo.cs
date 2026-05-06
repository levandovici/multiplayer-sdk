namespace Michitai.Multiplayer.Rooms.Realtime
{
    public struct SenderInfo
    {
        public bool is_host { get; set; }
        public int game_player_id { get; set; }
        public string player_name { get; set; }
    }
}
