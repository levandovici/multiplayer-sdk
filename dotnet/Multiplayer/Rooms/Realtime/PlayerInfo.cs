namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Player information for realtime WebSocket connections.
    /// Contains minimal player details for realtime communication.
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// The unique player ID.
        /// </summary>
        public int player_id { get; set; }

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string player_name { get; set; }

        /// <summary>
        /// The ID of the room the player is in.
        /// </summary>
        public string room_id { get; set; }

        /// <summary>
        /// Whether the player is the host of the room.
        /// </summary>
        public bool is_host { get; set; }
    }
}
