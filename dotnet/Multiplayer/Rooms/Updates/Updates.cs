using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms.Updates
{
    /// <summary>
    /// Provides static methods for managing room updates.
    /// Handles sending updates to players and polling for received updates.
    /// </summary>
    public class Updates
    {
        /// <summary>
        /// Sends updates to specific players in the room.
        /// </summary>
        /// <typeparam name="T">The type of update data.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="request">The update request containing targets and data.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the update players response.</returns>
        public static Task<UpdatePlayersResponse> UpdatePlayersAsync<T>(Client client, string playerToken, UpdatePlayers<T> request, CancellationToken ct = default) where T : class, new()
            => client.Send<UpdatePlayersResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomUpdates, $"&player_token={playerToken}"),
                new UpdatePlayersRequest<T>(request.Target_players, request.Type, request.Data, request.Target_players_ids), ct);

        /// <summary>
        /// Polls for updates that were sent to the current player.
        /// </summary>
        /// <typeparam name="T">The type of update data.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="request">The poll request containing filters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the poll updates response with received updates.</returns>
        public static Task<PollUpdatesResponse<T>> PollUpdatesAsync<T>(Client client, string playerToken, PollUpdates request,
            CancellationToken ct = default) where T : class, new()
                => client.Send<PollUpdatesResponse<T>>(HttpMethod.Post, client.Url(Endpoints.GameRoomUpdatesPoll, $"&player_token={playerToken}"),
                    new PollUpdatesRequest(request.From_players, request.From_players_ids, request.Last_update), ct);
    }
}
