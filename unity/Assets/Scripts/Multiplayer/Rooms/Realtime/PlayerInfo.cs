namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Player information for realtime WebSocket connections.
    /// Contains minimal player details for realtime communication.
    /// </summary>
    [System.Serializable]
    public class PlayerInfo
    {
        /// <summary>
        /// The unique player ID.
        /// </summary>
        public int player_id;

        /// <summary>
        /// The player's display name.
        /// </summary>
        public string player_name;

        /// <summary>
        /// The ID of the room the player is in.
        /// </summary>
        public string room_id;

        /// <summary>
        /// Whether the player is the host of the room.
        /// </summary>
        public bool is_host;
    }
}
