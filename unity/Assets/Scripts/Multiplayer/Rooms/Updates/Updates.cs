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
    /// <summary>
    /// Provides static methods for managing room updates.
    /// Handles sending updates to players and polling for received updates.
    /// Uses Unity's JsonUtility for serialization.
    /// </summary>
    public static class Updates
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
                new UpdatePlayersRequest(request.TargetPlayers, request.Type, JsonUtility.ToJson(request.Data), request.TargetPlayersIds), ct);

        /// <summary>
        /// Polls for updates that were sent to the current player.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="request">The poll request containing filters.</param>
        /// <param name="lastUpdateId">Optional ID of the last update to poll from.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the poll updates response with received updates.</returns>
        public static Task<PollUpdatesResponse> PollUpdatesAsync(Client client, string playerToken, PollUpdates request, string lastUpdateId = null, CancellationToken ct = default)
            => client.Send<PollUpdatesResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomUpdatesPoll, $"&player_token={playerToken}"),
                new PollUpdatesRequest(request.FromPlayers, request.FromPlayersIds, request.LastUpdate), ct);
    }
}
