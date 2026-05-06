namespace Michitai.Multiplayer.Rooms.Realtime
{
    [System.Serializable]
    public class RealtimeMessage
    {
        public string type;
        public string command;
        public string data_json;
        public SenderInfo sender;
    }

    [System.Serializable]
    public class SendMessage
    {
        public string type;
        public string command;
        public string data_json;
        public int[] target_ids;
        public string target;
    }
}
