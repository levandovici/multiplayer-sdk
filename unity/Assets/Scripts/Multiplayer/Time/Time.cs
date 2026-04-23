using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    public static class Time
    {
        public static Task<ServerTimeResponse> GetServerTime(Multiplayer client, CancellationToken ct = default)
            => client.Send<ServerTimeResponse>(HttpMethod.Get, client.Url(Endpoints.Time), null, ct);

        public static Task<ServerTimeWithOffsetResponse> GetServerTimeWithOffset(Multiplayer client, int utcOffset, CancellationToken ct = default)
            => client.Send<ServerTimeWithOffsetResponse>(HttpMethod.Get, client.Url(Endpoints.Time, $"&utc={utcOffset:+#;-#}"), null, ct);



        internal static DateTimeOffset? ParseUtc(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            if (DateTimeOffset.TryParse(value, out var dto))
                return dto;

            return null;
        }
    }
}
