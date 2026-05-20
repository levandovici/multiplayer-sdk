namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Information about the sender of a realtime message.
    /// Contains minimal sender details for realtime communication.
    /// </summary>
    public struct SenderInfo
    {
        /// <summary>
        /// Whether the sender is the host.
        /// </summary>
        public bool is_host { get; set; }

        /// <summary>
        /// The player ID of the sender.
        /// </summary>
        public int game_player_id { get; set; }

        /// <summary>
        /// The name of the sender.
        /// </summary>
        public string player_name { get; set; }
    }
}
