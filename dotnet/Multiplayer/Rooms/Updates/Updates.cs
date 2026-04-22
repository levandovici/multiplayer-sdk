using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    public class Updates
    {
        public static Task<UpdatePlayersResponse> UpdatePlayersAsync<T>(Multiplayer client, string playerToken, UpdatePlayers<T> request, CancellationToken ct = default) where T : class, new()
            => client.Send<UpdatePlayersResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomUpdates, $"&player_token={playerToken}"),
                new UpdatePlayersRequest<T>(request.Target_players, request.Type, request.Data, request.Target_players_ids), ct);

        public static Task<PollUpdatesResponse<T>> PollUpdatesAsync<T>(Multiplayer client, string playerToken,
            string? lastUpdateId = null, CancellationToken ct = default) where T : class, new()
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

            return client.Send<PollUpdatesResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomUpdatesPoll, extra), null, ct);
        }
    }
}
