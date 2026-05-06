using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    internal class Time
    {
        public static Task<ServerTimeResponse> GetServerTime(Client client, CancellationToken ct = default)
            => client.Send<ServerTimeResponse>(HttpMethod.Get, client.Url(Endpoints.Time), null, ct);
        public static Task<ServerTimeWithOffsetResponse> GetServerTimeWithOffset(Client client, int utcOffset, CancellationToken ct = default)
            => client.Send<ServerTimeWithOffsetResponse>(HttpMethod.Get, client.Url(Endpoints.Time, $"&utc={utcOffset:+#;-#}"), null, ct);
    }
}
