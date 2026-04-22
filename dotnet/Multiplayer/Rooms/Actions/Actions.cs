using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    public class Actions
    {
        public static Task<ActionSubmitResponse> SubmitActionAsync<T>(Multiplayer client, string playerToken, SubmitAction<T> request, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionSubmitResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomActions,
        $"&player_token={playerToken}"), new ActionSubmitRequest<T>(request.Target_players, request.Action_type, request.Request_data, request.Target_players_ids), ct);

        public static Task<ActionPollResponse<T>> PollActionsAsync<T>(Multiplayer client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionPollResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomActionsPoll, $"&player_token={playerToken}"), null, ct);

        public static Task<ActionPendingResponse<T>> GetPendingActionsAsync<T>(Multiplayer client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionPendingResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomActionsPending, $"&player_token={playerToken}"), null, ct);
        
        public static Task<ActionCompleteResponse> CompleteActionAsync<T>(Multiplayer client, string actionId, string playerToken,
            ActionComplete<T> request, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionCompleteResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.GameRoomActionComplete, actionId),
                $"&player_token={playerToken}"), new ActionCompleteRequest<T>(request.Status, request.Response_data), ct);
    }
}
