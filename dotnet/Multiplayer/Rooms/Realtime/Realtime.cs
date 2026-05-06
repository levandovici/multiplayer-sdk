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
    public class Realtime
    {
        private ClientWebSocket? _websocket;
        private string? _token;
        private PlayerInfo? _playerInfo;
        private CancellationTokenSource? _cancellationTokenSource;

        public event Func<string, object, SenderInfo, Task>? OnReceive;
        public event Func<Task>? OnConnected;
        public event Func<Task>? OnDisconnected;

        public static async Task<TokenResponse> GetTokenAsync(Client client, string playerToken)
        {
            var url = client.Url(Endpoints.RealtimeToken, $"&player_token={playerToken}");
            return await client.Send<TokenResponse>(HttpMethod.Post, url, null);
        }

        public static async Task<ValidateResponse> ValidateTokenAsync(Client client, string token)
        {
            var url = $"{Endpoints.RealtimeValidate}?token={token}";
            return await client.Send<ValidateResponse>(HttpMethod.Get, url, null);
        }

        public static async Task<RevokeResponse> RevokeTokenAsync(Client client, string playerToken)
        {
            var url = client.Url(Endpoints.RealtimeRevoke, $"&player_token={playerToken}");
            return await client.Send<RevokeResponse>(HttpMethod.Post, url, null);
        }

        public async Task<bool> ConnectAsync(string realtimeToken)
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _token = realtimeToken;

                // Connect to WebSocket server with .NET client type
                var uri = new Uri($"{Endpoints.RealtimeWebSocket}?token={_token}&client=dotnet");
                _websocket = new ClientWebSocket();
                
                await _websocket.ConnectAsync(uri, _cancellationTokenSource.Token);
                
                // Start listening for messages
                _ = Task.Run(ListenForMessagesAsync);
                
                OnConnected?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                return false;
            }
        }

        public async Task SendAsync(ERoomTargetPlayer target, string command, object? data = null)
        {
            if (_websocket?.State != WebSocketState.Open)
                return;

            var message = new
            {
                type = "send",
                command = command,
                data = data,
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

        public async Task DisconnectAsync()
        {
            try
            {
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
            var buffer = new byte[1024 * 4];

            while (_websocket?.State == WebSocketState.Open && !(_cancellationTokenSource?.Token.IsCancellationRequested ?? true))
            {
                try
                {
                    var result = await _websocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), 
                        _cancellationTokenSource?.Token ?? CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var realtimeMessage = JsonSerializer.Deserialize<RealtimeMessage>(message);

                        if (realtimeMessage?.type == "receive")
                        {
                            if (realtimeMessage != null && OnReceive != null)
                            {
                                await OnReceive.Invoke(
                                    realtimeMessage.command ?? string.Empty, 
                                    realtimeMessage.data ?? new object(), 
                                    realtimeMessage.sender);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message receive error: {ex.Message}");
                }
            }
        }
    }
}
