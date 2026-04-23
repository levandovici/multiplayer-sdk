using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Leaderboard
{
    public static class Leaderboard
    {
        public static Task<LeaderboardResponse<T>> GetLeaderboardAsync<T>(Multiplayer client, string[] sortBy, int limit = 10, CancellationToken ct = default) where T : class, new()
            => client.Send<LeaderboardResponse<T>>(HttpMethod.Post, client.Url(Endpoints.Leaderboard), new LeaderboardRequest { sort_by = sortBy, limit = limit }, ct);
    }
}
