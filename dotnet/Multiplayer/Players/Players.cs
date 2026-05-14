using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Michitai;
using Michitai.Multiplayer;

namespace Michitai.Multiplayer.Players
{
    public class Players
    {
        public static Task<PlayerRegisterResponse> RegisterPlayer<T>(Client client, string name, T? playerData = null, CancellationToken ct = default) where T : class, new()
            => client.Send<PlayerRegisterResponse>(HttpMethod.Post, client.Url(Endpoints.GamePlayersRegister), new PlayerRegisterRequest<T>(name, playerData), ct);

        public  static Task<PlayerAuthResponse<T>> AuthenticatePlayer<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<PlayerAuthResponse<T>>(HttpMethod.Put, client.Url(Endpoints.GamePlayersLogin, $"&player_token={playerToken}"), null, ct);

        public static Task<PlayerHeartbeatResponse> SendPlayerHeartbeatAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<PlayerHeartbeatResponse>(HttpMethod.Post, client.Url(Endpoints.GamePlayersHeartbeat, $"&player_token={playerToken}"), null, ct);

        public static Task<PlayerLogoutResponse> LogoutPlayerAsync(Client client, string playerToken, CancellationToken ct = default)
            => client.Send<PlayerLogoutResponse>(HttpMethod.Post, client.Url(Endpoints.GamePlayersLogout, $"&player_token={playerToken}"), null, ct);

        public static Task<PlayerRenameResponse> RenamePlayerAsync(Client client, string playerToken, string newName, CancellationToken ct = default)
            => client.Send<PlayerRenameResponse>(HttpMethod.Put, client.Url(Endpoints.GamePlayersRename, $"&player_token={playerToken}"), new PlayerRenameRequest(newName), ct);

        public static Task<PlayerBanResponse> BanPlayerAsync(Client client, int playerId, EBanTime banDuration, string? banReason = null, CancellationToken ct = default)
            => client.Send<PlayerBanResponse>(HttpMethod.Post, client.PrivateUrl(Endpoints.GamePlayersBan), new PlayerBanRequest(playerId, banDuration, banReason), ct);

        public static Task<PlayerUnbanResponse> UnbanPlayerAsync(Client client, int playerId, CancellationToken ct = default)
            => client.Send<PlayerUnbanResponse>(HttpMethod.Post, client.PrivateUrl(Endpoints.GamePlayersUnban), new PlayerUnbanRequest(playerId), ct);


        public static Task<PlayerDataResponse<T>> GetPlayerData<T>(Client client, string playerToken, CancellationToken ct = default) where T : class, new()
            => client.Send<PlayerDataResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameDataPlayerGet, $"&player_token={playerToken}"), null, ct);

        public static Task<SuccessResponse> UpdatePlayerData<T>(Client client, string playerToken, T data, CancellationToken ct = default) where T : class, new()
            => client.Send<SuccessResponse>(HttpMethod.Put, client.Url(Endpoints.GameDataPlayerUpdate, $"&player_token={playerToken}"), data, ct);

        /// <summary>
        /// Check if an API response indicates the player is banned
        /// </summary>
        /// <param name="response">The API response</param>
        /// <returns>True if the error indicates the player is banned</returns>
        public static bool IsBanned(ApiResponse response)
        {
            if (!response.Success && response.Error != null)
            {
                return response.Error.Contains("You are banned");
            }
            return false;
        }
    }
}
