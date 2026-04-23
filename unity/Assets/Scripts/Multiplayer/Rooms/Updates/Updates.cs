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

        public static Task<PollUpdatesResponse> PollUpdatesAsync(Multiplayer client, string playerToken, string lastUpdateId = null, CancellationToken ct = default)
        {
            string extra;

            if (string.IsNullOrEmpty(lastUpdateId))
            {
                extra = $"&player_token={playerToken}";
            }
            else
            {
                extra = $"&player_token={playerToken}&last_update={lastUpdateId}";
            }

            return client.Send<PollUpdatesResponse>(HttpMethod.Get, client.Url(Endpoints.GameRoomUpdatesPoll, extra), null, ct);
        }
    }
}
