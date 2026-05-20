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
    /// <summary>
    /// Provides static methods for managing matchmaking join requests.
    /// Handles creating lobbies, requesting to join, responding to requests, and checking request status.
    /// Uses Unity's JsonUtility for serialization.
    /// </summary>
    public static class Requests
    {
        /// <summary>
        /// Creates a new matchmaking lobby with the specified configuration.
        /// </summary>
        /// <typeparam name="TPlayerData">The type of player data to include.</typeparam>
        /// <typeparam name="TRules">The type of lobby rules to include.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="matchmakingName">The name for the matchmaking lobby.</param>
        /// <param name="maxPlayers">Maximum number of players allowed (default: 4).</param>
        /// <param name="strictFull">Whether the lobby must be full to start (default: false).</param>
        /// <param name="joinByRequests">Whether players must request to join (default: false).</param>
        /// <param name="hostSwitch">Whether host switching is allowed (default: false).</param>
        /// <param name="canLeaveRoom">Whether players can leave the resulting room (default: false).</param>
        /// <param name="realtimeRoom">Whether the room supports realtime communication (default: false).</param>
        /// <param name="password">Optional password for the lobby.</param>
        /// <param name="playerData">Optional player data to include.</param>
        /// <param name="rules">Optional lobby rules to include.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the matchmaking creation response.</returns>
        public static Task<MatchmakingCreateResponse> CreateMatchmakingLobbyAsync<TPlayerData, TRules>(Client client, string playerToken, string matchmakingName, int maxPlayers = 4, bool strictFull = false,
            bool joinByRequests = false, bool hostSwitch = false, bool canLeaveRoom = false, bool realtimeRoom = false, string password = null, TPlayerData playerData = null, TRules rules = null, CancellationToken ct = default) where TPlayerData : class, new() where TRules : class, new()
            => client.Send<MatchmakingCreateResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingCreate, $"&player_token={playerToken}"),
                new MatchmakingCreateRequest(matchmakingName, maxPlayers, strictFull, joinByRequests, hostSwitch, canLeaveRoom, realtimeRoom, password, JsonUtility.ToJson(playerData), JsonUtility.ToJson(rules)), ct);

        /// <summary>
        /// Requests to join an existing matchmaking lobby.
        /// </summary>
        /// <typeparam name="T">The type of player data to include.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="matchmakingId">The ID of the matchmaking lobby to join.</param>
        /// <param name="playerData">Optional player data to include with the request.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the join request response.</returns>
        public static Task<MatchmakingJoinRequestResponse> RequestToJoinMatchmakingAsync<T>(Client client, string playerToken, string matchmakingId, T playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingJoinRequestResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.MatchmakingRequest, matchmakingId), $"&player_token={playerToken}"), playerData, ct);

        /// <summary>
        /// Responds to a pending join request (approve or reject).
        /// Only the host can respond to join requests.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="requestId">The ID of the join request to respond to.</param>
        /// <param name="action">The action to take (Approve or Reject).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the permission response.</returns>
        public static Task<MatchmakingPermissionResponse> RespondToJoinRequestAsync(Client client, string playerToken, string requestId, EMatchmakingRequestAction action, CancellationToken ct = default)
            => client.Send<MatchmakingPermissionResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.MatchmakingResponse, requestId), $"&player_token={playerToken}"),
                new MatchmakingPermissionRequest(action.ToString().ToLower()), ct);

        /// <summary>
        /// Checks the status of a specific join request.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="requestId">The ID of the join request to check.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Task containing the request status response.</returns>
        public static Task<MatchmakingRequestStatusResponse> CheckJoinRequestStatusAsync(Client client, string playerToken, string requestId, CancellationToken ct = default)
            => client.Send<MatchmakingRequestStatusResponse>(HttpMethod.Get, client.Url(string.Format(Endpoints.MatchmakingRequestStatus, requestId), $"&player_token={playerToken}"), null, ct);
    }
}
