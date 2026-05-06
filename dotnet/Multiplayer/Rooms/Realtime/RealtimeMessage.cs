namespace Michitai.Multiplayer.Rooms.Realtime
{
    public class RealtimeMessage
    {
        public string? type { get; set; }
        public string? command { get; set; }
        public object? data { get; set; }
        public SenderInfo sender { get; set; }
    }
}
