using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Michitai.Multiplayer.Games;

namespace Michitai.Multiplayer.Games
{
    public class Games
    {
        public static Task<PlayerListResponse> GetAllPlayers(Client client, CancellationToken ct = default)
            => client.Send<PlayerListResponse>(HttpMethod.Get, client.PrivateUrl(Endpoints.GamePlayersList), null, ct);


        public static Task<GameDataResponse<T>> GetGameData<T>(Client client, CancellationToken ct = default) where T : class, new()
            => client.Send<GameDataResponse<T>>(HttpMethod.Get, client.Url(Endpoints.GameDataGameGet), null, ct);

        public static Task<SuccessResponse> UpdateGameData<T>(Client client, T data, CancellationToken ct = default) where T : class, new()
            => client.Send<SuccessResponse>(HttpMethod.Put, client.PrivateUrl(Endpoints.GameDataGameUpdate), data, ct);
    }
}
