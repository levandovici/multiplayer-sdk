using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Michitai.Multiplayer.Moderation
{
    /// <summary>
    /// Provides methods for screening player-visible text through the
    /// server-side moderation filter. The filter normalizes leetspeak,
    /// confusable scripts, separators and repeated-letter evasion before
    /// matching, so "w e e d" or "w33d" are caught the same as "weed".
    /// </summary>
    public class Moderation
    {
        /// <summary>
        /// Screens text and returns whether it is clean plus a filtered copy.
        /// </summary>
        /// <param name="client">The API client instance.</param>
        /// <param name="text">The text to screen (max 1000 characters).</param>
        /// <param name="ct">Cancellation token for the async operation.</param>
        /// <returns>Response with Clean (no listed word found) and Filtered (masked display text).</returns>
        public static Task<CensorResponse> CensorAsync(Client client, string text, CancellationToken ct = default)
            => client.Send<CensorResponse>(HttpMethod.Post, client.Url(Endpoints.Censor), new CensorRequest(text), ct);
    }
}
