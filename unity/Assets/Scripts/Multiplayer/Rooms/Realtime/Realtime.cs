using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Michitai.Multiplayer;
using System.Net.Http;

namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Manages realtime communication in game rooms over two channels:
    /// a reliable WebSocket (TCP) connection for control traffic plus one
    /// optional bound UDP address for high-frequency unreliable traffic
    /// (movement state, inputs). One player may hold one TCP connection
    /// and one UDP binding at the same time — see ConnectUdpAsync.
    /// UDP is unavailable on WebGL — SendUnreliableAsync falls back to the socket.
    /// Uses Unity's JsonUtility for serialization.
    /// </summary>
    public class Realtime
    {
        private ClientWebSocket _websocket;
        private string _token;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _heartbeatCancellationTokenSource;
        private readonly string _realtimeWebSocketUrl;
        private bool _isConnecting = false;

        // UDP companion channel state
        private UdpClient _udpSocket;
        private CancellationTokenSource _udpCancellationTokenSource;
        private volatile bool _udpBound;
        private string _connectionId;
        private int _serverUdpPort;
        private int _udpSeq;

        /// <summary>
        /// Event raised when a message is received (either transport).
        /// </summary>
        public event Action<string, string, SenderInfo> OnReceive;

        /// <summary>
        /// Event raised when the WebSocket connection is established.
        /// </summary>
        public event Action OnConnected;

        /// <summary>The connection id issued by the server, or null before connect.</summary>
        public string ConnectionId => _connectionId;

        /// <summary>True while a UDP address is bound to this session.</summary>
        public bool IsUdpConnected => _udpBound && _udpSocket != null;

        /// <summary>Last measured UDP round-trip time in milliseconds (0 = unknown).</summary>
        public int LastUdpRttMs { get; private set; }

        /// <summary>
        /// Initializes a new Realtime instance.
        /// </summary>
        /// <param name="realtimeWebSocketUrl">The WebSocket server URL (default: wss://realtime.michitai.com).</param>
        public Realtime(string realtimeWebSocketUrl = "wss://realtime.michitai.com")
        {
            _realtimeWebSocketUrl = realtimeWebSocketUrl;
        }

        /// <summary>
        /// Retrieves a realtime authentication token for WebSocket connections.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's private authentication token.</param>
        /// <returns>Response containing the realtime token.</returns>
        public static async Task<TokenResponse> GetTokenAsync(Client client, string playerToken)
        {
            var url = client.Url(Endpoints.RealtimeToken);
            return await client.Send<TokenResponse>(HttpMethod.Post, url, null, playerToken: playerToken);
        }

        /// <summary>
        /// Connects to the realtime WebSocket server using the provided token.
        /// </summary>
        /// <param name="realtimeToken">The realtime authentication token.</param>
        /// <returns>True if connection succeeded, false otherwise.</returns>
        public async Task<bool> ConnectAsync(string realtimeToken)
        {
            if (_isConnecting || (_websocket?.State == WebSocketState.Open))
                return false;

            _isConnecting = true;
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _token = realtimeToken;

                // Wake up the server before connecting
                using (var httpClient = new HttpClient())
                {
                    await httpClient.GetAsync("https://realtime.michitai.com/");
                }

                var uri = new Uri($"{_realtimeWebSocketUrl}?client=unity");

                _websocket = new ClientWebSocket();
                try
                {
                    // Prefer the header so the token stays out of URLs/logs.
                    _websocket.Options.SetRequestHeader("X-Realtime-Token", _token);
                }
                catch (Exception)
                {
                    // Platforms without header support (e.g. WebGL) fall back to the query token.
                    uri = new Uri($"{_realtimeWebSocketUrl}?client=unity&token={_token}");
                }

                await _websocket.ConnectAsync(uri, _cancellationTokenSource.Token);

                _ = Task.Run(ListenForMessagesAsync, _cancellationTokenSource.Token);
                _ = Task.Run(StartHeartbeatAsync, _cancellationTokenSource.Token);

                OnConnected?.Invoke();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Connection failed: {ex.Message}");
                await DisconnectAsync();
                return false;
            }
            finally
            {
                _isConnecting = false;
            }
        }

        /// <summary>
        /// Binds a UDP endpoint to this session. Call after ConnectAsync; the
        /// server accepts one UDP binding per connection. Datagrams are
        /// authenticated with the connection id issued on the socket.
        /// </summary>
        /// <param name="udpPort">Override port — defaults to the udp_port
        /// announced in the server's connected payload.</param>
        /// <returns>True when the server acknowledged the binding.</returns>
        public async Task<bool> ConnectUdpAsync(int? udpPort = null)
        {
            if (_websocket?.State != WebSocketState.Open || _connectionId == null) return false;
            if (_udpBound) return true;

            try
            {
                var uri = new Uri(_realtimeWebSocketUrl);
                // The WS port is a proxy port for wss:// — UDP needs the
                // service port announced by the server (3000 by default).
                int port = udpPort ?? (_serverUdpPort > 0 ? _serverUdpPort : 3000);

                _udpSocket = new UdpClient();
                _udpSocket.Connect(uri.Host, port);
                _udpCancellationTokenSource = new CancellationTokenSource();
                _ = Task.Run(ListenUdpAsync);

                // BIND is unreliable — retry once before giving up.
                var bindPacket = RealtimeUdpWire.Encode(RealtimeUdpWire.PacketType.Bind,
                    _connectionId, JsonUtility.ToJson(new UdpBind()));
                for (int attempt = 0; attempt < 2 && !_udpBound; attempt++)
                {
                    await _udpSocket.SendAsync(bindPacket, bindPacket.Length);
                    var deadline = DateTime.UtcNow.AddMilliseconds(1500);
                    while (!_udpBound && DateTime.UtcNow < deadline)
                        await Task.Delay(25);
                }
                return _udpBound;
            }
            catch (Exception ex)
            {
                // UdpClient is unsupported on some platforms (WebGL) — stay TCP-only.
                Debug.LogWarning($"UDP connect failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sends a message to the specified players via WebSocket.
        /// Uses Unity's JsonUtility for serialization.
        /// </summary>
        /// <typeparam name="T">The type of data to send.</typeparam>
        /// <param name="target">The target players (All, Host, Others, Specific).</param>
        /// <param name="command">The command/type of the message.</param>
        /// <param name="data">Optional data payload to send.</param>
        /// <param name="targetIds">Specific player IDs if target is Specific.</param>
        public async Task SendAsync<T>(ERoomTargetPlayer target, string command, T data = null, int[] targetIds = null) where T : class, new()
        {
            if (_websocket?.State != WebSocketState.Open) return;

            var message = new SendMessage
            {
                type = "send",
                command = command,
                data_json = data != null ? JsonUtility.ToJson(data) : null,
                target_ids = targetIds ?? new int[0],
                target = target.ToString().ToLower()
            };

            var json = JsonUtility.ToJson(message);
            var buffer = Encoding.UTF8.GetBytes(json);

            await _websocket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                _cancellationTokenSource?.Token ?? CancellationToken.None);
        }

        /// <summary>
        /// Sends over the bound UDP channel — unordered, unacknowledged,
        /// latest-wins via seq. Best for movement/state streams.
        /// Falls back to the WebSocket when UDP is not bound.
        /// </summary>
        public async Task SendUnreliableAsync<T>(ERoomTargetPlayer target, string command, T data = null, int[] targetIds = null) where T : class, new()
        {
            if (!_udpBound || _udpSocket == null || _connectionId == null)
            {
                await SendAsync(target, command, data, targetIds);
                return;
            }

            var message = new SendMessage
            {
                type = "send",
                command = command,
                data_json = data != null ? JsonUtility.ToJson(data) : null,
                seq = ++_udpSeq,
                target_ids = targetIds ?? new int[0],
                target = target.ToString().ToLower()
            };

            var packet = RealtimeUdpWire.Encode(RealtimeUdpWire.PacketType.Msg,
                _connectionId, JsonUtility.ToJson(message));
            await _udpSocket.SendAsync(packet, packet.Length);
        }

        /// <summary>
        /// Disconnects from the server and cleans up both channels.
        /// </summary>
        public async Task DisconnectAsync()
        {
            try
            {
                _heartbeatCancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Cancel();
                _udpCancellationTokenSource?.Cancel();
                _udpBound = false;

                if (_websocket?.State == WebSocketState.Open)
                {
                    await _websocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Client disconnecting",
                        CancellationToken.None);
                }

                try { _udpSocket?.Close(); } catch {}
                _udpSocket = null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Disconnect error: {ex.Message}");
            }
        }

        private async Task ListenForMessagesAsync()
        {
            var buffer = new byte[4096];

            while (_websocket?.State == WebSocketState.Open &&
                !(_cancellationTokenSource?.Token.IsCancellationRequested ?? true))
            {
                try
                {
                    var result = await _websocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        _cancellationTokenSource?.Token ?? CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text && result.Count > 0)
                    {
                        var messageStr = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var realtimeMessage = JsonUtility.FromJson<RealtimeMessage>(messageStr);

                        if (realtimeMessage?.type == "connected")
                        {
                            var connected = JsonUtility.FromJson<ConnectedMessage>(messageStr);
                            if (connected?.data != null)
                            {
                                _connectionId = connected.data.connection_id;
                                _serverUdpPort = connected.data.udp_port;
                            }
                        }
                        else if (realtimeMessage?.type == "receive" && OnReceive != null)
                        {
                            OnReceive.Invoke(realtimeMessage.command ?? string.Empty,
                                realtimeMessage.data_json ?? string.Empty,
                                realtimeMessage.sender);
                        }
                    }
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    Debug.LogError($"Message receive error: {ex.Message}");
                    if (ex is WebSocketException) break;
                }
            }
        }

        private async Task ListenUdpAsync()
        {
            while (_udpSocket != null &&
                !(_udpCancellationTokenSource?.Token.IsCancellationRequested ?? true))
            {
                UdpReceiveResult result;
                try
                {
                    result = await _udpSocket.ReceiveAsync();
                }
                catch (ObjectDisposedException) { break; }
                catch
                {
                    if (_udpCancellationTokenSource?.Token.IsCancellationRequested ?? true) break;
                    continue;
                }

                if (!RealtimeUdpWire.TryDecode(result.Buffer, out var type, out var connId, out var json))
                    continue;
                if (connId != _connectionId) continue;

                switch (type)
                {
                    case RealtimeUdpWire.PacketType.BindAck:
                        var bindAck = JsonUtility.FromJson<UdpBindAck>(json ?? "{}");
                        if (bindAck != null && bindAck.ok) _udpBound = true;
                        break;

                    case RealtimeUdpWire.PacketType.Msg:
                        var msg = JsonUtility.FromJson<RealtimeMessage>(json ?? "{}");
                        if (msg?.type == "receive" && OnReceive != null)
                        {
                            OnReceive.Invoke(msg.command ?? string.Empty,
                                msg.data_json ?? string.Empty, msg.sender);
                        }
                        break;

                    case RealtimeUdpWire.PacketType.Pong:
                        var pong = JsonUtility.FromJson<UdpPong>(json ?? "{}");
                        if (pong != null)
                        {
                            var now = (int)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
                            LastUdpRttMs = Math.Max(0, now - pong.t);
                        }
                        break;
                }
            }
        }

        private async Task StartHeartbeatAsync()
        {
            _heartbeatCancellationTokenSource = new CancellationTokenSource();

            while (!_heartbeatCancellationTokenSource.Token.IsCancellationRequested &&
                   _websocket?.State == WebSocketState.Open)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(20), _heartbeatCancellationTokenSource.Token);

                    if (!_heartbeatCancellationTokenSource.Token.IsCancellationRequested &&
                        _websocket?.State == WebSocketState.Open)
                    {
                        await SendHeartbeatAsync();
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Heartbeat error: {ex.Message}");
                }
            }
        }

        private async Task SendHeartbeatAsync()
        {
            try
            {
                var message = new SendMessage
                {
                    type = "heartbeat"
                };

                var json = JsonUtility.ToJson(message);
                var buffer = Encoding.UTF8.GetBytes(json);

                if (_websocket?.State == WebSocketState.Open)
                {
                    await _websocket.SendAsync(
                        new ArraySegment<byte>(buffer),
                        WebSocketMessageType.Text,
                        true,
                        _heartbeatCancellationTokenSource?.Token ?? CancellationToken.None);
                }

                // Keep the NAT mapping warm on the UDP channel too.
                if (_udpBound && _udpSocket != null && _connectionId != null)
                {
                    var ping = new UdpPing
                    {
                        t = (int)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond)
                    };
                    var packet = RealtimeUdpWire.Encode(RealtimeUdpWire.PacketType.Ping,
                        _connectionId, JsonUtility.ToJson(ping));
                    await _udpSocket.SendAsync(packet, packet.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Send heartbeat error: {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            _ = DisconnectAsync();
        }
    }

    /// <summary>
    /// MUR1 datagram codec — [4B magic][1B type][16B connection id][JSON].
    /// Shared layout with realtime/src/protocol.js.
    /// </summary>
    internal static class RealtimeUdpWire
    {
        public const int HeaderSize = 21;

        public enum PacketType : byte
        {
            Bind = 1,
            BindAck = 2,
            Msg = 3,
            Ping = 4,
            Pong = 5
        }

        public static byte[] Encode(PacketType type, string connectionId, string jsonBody)
        {
            byte[] id = UuidToBytes(connectionId);
            if (id == null) return null;
            byte[] body = jsonBody == null ? new byte[0] : Encoding.UTF8.GetBytes(jsonBody);
            byte[] buf = new byte[HeaderSize + body.Length];
            buf[0] = (byte)'M'; buf[1] = (byte)'U'; buf[2] = (byte)'R'; buf[3] = (byte)'1';
            buf[4] = (byte)type;
            Buffer.BlockCopy(id, 0, buf, 5, 16);
            Buffer.BlockCopy(body, 0, buf, HeaderSize, body.Length);
            return buf;
        }

        public static bool TryDecode(byte[] datagram, out PacketType type, out string connectionId, out string json)
        {
            type = 0;
            connectionId = null;
            json = null;
            if (datagram == null || datagram.Length < HeaderSize) return false;
            if (datagram[0] != 'M' || datagram[1] != 'U' || datagram[2] != 'R' || datagram[3] != '1')
                return false;
            type = (PacketType)datagram[4];
            connectionId = BytesToUuid(datagram, 5);
            json = datagram.Length > HeaderSize
                ? Encoding.UTF8.GetString(datagram, HeaderSize, datagram.Length - HeaderSize)
                : null;
            return true;
        }

        // Manual hex — Convert.FromHexString is unavailable on older Unity runtimes.
        private static byte[] UuidToBytes(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) return null;
            var hex = uuid.Replace("-", "");
            if (hex.Length != 32) return null;
            var bytes = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                int hi = HexVal(hex[i * 2]);
                int lo = HexVal(hex[i * 2 + 1]);
                if (hi < 0 || lo < 0) return null;
                bytes[i] = (byte)((hi << 4) | lo);
            }
            return bytes;
        }

        private static string BytesToUuid(byte[] buf, int offset)
        {
            var chars = new char[36];
            int j = 0;
            for (int i = 0; i < 16; i++)
            {
                if (i == 4 || i == 6 || i == 8 || i == 10) chars[j++] = '-';
                byte b = buf[offset + i];
                chars[j++] = HexChar(b >> 4);
                chars[j++] = HexChar(b & 0xF);
            }
            return new string(chars);
        }

        private static int HexVal(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            return -1;
        }

        private static char HexChar(int v) => (char)(v < 10 ? '0' + v : 'a' + v - 10);
    }
}
