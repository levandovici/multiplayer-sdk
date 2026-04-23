using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public static class Actions
    {
        public static Task<ActionSubmitResponse> SubmitActionAsync<T>(Multiplayer client, string playerToken, SubmitAction<T> request, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionSubmitResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomActions, $"&player_token={playerToken}"),
                new ActionSubmitRequest(request.TargetPlayers, request.ActionType, JsonUtility.ToJson(request.RequestData), request.TargetPlayersIds), ct);

        public static Task<ActionPollResponse> PollActionsAsync(Multiplayer client, string playerToken, CancellationToken ct = default)
            => client.Send<ActionPollResponse>(HttpMethod.Get, client.Url(Endpoints.GameRoomActionsPoll, $"&player_token={playerToken}"), null, ct);

        public static Task<ActionPendingResponse<T>> GetPendingActionsAsync<T>(Multiplayer client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionPendingResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomActionsPending, $"&player_token={playerToken}"), null, ct);

        public static Task<ActionCompleteResponse> CompleteActionAsync<T>(Multiplayer client, string actionId, string playerToken, ActionComplete<T> request, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionCompleteResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.GameRoomActionComplete, actionId), $"&player_token={playerToken}"),
                new ActionCompleteRequest(request.Status.ToString().ToLower(), JsonUtility.ToJson(request.ResponseData)), ct);
    }
}
