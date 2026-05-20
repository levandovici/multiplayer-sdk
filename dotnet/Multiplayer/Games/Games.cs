using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Michitai.Multiplayer.Games;

namespace Michitai.Multiplayer.Games
{
    /// <summary>
    /// Provides methods for game-level operations including player listings and global game data management.
    /// </summary>
    public class Games
    {
        /// <summary>
        /// Retrieves a list of all players in the game.
        /// Requires the private API token for authentication.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the list of all players with their basic information.</returns>
        public static Task<PlayerListResponse> GetAllPlayers(Client client, CancellationToken ct = default)
            => client.Send<PlayerListResponse>(HttpMethod.Get, client.PrivateUrl(Endpoints.GamePlayersList), null, ct);


        /// <summary>
        /// Retrieves global game data with typed deserialization support.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the game data into.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the game data deserialized into the specified type.</returns>
        public static Task<GameDataResponse<T>> GetGameData<T>(Client client, CancellationToken ct = default) where T : class, new()
            => client.Send<GameDataResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameDataGameGet), null, ct);

        /// <summary>
        /// Updates global game data with the provided object.
        /// Requires the private API token for authentication.
        /// </summary>
        /// <typeparam name="T">The type of the game data object.</typeparam>
        /// <param name="client">The API client instance.</param>
        /// <param name="data">The game data object to update.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Success response confirming the update.</returns>
        public static Task<SuccessResponse> UpdateGameData<T>(Client client, T data, CancellationToken ct = default) where T : class, new()
            => client.Send<SuccessResponse>(HttpMethod.Put, client.PrivateUrl(Endpoints.GameDataGameUpdate), data, ct);
    }
}
