using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms
{
    public static class Rooms
    {
        public static Task<RoomCreateResponse> CreateRoomAsync<TPlayerData, TRules>(Client client, string playerToken, string roomName, string password = null,
            int maxPlayers = 4, bool hostSwitch = false, bool realtime = false, TPlayerData playerData = null, TRules rules = null, CancellationToken ct = default)
            where TPlayerData : class, new() where TRules : class, new()
                => client.Send<RoomCreateResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomCreate, $"&player_token={playerToken}"),
                    new RoomCreateRequest(roomName, password, maxPlayers, hostSwitch, realtime, JsonUtility.ToJson(playerData), JsonUtility.ToJson(rules)), ct);

        public static Task<RoomListResponse<T>> GetRoomsAsync<T>(Client client, string search = "", int limit = 20, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomListResponse<T>>(HttpMethod.Post, client.Url(Endpoints.GameRoomList), new RoomListRequest { search = search, limit = limit }, ct);

        public static Task<RoomJoinResponse> JoinRoomAsync<T>(Client client, string playerToken, string roomId, string password = null, T playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomJoinResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.GameRoomJoin, roomId), $"&player_token={playerToken}"),
                (password != null || playerData != null) ? new RoomJoinRequest(password, JsonUtility.ToJson(playerData)) : null, ct);

        public static Task<RoomLeaveResponse> LeaveRoomAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<RoomLeaveResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomLeave, $"&player_token={playerToken}"), null, ct);

        public static Task<RoomPlayersResponse<T>> GetRoomPlayersAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomPlayersResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomPlayers, $"&player_token={playerToken}"), null, ct);

        public static Task<HeartbeatResponse> SendRoomHeartbeatAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<HeartbeatResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomHeartbeat, $"&player_token={playerToken}"), null, ct);

        public static Task<CurrentRoomResponse<T>> GetCurrentRoomAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<CurrentRoomResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomCurrent, $"&player_token={playerToken}"), null, ct);

        public static Task<SuccessResponse> StopRoomAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<SuccessResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomStop, $"&player_token={playerToken}"), null, ct);

        public static Task<RoomKickResponse> KickPlayerAsync(Client client, string playerToken, int playerId, CancellationToken ct = default)
            => client.Send<RoomKickResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomKick, $"&player_token={playerToken}"), new RoomKickRequest { player_id = playerId }, ct);

        public static Task<SuccessResponse> UpdateRoomPasswordAsync(Client client, string playerToken, string password, CancellationToken ct = default)
            => client.Send<SuccessResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomPassword, $"&player_token={playerToken}"), new RoomPasswordUpdateRequest(password), ct);
    }
}
