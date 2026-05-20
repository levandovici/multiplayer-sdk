using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Time
{
    /// <summary>
    /// Provides methods for querying server time information.
    /// </summary>
    internal class Time
    {
        /// <summary>
        /// Retrieves the current server time in UTC.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the server UTC time, timestamp, and readable format.</returns>
        public static Task<ServerTimeResponse> GetServerTime(Client client, CancellationToken ct = default)
            => client.Send<ServerTimeResponse>(HttpMethod.Get, client.Url(Endpoints.Time), null, ct);

        /// <summary>
        /// Retrieves the server time with a specified UTC offset.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="utcOffset">The UTC offset in hours (e.g., 3 for UTC+3, -5 for UTC-5).</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response containing the adjusted time with offset information.</returns>
        public static Task<ServerTimeWithOffsetResponse> GetServerTimeWithOffset(Client client, int utcOffset, CancellationToken ct = default)
            => client.Send<ServerTimeWithOffsetResponse>(HttpMethod.Get, client.Url(Endpoints.Time, $"&utc={utcOffset:+#;-#}"), null, ct);
    }
}
