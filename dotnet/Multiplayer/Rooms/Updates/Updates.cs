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

        public static Task<PollUpdatesResponse<T>> PollUpdatesAsync<T>(Multiplayer client, string playerToken, PollUpdates request,
            CancellationToken ct = default) where T : class, new()
                => client.Send<PollUpdatesResponse<T>>(HttpMethod.Post, client.Url(Endpoints.GameRoomUpdatesPoll, $"&player_token={playerToken}"),
                    new PollUpdatesRequest(request.From_players, request.From_players_ids, request.Last_update), ct);
    }
}
