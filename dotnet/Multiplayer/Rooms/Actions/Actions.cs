using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Actions
{
    /// <summary>
    /// Provides static methods for managing room actions.
    /// Handles submitting actions, polling for completed actions, retrieving pending actions, and completing actions.
    /// </summary>
    public class Actions
    {
        /// <summary>
        /// Submits an action to target players in the room.
        /// </summary>
        /// <typeparam name="T">The type of action data.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="request">The action submission request containing targets and data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the action submission response.</returns>
        public static Task<ActionSubmitResponse> SubmitActionAsync<T>(Client client, string playerToken, SubmitAction<T> request, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionSubmitResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomActions,
        $"&player_token={playerToken}"), new ActionSubmitRequest<T>(request.Target_players, request.Action_type, request.Request_data, request.Target_players_ids), ct);

        /// <summary>
        /// Polls for completed actions that were targeted to the current player.
        /// </summary>
        /// <typeparam name="T">The type of action data.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the poll actions response with completed actions.</returns>
        public static Task<ActionPollResponse<T>> PollActionsAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionPollResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomActionsPoll, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Retrieves pending actions that need to be completed by the host.
        /// Only the host can retrieve pending actions.
        /// </summary>
        /// <typeparam name="T">The type of action data.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the pending actions response.</returns>
        public static Task<ActionPendingResponse<T>> GetPendingActionsAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionPendingResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomActionsPending, $"&player_token={playerToken}"), null, ct);
        
        /// <summary>
        /// Marks an action as complete with an optional response.
        /// Only the host can complete actions.
        /// </summary>
        /// <typeparam name="T">The type of response data.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="actionId">The ID of the action to complete.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="request">The action completion request containing status and response data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the action completion response.</returns>
        public static Task<ActionCompleteResponse> CompleteActionAsync<T>(Client client, string actionId, string playerToken,
            ActionComplete<T> request, CancellationToken ct = default) where T : class, new()
            => client.Send<ActionCompleteResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.GameRoomActionComplete, actionId),
                $"&player_token={playerToken}"), new ActionCompleteRequest<T>(request.Status, request.Response_data), ct);
    }
}
