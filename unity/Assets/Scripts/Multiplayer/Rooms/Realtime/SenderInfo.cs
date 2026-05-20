namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Information about the sender of a realtime message.
    /// Contains minimal sender details for realtime communication.
    /// </summary>
    [System.Serializable]
    public struct SenderInfo
    {
        /// <summary>
        /// Whether the sender is the host.
        /// </summary>
        public bool is_host;

        /// <summary>
        /// The player ID of the sender.
        /// </summary>
        public int game_player_id;

        /// <summary>
        /// The name of the sender.
        /// </summary>
        public string player_name;
    }
}
