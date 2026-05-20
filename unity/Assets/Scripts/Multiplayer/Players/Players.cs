using Michitai.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Michitai.Multiplayer.Players
{
    /// <summary>
    /// Provides methods for player management including registration, authentication,
    /// data management, and administrative operations like banning in Unity.
    /// </summary>
    public class Players
    {
        /// <summary>
        /// Registers a new player with the game.
        /// </summary>
        /// <typeparam name="T">The type of optional player data to include.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="name">The player's display name.</param>
        /// <param name="playerData">Optional initial player data.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the player ID and private key token.</returns>
        public static Task<PlayerRegisterResponse> RegisterPlayer<T>(Client client, string name, T playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<PlayerRegisterResponse>(HttpMethod.Post, client.Url(Endpoints.GamePlayersRegister), new PlayerRegisterRequest(name, JsonUtility.ToJson(playerData)), ct);

        /// <summary>
        /// Authenticates a player using their private token and retrieves their data.
        /// </summary>
        /// <typeparam name="T">The type to deserialize player data into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's private authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the authenticated player information and typed data.</returns>
        public static Task<PlayerAuthResponse<T>> AuthenticatePlayer<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<PlayerAuthResponse<T>>(HttpMethod.Put, client.Url(Endpoints.GamePlayersLogin, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Sends a heartbeat to maintain the player's online status.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's private authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the heartbeat was received.</returns>
        public static Task<PlayerHeartbeatResponse> SendPlayerHeartbeatAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<PlayerHeartbeatResponse>(HttpMethod.Post, client.Url(Endpoints.GamePlayersHeartbeat, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Logs out a player from the game.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's private authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the logout.</returns>
        public static Task<PlayerLogoutResponse> LogoutPlayerAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<PlayerLogoutResponse>(HttpMethod.Post, client.Url(Endpoints.GamePlayersLogout, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Renames a player to a new name (2-50 characters).
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's private authentication token.</param>
        /// <param name="newName">The new name for the player.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the name change.</returns>
        public static Task<PlayerRenameResponse> RenamePlayerAsync(Client client, string playerToken, string newName, CancellationToken ct = default)
            => client.Send<PlayerRenameResponse>(HttpMethod.Put, client.Url(Endpoints.GamePlayersRename, $"&player_token={playerToken}"), new PlayerRenameRequest(newName), ct);

        /// <summary>
        /// Bans a player from the game with a specified duration.
        /// Requires the private API token for authentication.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerId">The ID of the player to ban.</param>
        /// <param name="banDuration">The duration of the ban (Hour, Day, Week, Month, Quarter, Year, Forever).</param>
        /// <param name="banReason">Optional reason for the ban.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the ban details.</returns>
        public static Task<PlayerBanResponse> BanPlayerAsync(Client client, int playerId, EBanTime banDuration, string banReason = null, CancellationToken ct = default)
            => client.Send<PlayerBanResponse>(HttpMethod.Post, client.PrivateUrl(Endpoints.GamePlayersBan), new PlayerBanRequest(playerId, banDuration, banReason), ct);

        /// <summary>
        /// Unbans a previously banned player.
        /// Requires the private API token for authentication.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerId">The ID of the player to unban.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response confirming the player was unbanned.</returns>
        public static Task<PlayerUnbanResponse> UnbanPlayerAsync(Client client, int playerId, CancellationToken ct = default)
            => client.Send<PlayerUnbanResponse>(HttpMethod.Post, client.PrivateUrl(Endpoints.GamePlayersUnban), new PlayerUnbanRequest(playerId), ct);


        /// <summary>
        /// Retrieves a player's data with typed deserialization support.
        /// </summary>
        /// <typeparam name="T">The type to deserialize player data into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's private authentication token.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the player's data deserialized into the specified type.</returns>
        public static Task<PlayerDataResponse<T>> GetPlayerData<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<PlayerDataResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameDataPlayerGet, $"&player_token={playerToken}"), null, ct);

        /// <summary>
        /// Updates a player's data with the provided object.
        /// </summary>
        /// <typeparam name="T">The type of the player data object.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="playerToken">The player's private authentication token.</param>
        /// <param name="data">The player data object to update.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Success response confirming the update.</returns>
        public static Task<SuccessResponse> UpdatePlayerData<T>(Client client, string playerToken, T data, CancellationToken ct = default) where T : class, new()
            => client.Send<SuccessResponse>(HttpMethod.Put, client.Url(Endpoints.GameDataPlayerUpdate, $"&player_token={playerToken}"), data, ct);
    }
}
