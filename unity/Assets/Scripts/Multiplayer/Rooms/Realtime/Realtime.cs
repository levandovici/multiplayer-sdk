using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Michitai.Multiplayer;
using System.Net.Http;

namespace Michitai.Multiplayer.Rooms.Realtime
{
    public class Realtime
    {
        private ClientWebSocket _websocket;
        private string _token;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _heartbeatCancellationTokenSource;
        private readonly string _realtimeWebSocketUrl;
        private bool _isConnecting = false;

        public event Action<string, string, SenderInfo> OnReceive;
        public event Action OnConnected;

        public Realtime(string realtimeWebSocketUrl = "ws://realtime.michitai.com")
        {
            _realtimeWebSocketUrl = realtimeWebSocketUrl;
        }

        public static async Task<TokenResponse> GetTokenAsync(Client client, string playerToken)
        {
            var url = client.Url(Endpoints.RealtimeToken, $"&player_token={playerToken}");
            return await client.Send<TokenResponse>(HttpMethod.Post, url, null);
        }

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
                    await httpClient.GetAsync("http://realtime.michitai.com/");
                }

                var uri = new Uri($"{_realtimeWebSocketUrl}?token={_token}&client=unity");

                _websocket = new ClientWebSocket();

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

                        if (realtimeMessage?.type == "receive" && OnReceive != null)
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

                await _websocket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    _heartbeatCancellationTokenSource?.Token ?? CancellationToken.None);
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
}
