namespace Michitai.Multiplayer.Rooms.Realtime
{
    [System.Serializable]
    public class PlayerInfo
    {
        public int player_id;
        public string player_name;
        public string room_id;
        public bool is_host;
    }
}
