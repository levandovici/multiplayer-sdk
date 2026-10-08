using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Michitai.Multiplayer.Errors;

namespace Michitai.Multiplayer
{
    /// <summary>
    /// Main HTTP client for communicating with the Michitai Multiplayer API.
    /// Handles authentication, serialization, and HTTP requests with comprehensive error handling.
    /// </summary>
    public class Client
    {
        private readonly string _apiToken;
        private readonly string _apiPrivateToken;
        private readonly string _baseUrl;
        private readonly HttpClient _http;
        private readonly ILogger? _logger;
        private readonly bool _debugLogging;

        /// <summary>
        /// JSON serialization options used throughout the SDK.
        /// Configured for camelCase property naming and case-insensitive deserialization.
        /// </summary>
        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Initializes a new instance of the Client class.
        /// </summary>
        /// <param name="apiToken">Public API token for game identification.</param>
        /// <param name="apiPrivateToken">Private API token for admin operations (optional — omit it
        /// in shipped/game clients; admin endpoints then throw <see cref="InvalidOperationException"/>).</param>
        /// <param name="baseUrl">Base URL for the API (default: https://api.michitai.com/api).</param>
        /// <param name="logger">Optional logger for debugging and error tracking.</param>
        /// <param name="httpClient">Optional custom HTTP client (default: new client with 30s timeout).</param>
        /// <param name="debugLogging">Log full request/response bodies (default: false — responses can contain credentials, keep off in builds).</param>
        /// <exception cref="ArgumentNullException">Thrown when apiToken is null.</exception>
        public Client(string apiToken, string apiPrivateToken = "", string baseUrl = "https://api.michitai.com/api",
                       ILogger? logger = null, HttpClient? httpClient = null, bool debugLogging = false)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _apiPrivateToken = apiPrivateToken ?? throw new ArgumentNullException(nameof(apiPrivateToken));
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _logger = logger;
            _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _debugLogging = debugLogging;
        }

        /// <summary>
        /// Generates a URL for an API endpoint. Credentials are never placed in
        /// the URL — they are sent as headers by <see cref="Send{T}"/>.
        /// </summary>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="extra">Additional query parameters, with or without a leading '?' or '&amp;'.</param>
        /// <returns>Complete URL without credentials.</returns>
        internal string Url(string endpoint, string extra = "")
            => $"{_baseUrl}{endpoint}{NormalizeQuery(extra)}";

        private static string NormalizeQuery(string extra)
        {
            if (string.IsNullOrEmpty(extra)) return "";
            if (extra.StartsWith("?")) return extra;
            if (extra.StartsWith("&")) return "?" + extra.Substring(1);
            return "?" + extra;
        }

        /// <summary>
        /// Sends an HTTP request to the API and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The response type, must inherit from ApiResponse.</typeparam>
        /// <param name="method">The HTTP method (GET, POST, PUT, DELETE).</param>
        /// <param name="url">The complete URL to send the request to.</param>
        /// <param name="body">Optional request body to serialize as JSON.</param>
        /// <param name="ct">Cancellation token for async operation.</param>
        /// <param name="playerToken">Optional player token for player-scoped endpoints (sent as X-Game-Player-Token).</param>
        /// <param name="includePrivateToken">Send the admin X-Api-Private-Token header (admin endpoints only).</param>
        /// <returns>Deserialized API response of type T.</returns>
        internal async Task<T> Send<T>(HttpMethod method, string url, object? body = null,
            CancellationToken ct = default, string? playerToken = null, bool includePrivateToken = false) where T : ApiResponse, new()
        {
            var req = new HttpRequestMessage(method, url);
            req.Headers.TryAddWithoutValidation("X-Api-Token", _apiToken);
            if (playerToken != null)
            {
                req.Headers.TryAddWithoutValidation("X-Game-Player-Token", playerToken);
            }
            if (includePrivateToken)
            {
                if (string.IsNullOrEmpty(_apiPrivateToken))
                {
                    throw new InvalidOperationException(
                        "Admin operations require apiPrivateToken — construct Client with the private key (server-side tooling only).");
                }
                req.Headers.TryAddWithoutValidation("X-Api-Private-Token", _apiPrivateToken);
            }
            if (body != null)
            {
                string json = JsonSerializer.Serialize(body, JsonOptions);
                req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var res = await _http.SendAsync(req, ct);
            string responseText = await res.Content.ReadAsStringAsync(ct);

            if (_debugLogging)
            {
                _logger?.Log($"API Response: {responseText}");
            }

            try
            {
                var response = JsonSerializer.Deserialize<T>(responseText, JsonOptions) ?? new T();

                if (!response.Success)
                {
                    _logger?.Error($"API Error: {response.Error ?? "Unknown error"}");
                    // Don't throw exception - let caller handle the typed error
                }

                return response;
            }
            catch (JsonException ex)
            {
                _logger?.Warn(_debugLogging
                    ? $"JSON Deserialization Error. Raw: {responseText}. Exception: {ex.Message}"
                    : $"JSON Deserialization Error. Exception: {ex.Message}");

                // Return a default error response instead of throwing
                var errorResponse = new T();
                errorResponse.Success = false;
                errorResponse.Error = "Failed to deserialize response";
                return errorResponse;
            }
        }
    }
}
