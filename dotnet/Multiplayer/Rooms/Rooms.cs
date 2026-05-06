using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    public class Rooms
    {
        public static Task<RoomCreateResponse> CreateRoomAsync<TPlayerData, TRules>(Client client, string playerToken, string roomName, int maxPlayers = 4,
           string? password = null, bool hostSwitch = false, bool realtime = false, TPlayerData? playerData = null, TRules? rules = null, CancellationToken ct = default) where TPlayerData : class, new() where TRules : class, new()
           => client.Send<RoomCreateResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomCreate, $"&player_token={playerToken}"),
               new RoomCreateRequest<TPlayerData, TRules>(roomName, maxPlayers, password, hostSwitch, realtime, playerData, rules), ct);

        public static Task<RoomListResponse<T>> GetRoomsAsync<T>(Client client, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomListResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomList), null, ct);

        public static Task<RoomJoinResponse> JoinRoomAsync<T>(Client client, string playerToken, string roomId, string? password = null, T? playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomJoinResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.GameRoomJoin, roomId), $"&player_token={playerToken}"),
                (password != null || playerData != null) ? new RoomJoinRequest<T>(password, playerData) : null, ct);

        public static Task<RoomLeaveResponse> LeaveRoomAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<RoomLeaveResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomLeave, $"&player_token={playerToken}"), null, ct);

        public static Task<RoomPlayersResponse<T>> GetRoomPlayersAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomPlayersResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomPlayers, $"&player_token={playerToken}"), null, ct);

        public static Task<HeartbeatResponse> SendRoomHeartbeatAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<HeartbeatResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomHeartbeat, $"&player_token={playerToken}"), null, ct);
        public  static Task<CurrentRoomResponse<T>> GetCurrentRoomAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<CurrentRoomResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomCurrent, $"&player_token={playerToken}"), null, ct);
    }
}
