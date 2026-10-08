using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Michitai.Multiplayer.Errors;

namespace Michitai.Multiplayer
{
    /// <summary>
    /// Main HTTP client for communicating with the Michitai Multiplayer API in Unity.
    /// Handles authentication, serialization with JsonUtility, and HTTP requests with comprehensive error handling.
    /// Supports Unity-specific JSON formatting for compatibility with Unity's serialization system.
    /// </summary>
    public class Client
    {
        private readonly string _apiToken;
        private readonly string _apiPrivateToken;
        private readonly string _baseUrl;
        private readonly HttpClient _http;
        private readonly Errors.ILogger _logger;
        private readonly bool _useUnityFormat;
        private readonly bool _debugLogging;

        /// <summary>
        /// Initializes a new instance of the Client class for Unity.
        /// </summary>
        /// <param name="apiToken">Public API token for game identification.</param>
        /// <param name="apiPrivateToken">Private API token for admin operations (optional — omit it
        /// in shipped/game clients; admin endpoints then throw InvalidOperationException).</param>
        /// <param name="baseUrl">Base URL for the API (default: https://api.michitai.com/api).</param>
        /// <param name="logger">Optional logger for debugging and error tracking (default: ConsoleLogger).</param>
        /// <param name="httpClient">Optional custom HTTP client (default: new client with 30s timeout).</param>
        /// <param name="useUnityFormat">Whether to use Unity-specific JSON formatting (default: true).</param>
        /// <param name="debugLogging">Log full request/response bodies (default: false — responses can contain credentials, keep off in builds).</param>
        /// <exception cref="ArgumentNullException">Thrown when apiToken is null.</exception>
        public Client(string apiToken, string apiPrivateToken = "", string baseUrl = "https://api.michitai.com/api",
                       Errors.ILogger logger = null, HttpClient httpClient = null, bool useUnityFormat = true,
                       bool debugLogging = false)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _apiPrivateToken = apiPrivateToken ?? throw new ArgumentNullException(nameof(apiPrivateToken));
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _logger = logger ?? new ConsoleLogger();
            _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _useUnityFormat = useUnityFormat;
            _debugLogging = debugLogging;
        }

        /// <summary>
        /// Generates a URL for an API endpoint with Unity format support.
        /// Credentials are never placed in the URL — they are sent as headers by <see cref="Send{T}"/>.
        /// </summary>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="extra">Additional query parameters, with or without a leading '?' or '&amp;'.</param>
        /// <returns>Complete URL with format parameter, without credentials.</returns>
        internal string Url(string endpoint, string extra = "")
        {
            string format = _useUnityFormat ? "unity" : "json";
            string normalized = extra.StartsWith("?") || extra.StartsWith("&")
                ? extra.Substring(1)
                : extra;
            return $"{_baseUrl}{endpoint}?format={format}{(normalized.Length > 0 ? "&" + normalized : "")}";
        }

        /// <summary>
        /// Sends an HTTP request to the API and deserializes the response using JsonUtility.
        /// </summary>
        /// <typeparam name="T">The response type, must inherit from ApiResponse.</typeparam>
        /// <param name="method">The HTTP method (GET, POST, PUT, DELETE).</param>
        /// <param name="url">The complete URL to send the request to.</param>
        /// <param name="body">Optional request body to serialize as JSON.</param>
        /// <param name="ct">Cancellation token for async operation.</param>
        /// <param name="playerToken">Optional player token for player-scoped endpoints (sent as X-Game-Player-Token).</param>
        /// <param name="includePrivateToken">Send the admin X-Api-Private-Token header (admin endpoints only).</param>
        /// <returns>Deserialized API response of type T.</returns>
        internal async Task<T> Send<T>(HttpMethod method, string url, object body = null,
            CancellationToken ct = default, string playerToken = null, bool includePrivateToken = false) where T : ApiResponse, new()
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
                string jsonBody = JsonUtility.ToJson(body);
                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            var res = await _http.SendAsync(req, ct);
            string responseText = await res.Content.ReadAsStringAsync();

            if (_debugLogging)
            {
                _logger.Log($"API Response: {responseText}");
            }

            try
            {
                var response = JsonUtility.FromJson<T>(responseText) ?? new T();

                if (!response.success)
                {
                    _logger.Error($"API Error: {response.error ?? "Unknown error"}");
                    // Don't throw exception - let caller handle the typed error
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.Warn(_debugLogging
                    ? $"JSON Deserialization Error. Raw: {responseText}. Exception: {ex.Message}"
                    : $"JSON Deserialization Error. Exception: {ex.Message}");

                // Return a default error response instead of throwing
                var errorResponse = new T();
                errorResponse.success = false;
                errorResponse.error = "Failed to deserialize response";
                return errorResponse;
            }
        }
    }
}
