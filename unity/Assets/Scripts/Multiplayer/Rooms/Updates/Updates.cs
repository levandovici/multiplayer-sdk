using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Rooms.Updates
{
    public static class Updates
    {
        public static Task<UpdatePlayersResponse> UpdatePlayersAsync<T>(Multiplayer client, string playerToken, UpdatePlayers<T> request, CancellationToken ct = default) where T : class, new()
            => client.Send<UpdatePlayersResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomUpdates, $"&player_token={playerToken}"),
                new UpdatePlayersRequest(request.TargetPlayers, request.Type, JsonUtility.ToJson(request.Data), request.TargetPlayersIds), ct);

        public static Task<PollUpdatesResponse> PollUpdatesAsync(Multiplayer client, string playerToken, PollUpdates request, string lastUpdateId = null, CancellationToken ct = default)
            => client.Send<PollUpdatesResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomUpdatesPoll, $"&player_token={playerToken}"),
                new PollUpdatesRequest(request.FromPlayers, request.FromPlayersIds, request.LastUpdate), ct);
    }
}
