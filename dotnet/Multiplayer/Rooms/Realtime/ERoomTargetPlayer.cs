namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Specifies the target players for realtime communication.
    /// </summary>
    public enum ERoomTargetPlayer
    {
        /// <summary>Target all players in the room.</summary>
        All,
        /// <summary>Target only the room host.</summary>
        Host,
        /// <summary>Target all players except the sender.</summary>
        Others,
        /// <summary>Target specific player IDs.</summary>
        Specific
    }
}
