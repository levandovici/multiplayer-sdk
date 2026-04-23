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
        public static Task<RoomCreateResponse> CreateRoomAsync<TPlayerData, TRules>(Multiplayer client, string playerToken, string roomName, string password = null,
            int maxPlayers = 4, bool hostSwitch = false, TPlayerData playerData = null, TRules rules = null, CancellationToken ct = default)
            where TPlayerData : class, new() where TRules : class, new()
                => client.Send<RoomCreateResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomCreate, $"&player_token={playerToken}"),
                    new RoomCreateRequest(roomName, password, maxPlayers, hostSwitch, JsonUtility.ToJson(playerData), JsonUtility.ToJson(rules)), ct);

        public static Task<RoomListResponse<T>> GetRoomsAsync<T>(Multiplayer client, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomListResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomList), null, ct);

        public static Task<RoomJoinResponse> JoinRoomAsync<T>(Multiplayer client, string playerToken, string roomId, string password = null, T playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomJoinResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.GameRoomJoin, roomId), $"&player_token={playerToken}"),
                (password != null || playerData != null) ? new RoomJoinRequest(password, JsonUtility.ToJson(playerData)) : null, ct);

        public static Task<RoomLeaveResponse> LeaveRoomAsync(Multiplayer client, string playerToken, CancellationToken ct = default)
            => client.Send<RoomLeaveResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomLeave, $"&player_token={playerToken}"), null, ct);

        public static Task<RoomPlayersResponse<T>> GetRoomPlayersAsync<T>(Multiplayer client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomPlayersResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomPlayers, $"&player_token={playerToken}"), null, ct);

        public static Task<HeartbeatResponse> SendRoomHeartbeatAsync(Multiplayer client, string playerToken, CancellationToken ct = default)
            => client.Send<HeartbeatResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomHeartbeat, $"&player_token={playerToken}"), null, ct);

        public static Task<CurrentRoomResponse<T>> GetCurrentRoomAsync<T>(Multiplayer client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<CurrentRoomResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomCurrent, $"&player_token={playerToken}"), null, ct);
    }
}
