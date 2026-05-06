namespace Michitai.Multiplayer.Rooms.Realtime
{
    [System.Serializable]
    public struct SenderInfo
    {
        public bool is_host;
        public int game_player_id;
        public string player_name;
    }
}
