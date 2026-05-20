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
        /// <param name="apiPrivateToken">Private API token for admin operations.</param>
        /// <param name="baseUrl">Base URL for the API (default: https://api.michitai.com/api).</param>
        /// <param name="logger">Optional logger for debugging and error tracking.</param>
        /// <param name="httpClient">Optional custom HTTP client (default: new client with 30s timeout).</param>
        /// <exception cref="ArgumentNullException">Thrown when apiToken or apiPrivateToken is null.</exception>
        public Client(string apiToken, string apiPrivateToken, string baseUrl = "https://api.michitai.com/api",
                       ILogger? logger = null, HttpClient? httpClient = null)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _apiPrivateToken = apiPrivateToken ?? throw new ArgumentNullException(nameof(apiPrivateToken));
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _logger = logger;
            _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        }

        /// <summary>
        /// Generates a URL for public API endpoints.
        /// </summary>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="extra">Additional query parameters.</param>
        /// <returns>Complete URL with API token.</returns>
        internal string Url(string endpoint, string extra = "") => $"{_baseUrl}{endpoint}?api_token={_apiToken}{extra}";

        /// <summary>
        /// Generates a URL for private API endpoints requiring admin access.
        /// </summary>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="extra">Additional query parameters.</param>
        /// <returns>Complete URL with API token and private token.</returns>
        internal string PrivateUrl(string endpoint, string extra = "") => $"{_baseUrl}{endpoint}?api_token={_apiToken}&private_token={_apiPrivateToken}{extra}";

        /// <summary>
        /// Sends an HTTP request to the API and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The response type, must inherit from ApiResponse.</typeparam>
        /// <param name="method">The HTTP method (GET, POST, PUT, DELETE).</param>
        /// <param name="url">The complete URL to send the request to.</param>
        /// <param name="body">Optional request body to serialize as JSON.</param>
        /// <param name="ct">Cancellation token for async operation.</param>
        /// <returns>Deserialized API response of type T.</returns>

        internal async Task<T> Send<T>(HttpMethod method, string url, object? body = null, CancellationToken ct = default) where T : ApiResponse, new()
        {
            var req = new HttpRequestMessage(method, url);
            if (body != null)
            {
                string json = JsonSerializer.Serialize(body, JsonOptions);
                req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var res = await _http.SendAsync(req, ct);
            string responseText = await res.Content.ReadAsStringAsync(ct);

            _logger?.Log($"API Response: {responseText}");

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
                _logger?.Warn($"JSON Deserialization Error. Raw: {responseText}. Exception: {ex.Message}");

                // Return a default error response instead of throwing
                var errorResponse = new T();
                errorResponse.Success = false;
                errorResponse.Error = "Failed to deserialize response";
                return errorResponse;
            }
        }
    }
}
