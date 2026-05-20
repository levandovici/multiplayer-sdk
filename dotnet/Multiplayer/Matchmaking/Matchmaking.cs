using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Matchmaking
{
    /// <summary>
    /// Provides methods for matchmaking lobby management including creation,
    /// joining, player management, and game session initiation.
    /// </summary>
    public class Matchmaking
    {
        /// <summary>
        /// Retrieves a list of available matchmaking lobbies.
        /// </summary>
        /// <typeparam name="T">The type to deserialize lobby rules into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="search">Optional search term to filter lobbies.</param>
        /// <param name="limit">Optional limit on the number of results.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the list of matchmaking lobbies.</returns>
        public static Task<MatchmakingListResponse<T>> GetMatchmakingLobbiesAsync<T>(Client client, string? search = null, int? limit = null, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingListResponse<T>>(HttpMethod.Post, client.Url(Endpoints.MatchmakingList), new MatchmakingListRequest { Search = search, Limit = limit }, ct);

        /// <summary>
        /// Creates a new matchmaking lobby with specified configuration.
        /// </summary>
        /// <typeparam name="TPlayerData">The type of player data for the host.</typeparam>
        /// <typeparam name="TRules">The type of game rules for the lobby.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's player token.</param>
        /// <param name="matchmakingName">The name of the matchmaking lobby.</param>
        /// <param name="maxPlayers">Maximum number of players allowed (default: 4).</param>
        /// <param name="strictFull">Whether the lobby must be full to start (default: false).</param>
        /// <param name="hostSwitch">Whether host switching is allowed (default: false).</param>
        /// <param name="canLeaveRoom">Whether players can leave the resulting room (default: false).</param>
        /// <param name="realtimeRoom">Whether the resulting room supports realtime communication (default: false).</param>
        /// <param name="password">Optional password for the lobby.</param>
        /// <param name="playerData">Optional player data for the host.</param>
        /// <param name="rules">Optional game rules for the lobby.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the matchmaking lobby ID.</returns>
        public static Task<MatchmakingCreateResponse> CreateMatchmakingLobbyAsync<TPlayerData, TRules>(Client client, string playerToken, string matchmakingName, int maxPlayers = 4, bool strictFull = false,
            bool hostSwitch = false, bool canLeaveRoom = false, bool realtimeRoom = false, string? password = null, TPlayerData? playerData = null, TRules? rules = null, CancellationToken ct = default) where TPlayerData : class, new() where TRules : class, new()
            => client.Send<MatchmakingCreateResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingCreate, $"&player_token={playerToken}"),
                new MatchmakingCreateRequest<TPlayerData, TRules>(matchmakingName, maxPlayers, strictFull, false, hostSwitch, canLeaveRoom, realtimeRoom, password, playerData, rules), ct);

        /// <summary>
        /// Gets the current status of the player's matchmaking lobby.
        /// </summary>
        /// <typeparam name="T">The type to deserialize lobby rules into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the current lobby status and information.</returns>
        public static Task<MatchmakingCurrentResponse<T>> GetCurrentMatchmakingStatusAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingCurrentResponse<T>>(HttpMethod.Get, client.Url(Endpoints.MatchmakingCurrent, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Joins a matchmaking lobby directly (without approval).
        /// Only works if the lobby doesn't require host approval.
        /// </summary>
        /// <typeparam name="T">The type of player data.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="matchmakingId">The ID of the matchmaking lobby to join.</param>
        /// <param name="playerData">Optional player data to include when joining.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the player joined the lobby.</returns>
        public static Task<MatchmakingDirectJoinResponse> JoinMatchmakingDirectlyAsync<T>(Client client, string playerToken, string matchmakingId, T? playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingDirectJoinResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.MatchmakingJoin, matchmakingId), $"&player_token={playerToken}"), playerData, ct);

        /// <summary>
        /// Leaves the current matchmaking lobby.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the player left the lobby.</returns>
        public static Task<MatchmakingLeaveResponse> LeaveMatchmakingAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<MatchmakingLeaveResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingLeave, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Gets the list of players in the current matchmaking lobby.
        /// </summary>
        /// <typeparam name="T">The type to deserialize player data into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the list of players in the lobby.</returns>
        public static Task<MatchmakingPlayersResponse<T>> GetMatchmakingPlayersAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<MatchmakingPlayersResponse<T>>(HttpMethod.Get, client.Url(Endpoints.MatchmakingPlayers, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Sends a heartbeat to maintain the player's presence in the matchmaking lobby.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the heartbeat was received.</returns>
        public static Task<MatchmakingHeartbeatResponse> SendMatchmakingHeartbeatAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<MatchmakingHeartbeatResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingHeartbeat, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Removes the current matchmaking lobby (host only).
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the lobby was removed.</returns>
        public static Task<MatchmakingRemoveResponse> RemoveMatchmakingLobbyAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<MatchmakingRemoveResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingRemove, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Starts the game from the current matchmaking lobby and creates a game room.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the created room ID.</returns>
        public static Task<MatchmakingStartResponse> StartGameFromMatchmakingAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<MatchmakingStartResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingStart, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Stops the current matchmaking lobby (host only).
        /// Cannot be called after the game has started.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Success response confirming the lobby was stopped.</returns>
        public static Task<SuccessResponse> StopMatchmakingAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<SuccessResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingStop, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Kicks a player from the matchmaking lobby (host only).
        /// Cannot kick players after the game has started.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's authentication token.</param>
        /// <param name="playerId">The ID of the player to kick.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the player was kicked.</returns>
        public static Task<MatchmakingKickResponse> KickPlayerAsync(Client client, string playerToken, int playerId, CancellationToken ct = default)
            => client.Send<MatchmakingKickResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingKick, $"&player_token={playerToken}"), new MatchmakingKickRequest { Player_id = playerId }, ct);

        /// <summary>
        /// Updates the password for the matchmaking lobby (host only).
        /// Cannot change password after the game has started.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's authentication token.</param>
        /// <param name="password">New password, or null to remove the password.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Success response confirming the password was updated.</returns>
        public static Task<SuccessResponse> UpdateMatchmakingPasswordAsync(Client client, string playerToken, string? password = null, CancellationToken ct = default)
            => client.Send<SuccessResponse>(HttpMethod.Post, client.Url(Endpoints.MatchmakingPassword, $"&player_token={playerToken}"), new MatchmakingPasswordUpdateRequest(password), ct);
    }
}
