namespace Michitai.Multiplayer.Rooms.Realtime
{
    [System.Serializable]
    public class RealtimeMessage
    {
        public string type;
        public string command;
        public string data_json;
        public SenderInfo sender;
        // Sender-assigned sequence on unreliable (UDP) traffic — drop
        // messages whose seq is not newer than the last seen.
        public int seq;
    }

    [System.Serializable]
    public class SendMessage
    {
        public string type;
        public string command;
        public string data_json;
        public int[] target_ids;
        public string target;
        public int seq;
    }

    [System.Serializable]
    public class ConnectedData
    {
        public string connection_id;
        public int player_id;
        public string player_name;
        public string room_id;
        public string client_type;
        public bool udp_enabled;
        public int udp_port;
    }

    [System.Serializable]
    public class ConnectedMessage
    {
        public string type;
        public ConnectedData data;
    }

    [System.Serializable]
    public class UdpBind
    {
        public int v = 1;
    }

    [System.Serializable]
    public class UdpBindAck
    {
        public bool ok;
        public string error;
    }

    [System.Serializable]
    public class UdpPing
    {
        public int t;
    }

    [System.Serializable]
    public class UdpPong
    {
        public int t;
        public long ts;
    }
}
