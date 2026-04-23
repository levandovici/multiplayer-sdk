using Michitai.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    public static class Players
    {
        public static Task<PlayerRegisterResponse> RegisterPlayer<T>(Multiplayer client, string name, T playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<PlayerRegisterResponse>(HttpMethod.Post, client.Url(Endpoints.GamePlayersRegister), new PlayerRegisterRequest(name, JsonUtility.ToJson(playerData)), ct);

        public static Task<PlayerAuthResponse<T>> AuthenticatePlayer<T>(Multiplayer client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<PlayerAuthResponse<T>>(HttpMethod.Put, client.Url(Endpoints.GamePlayersLogin, $"&player_token={playerToken}"), null, ct);

        public static Task<PlayerHeartbeatResponse> SendPlayerHeartbeatAsync(Multiplayer client, string playerToken, CancellationToken ct = default)
            => client.Send<PlayerHeartbeatResponse>(HttpMethod.Post, client.Url(Endpoints.GamePlayersHeartbeat, $"&player_token={playerToken}"), null, ct);

        public static Task<PlayerLogoutResponse> LogoutPlayerAsync(Multiplayer client, string playerToken, CancellationToken ct = default)
            => client.Send<PlayerLogoutResponse>(HttpMethod.Post, client.Url(Endpoints.GamePlayersLogout, $"&player_token={playerToken}"), null, ct);

        public static Task<PlayerRenameResponse> RenamePlayerAsync(Multiplayer client, string playerToken, string newName, CancellationToken ct = default)
            => client.Send<PlayerRenameResponse>(HttpMethod.Put, client.Url(Endpoints.GamePlayersRename, $"&player_token={playerToken}"), new PlayerRenameRequest(newName), ct);


        public static Task<PlayerDataResponse<T>> GetPlayerData<T>(Multiplayer client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<PlayerDataResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameDataPlayerGet, $"&player_token={playerToken}"), null, ct);

        public static Task<SuccessResponse> UpdatePlayerData<T>(Multiplayer client, string playerToken, T data, CancellationToken ct = default) where T : class, new()
            => client.Send<SuccessResponse>(HttpMethod.Put, client.Url(Endpoints.GameDataPlayerUpdate, $"&player_token={playerToken}"), data, ct);
    }
}
