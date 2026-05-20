using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Michitai.Multiplayer;

namespace Michitai.Multiplayer.Rooms.Realtime
{
    /// <summary>
    /// Manages WebSocket connections for realtime communication in game rooms.
    /// Handles connection, message sending/receiving, and automatic heartbeat.
    /// </summary>
    public class Realtime
    {
        private ClientWebSocket? _websocket;
        private string? _token;
        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationTokenSource? _heartbeatCancellationTokenSource;
        private readonly string _realtimeWebSocketUrl;

        /// <summary>
        /// Event raised when a message is received from the WebSocket.
        /// </summary>
        public event Action<string, object, SenderInfo>? OnReceive;

        /// <summary>
        /// Event raised when the WebSocket connection is established.
        /// </summary>
        public event Action? OnConnected;

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
            var url = client.Url(Endpoints.RealtimeToken, $"&player_token={playerToken}");
            return await client.Send<TokenResponse>(HttpMethod.Post, url, null);
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

                var uri = new Uri($"{_realtimeWebSocketUrl}?token={_token}&client=json");
                _websocket = new ClientWebSocket();

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
        /// Disconnects from the WebSocket server and cleans up resources.
        /// </summary>
        public async Task DisconnectAsync()
        {
            try
            {
                _heartbeatCancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Cancel();

                if (_websocket?.State == WebSocketState.Open)
                {
                    await _websocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Client disconnecting",
                        CancellationToken.None);
                }
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

                        if (realtimeMessage?.type == "receive" && OnReceive != null)
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send heartbeat error: {ex.Message}");
            }
        }

    }
}
