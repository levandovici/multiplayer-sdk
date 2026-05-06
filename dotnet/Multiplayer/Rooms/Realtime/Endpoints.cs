namespace Michitai.Multiplayer.Rooms.Realtime
{
    public static class Endpoints
    {
        public const string RealtimeToken = "https://api.michitai.com/api/realtime.php/token";
        public const string RealtimeValidate = "https://api.michitai.com/api/realtime.php/token/validate";
        public const string RealtimeRevoke = "https://api.michitai.com/api/realtime.php/token/revoke";
        public const string RealtimeWebSocket = "ws://realtime.michitai.com:8081";
    }
}
