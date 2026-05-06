using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    public class Leaderboard
    {
        public static Task<LeaderboardResponse<T>> GetLeaderboardAsync<T>(Client client, string[] sortBy, int limit = 10, CancellationToken ct = default) where T : class, new()
            => client.Send<LeaderboardResponse<T>>(HttpMethod.Post, client.Url(Endpoints.Leaderboard), new LeaderboardRequest(sortBy, limit), ct);
    }
}
