namespace Michitai.Multiplayer.Rooms.Realtime
{
    public class RealtimeMessage
    {
        public string? type { get; set; }
        public string? command { get; set; }
        public object? data { get; set; }
        public SenderInfo sender { get; set; }
        /// <summary>Sender-assigned sequence on unreliable (UDP) traffic —
        /// drop messages whose seq is not newer than the last seen.</summary>
        public uint seq { get; set; }
    }
}
