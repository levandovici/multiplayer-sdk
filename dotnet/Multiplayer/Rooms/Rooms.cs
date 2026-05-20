using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Rooms
{
    /// <summary>
    /// Provides methods for game room management including creation, joining,
    /// player management, actions, updates, and administrative operations.
    /// </summary>
    public class Rooms
    {
        /// <summary>
        /// Creates a new game room with specified configuration.
        /// </summary>
        /// <typeparam name="TPlayerData">The type of player data for the host.</typeparam>
        /// <typeparam name="TRules">The type of game rules for the room.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's player token.</param>
        /// <param name="roomName">The name of the game room.</param>
        /// <param name="maxPlayers">Maximum number of players allowed (default: 4).</param>
        /// <param name="password">Optional password for the room.</param>
        /// <param name="hostSwitch">Whether host switching is allowed (default: false).</param>
        /// <param name="realtime">Whether the room supports realtime communication (default: false).</param>
        /// <param name="playerData">Optional player data for the host.</param>
        /// <param name="rules">Optional game rules for the room.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the created room ID.</returns>
        public static Task<RoomCreateResponse> CreateRoomAsync<TPlayerData, TRules>(Client client, string playerToken, string roomName, int maxPlayers = 4,
           string? password = null, bool hostSwitch = false, bool realtime = false, TPlayerData? playerData = null, TRules? rules = null, CancellationToken ct = default) where TPlayerData : class, new() where TRules : class, new()
           => client.Send<RoomCreateResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomCreate, $"&player_token={playerToken}"),
               new RoomCreateRequest<TPlayerData, TRules>(roomName, maxPlayers, password, hostSwitch, realtime, playerData, rules), ct);

        /// <summary>
        /// Retrieves a list of available game rooms.
        /// </summary>
        /// <typeparam name="T">The type to deserialize room rules into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="search">Optional search term to filter rooms.</param>
        /// <param name="limit">Optional limit on the number of results.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the list of game rooms.</returns>
        public static Task<RoomListResponse<T>> GetRoomsAsync<T>(Client client, string? search = null, int? limit = null, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomListResponse<T>>(HttpMethod.Post, client.Url(Endpoints.GameRoomList), new RoomListRequest { Search = search, Limit = limit }, ct);

        /// <summary>
        /// Joins an existing game room.
        /// </summary>
        /// <typeparam name="T">The type of player data.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="roomId">The ID of the room to join.</param>
        /// <param name="password">Optional password if the room is password-protected.</param>
        /// <param name="playerData">Optional player data to include when joining.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the player joined the room.</returns>
        public static Task<RoomJoinResponse> JoinRoomAsync<T>(Client client, string playerToken, string roomId, string? password = null, T? playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomJoinResponse>(HttpMethod.Post, client.Url(string.Format(Endpoints.GameRoomJoin, roomId), $"&player_token={playerToken}"),
                (password != null || playerData != null) ? new RoomJoinRequest<T>(password, playerData) : null, ct);

        /// <summary>
        /// Leaves the current game room.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the player left the room.</returns>
        public static Task<RoomLeaveResponse> LeaveRoomAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<RoomLeaveResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomLeave, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Gets the list of players in the current game room.
        /// </summary>
        /// <typeparam name="T">The type to deserialize player data into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the list of players in the room.</returns>
        public static Task<RoomPlayersResponse<T>> GetRoomPlayersAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<RoomPlayersResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomPlayers, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Sends a heartbeat to maintain the player's presence in the game room.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the heartbeat was received.</returns>
        public static Task<HeartbeatResponse> SendRoomHeartbeatAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<HeartbeatResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomHeartbeat, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Gets comprehensive information about the current game room including players and pending actions.
        /// </summary>
        /// <typeparam name="T">The type to deserialize room rules into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing detailed room information.</returns>
        public  static Task<CurrentRoomResponse<T>> GetCurrentRoomAsync<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<CurrentRoomResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameRoomCurrent, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Stops the current game room and removes all associated data (host only).
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Success response confirming the room was stopped.</returns>
        public static Task<SuccessResponse> StopRoomAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<SuccessResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomStop, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Kicks a player from the game room (host only).
        /// Cannot kick yourself.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's authentication token.</param>
        /// <param name="playerId">The ID of the player to kick.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the player was kicked.</returns>
        public static Task<RoomKickResponse> KickPlayerAsync(Client client, string playerToken, int playerId, CancellationToken ct = default)
            => client.Send<RoomKickResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomKick, $"&player_token={playerToken}"), new RoomKickRequest { Player_id = playerId }, ct);

        /// <summary>
        /// Updates the password for the game room (host only).
        /// Use empty string to remove the password.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The host's authentication token.</param>
        /// <param name="password">New password, or null to remove the password.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Success response confirming the password was updated.</returns>
        public static Task<SuccessResponse> UpdateRoomPasswordAsync(Client client, string playerToken, string? password = null, CancellationToken ct = default)
            => client.Send<SuccessResponse>(HttpMethod.Post, client.Url(Endpoints.GameRoomPassword, $"&player_token={playerToken}"), new RoomPasswordUpdateRequest(password), ct);
    }
}
