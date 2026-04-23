using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Matchmaking.Requests
{
    public static class Requests
    {
        public static Task<MatchmakingCreateResponse> CreateMatchmakingLobbyAsync<TPlayerData, TRules>(Multiplayer client, string playerToken, string matchmakingName, int maxPlayers = 4, bool strictFull = false,
            bool joinByRequests = false, bool hostSwitch = false, bool canLeaveRoom = false, TPlayerData playerData = null, TRules rules = null, CancellationToken ct = default) where TPlayerData : class, new() where TRules : class, new()
            => client.Send<MatchmakingCreateResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingCreate, $"&player_token={playerToken}"),
                new MatchmakingCreateRequest(matchmakingName, maxPlayers, strictFull, joinByRequests, hostSwitch, canLeaveRoom, JsonUtility.ToJson(playerData), JsonUtility.ToJson(rules)), ct);

        public static Task<MatchmakingJoinRequestResponse> RequestToJoinMatchmakingAsync<T>(Multiplayer client, string playerToken, string matchmakingId, T playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingJoinRequestResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.MatchmakingRequest, matchmakingId), $"&player_token={playerToken}"), playerData, ct);

        public static Task<MatchmakingPermissionResponse> RespondToJoinRequestAsync(Multiplayer client, string playerToken, string requestId, EMatchmakingRequestAction action, CancellationToken ct = default)
            => client.Send<MatchmakingPermissionResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.MatchmakingResponse, requestId), $"&player_token={playerToken}"),
                new MatchmakingPermissionRequest(action.ToString().ToLower()), ct);

        public static Task<MatchmakingRequestStatusResponse> CheckJoinRequestStatusAsync(Multiplayer client, string playerToken, string requestId, CancellationToken ct = default)
            => client.Send<MatchmakingRequestStatusResponse>(HttpMethod.Get, client.Url(string.Format(Endpoints.MatchmakingRequestStatus, requestId), $"&player_token={playerToken}"), null, ct);
    }
}
