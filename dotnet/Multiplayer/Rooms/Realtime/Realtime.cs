using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Michitai.Multiplayer;

namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Manages realtime communication in game rooms over two channels:
    /// a reliable WebSocket (TCP) connection for control traffic plus one
    /// optional bound UDP address for high-frequency unreliable traffic
    /// (movement state, inputs). One player may hold one TCP connection
    /// and one UDP binding at the same time — see <see cref="ConnectUdpAsync"/>.
    /// </summary>
    public class Realtime
    {
        private ClientWebSocket? _websocket;
        private string? _token;
        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationTokenSource? _heartbeatCancellationTokenSource;
        private readonly string _realtimeWebSocketUrl;

        // UDP companion channel state
        private UdpClient? _udpSocket;
        private CancellationTokenSource? _udpCancellationTokenSource;
        private volatile bool _udpBound;
        private string? _connectionId;
        private int _serverUdpPort;
        private int _udpSeq;
        private long _lastUdpPingTicks;

        /// <summary>
        /// Event raised when a message is received (either transport).
        /// </summary>
        public event Action<string, object, SenderInfo>? OnReceive;

        /// <summary>
        /// Event raised when the WebSocket connection is established.
        /// </summary>
        public event Action? OnConnected;

        /// <summary>The connection id issued by the server, or null before connect.</summary>
        public string? ConnectionId => _connectionId;

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
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _token = realtimeToken;

                // Wake up the server before connecting
                using (var httpClient = new HttpClient())
                {
                    await httpClient.GetAsync("https://realtime.michitai.com/");
                }

                var uri = new Uri($"{_realtimeWebSocketUrl}?client=json");
                _websocket = new ClientWebSocket();
                _websocket.Options.SetRequestHeader("X-Realtime-Token", _token);

                await _websocket.ConnectAsync(uri, _cancellationTokenSource.Token);

                _ = Task.Run(ListenForMessagesAsync, _cancellationTokenSource.Token);
                _ = Task.Run(StartHeartbeatAsync, _cancellationTokenSource.Token);

                OnConnected?.Invoke();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                await DisconnectAsync();
                return false;
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
                var bindPacket = RealtimeUdpWire.Encode(RealtimeUdpWire.PacketType.Bind, _connectionId, "{\"v\":1}");
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
                Console.WriteLine($"UDP connect failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sends a message to the specified players via WebSocket.
        /// </summary>
        /// <param name="target">The target players (All, Host, Others, Specific).</param>
        /// <param name="command">The command/type of the message.</param>
        /// <param name="data">Optional data payload to send.</param>
        /// <param name="targetIds">Specific player IDs if target is Specific.</param>
        public async Task SendAsync(ERoomTargetPlayer target, string command, object? data = null, int[]? targetIds = null)
        {
            if (_websocket?.State != WebSocketState.Open) return;

            var message = new
            {
                type = "send",
                command = command,
                data = data,
                target_ids = targetIds ?? new int[0],
                target = target.ToString().ToLower()
            };

            var json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);

            await _websocket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                _cancellationTokenSource?.Token ?? CancellationToken.None);
        }

        /// <summary>
        /// Sends over the bound UDP channel — unordered, unacknowledged,
        /// latest-wins via <c>seq</c>. Best for movement/state streams.
        /// Falls back to the WebSocket when UDP is not bound.
        /// </summary>
        public async Task SendUnreliableAsync(ERoomTargetPlayer target, string command, object? data = null, int[]? targetIds = null)
        {
            if (!_udpBound || _udpSocket == null || _connectionId == null)
            {
                await SendAsync(target, command, data, targetIds);
                return;
            }

            var json = JsonSerializer.Serialize(new
            {
                type = "send",
                command = command,
                data = data,
                seq = Interlocked.Increment(ref _udpSeq),
                target_ids = targetIds ?? new int[0],
                target = target.ToString().ToLower()
            });
            var packet = RealtimeUdpWire.Encode(RealtimeUdpWire.PacketType.Msg, _connectionId, json);
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
                _udpSocket?.Close();
                _udpSocket = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disconnect error: {ex.Message}");
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
                        var realtimeMessage = JsonSerializer.Deserialize<RealtimeMessage>(messageStr,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (realtimeMessage?.type == "connected")
                        {
                            HandleConnected(realtimeMessage.data);
                        }
                        else if (realtimeMessage?.type == "receive" && OnReceive != null)
                        {
                            OnReceive.Invoke(realtimeMessage.command ?? string.Empty,
                                realtimeMessage.data ?? new object(),
                                realtimeMessage.sender);
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    if (ex is WebSocketException) break;
                }
            }
        }

        private void HandleConnected(object? data)
        {
            if (data is JsonElement el && el.ValueKind == JsonValueKind.Object)
            {
                if (el.TryGetProperty("connection_id", out var cid))
                    _connectionId = cid.GetString();
                if (el.TryGetProperty("udp_port", out var up) && up.ValueKind == JsonValueKind.Number)
                    _serverUdpPort = up.GetInt32();
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
                    result = await _udpSocket.ReceiveAsync(_udpCancellationTokenSource?.Token ?? CancellationToken.None);
                }
                catch (OperationCanceledException) { break; }
                catch (ObjectDisposedException) { break; }
                catch { continue; }

                if (!RealtimeUdpWire.TryDecode(result.Buffer, out var type, out var connId, out var json))
                    continue;
                if (connId != _connectionId) continue;

                switch (type)
                {
                    case RealtimeUdpWire.PacketType.BindAck:
                        try
                        {
                            using var doc = JsonDocument.Parse(json ?? "{}");
                            if (doc.RootElement.TryGetProperty("ok", out var ok) && ok.GetBoolean())
                                _udpBound = true;
                        }
                        catch { }
                        break;

                    case RealtimeUdpWire.PacketType.Msg:
                        try
                        {
                            var msg = JsonSerializer.Deserialize<RealtimeMessage>(json ?? "{}",
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            if (msg?.type == "receive" && OnReceive != null)
                            {
                                OnReceive.Invoke(msg.command ?? string.Empty,
                                    msg.data ?? new object(), msg.sender);
                            }
                        }
                        catch { }
                        break;

                    case RealtimeUdpWire.PacketType.Pong:
                        try
                        {
                            using var doc = JsonDocument.Parse(json ?? "{}");
                            if (doc.RootElement.TryGetProperty("t", out var t) &&
                                t.ValueKind == JsonValueKind.Number)
                            {
                                var sent = t.GetInt64();
                                var now = (int)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
                                LastUdpRttMs = Math.Max(0, now - (int)sent);
                            }
                        }
                        catch { }
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
                    Console.WriteLine($"Heartbeat error: {ex.Message}");
                }
            }
        }

        private async Task SendHeartbeatAsync()
        {
            try
            {
                var message = new
                {
                    type = "heartbeat"
                };

                var json = JsonSerializer.Serialize(message);
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
                    var nowMs = (int)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
                    var ping = RealtimeUdpWire.Encode(RealtimeUdpWire.PacketType.Ping,
                        _connectionId, $"{{\"t\":{nowMs}}}");
                    await _udpSocket.SendAsync(ping, ping.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send heartbeat error: {ex.Message}");
            }
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

        public static byte[] Encode(PacketType type, string connectionId, string? jsonBody)
        {
            byte[] id = Convert.FromHexString(connectionId.Replace("-", ""));
            byte[] body = jsonBody == null ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(jsonBody);
            byte[] buf = new byte[HeaderSize + body.Length];
            buf[0] = (byte)'M'; buf[1] = (byte)'U'; buf[2] = (byte)'R'; buf[3] = (byte)'1';
            buf[4] = (byte)type;
            Buffer.BlockCopy(id, 0, buf, 5, 16);
            Buffer.BlockCopy(body, 0, buf, HeaderSize, body.Length);
            return buf;
        }

        public static bool TryDecode(byte[] datagram, out PacketType type, out string? connectionId, out string? json)
        {
            type = 0;
            connectionId = null;
            json = null;
            if (datagram == null || datagram.Length < HeaderSize) return false;
            if (datagram[0] != 'M' || datagram[1] != 'U' || datagram[2] != 'R' || datagram[3] != '1')
                return false;
            type = (PacketType)datagram[4];
            var hex = Convert.ToHexString(datagram, 5, 16).ToLowerInvariant();
            connectionId = $"{hex.Substring(0, 8)}-{hex.Substring(8, 4)}-{hex.Substring(12, 4)}-{hex.Substring(16, 4)}-{hex.Substring(20)}";
            json = datagram.Length > HeaderSize ? Encoding.UTF8.GetString(datagram, HeaderSize, datagram.Length - HeaderSize) : null;
            return true;
        }
    }
}
