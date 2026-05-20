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

        /// <summary>
        /// Initializes a new instance of the Client class for Unity.
        /// </summary>
        /// <param name="apiToken">Public API token for game identification.</param>
        /// <param name="apiPrivateToken">Private API token for admin operations.</param>
        /// <param name="baseUrl">Base URL for the API (default: https://api.michitai.com/api).</param>
        /// <param name="logger">Optional logger for debugging and error tracking (default: ConsoleLogger).</param>
        /// <param name="httpClient">Optional custom HTTP client (default: new client with 30s timeout).</param>
        /// <param name="useUnityFormat">Whether to use Unity-specific JSON formatting (default: true).</param>
        /// <exception cref="ArgumentNullException">Thrown when apiToken or apiPrivateToken is null.</exception>
        public Client(string apiToken, string apiPrivateToken, string baseUrl = "https://api.michitai.com/api",
                       Errors.ILogger logger = null, HttpClient httpClient = null, bool useUnityFormat = true)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _apiPrivateToken = apiPrivateToken ?? throw new ArgumentNullException(nameof(apiPrivateToken));
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _logger = logger ?? new ConsoleLogger();
            _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _useUnityFormat = useUnityFormat;
        }

        /// <summary>
        /// Generates a URL for public API endpoints with Unity format support.
        /// </summary>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="extra">Additional query parameters.</param>
        /// <returns>Complete URL with API token and format parameter.</returns>
        internal string Url(string endpoint, string extra = "")
        {
            string format = _useUnityFormat ? "unity" : "json";
            return $"{_baseUrl}{endpoint}?api_token={_apiToken}&format={format}{extra}";
        }

        /// <summary>
        /// Generates a URL for private API endpoints requiring admin access with Unity format support.
        /// </summary>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="extra">Additional query parameters.</param>
        /// <returns>Complete URL with API token, private token, and format parameter.</returns>
        internal string PrivateUrl(string endpoint, string extra = "")
        {
            string format = _useUnityFormat ? "unity" : "json";
            return $"{_baseUrl}{endpoint}?api_token={_apiToken}&private_token={_apiPrivateToken}&format={format}{extra}";
        }

        /// <summary>
        /// Sends an HTTP request to the API and deserializes the response using JsonUtility.
        /// </summary>
        /// <typeparam name="T">The response type, must inherit from ApiResponse.</typeparam>
        /// <param name="method">The HTTP method (GET, POST, PUT, DELETE).</param>
        /// <param name="url">The complete URL to send the request to.</param>
        /// <param name="body">Optional request body to serialize as JSON.</param>
        /// <param name="ct">Cancellation token for async operation.</param>
        /// <returns>Deserialized API response of type T.</returns>
        internal async Task<T> Send<T>(HttpMethod method, string url, object body = null, CancellationToken ct = default) where T : ApiResponse, new()
        {
            var req = new HttpRequestMessage(method, url);

            if (body != null)
            {
                string jsonBody = JsonUtility.ToJson(body);
                req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            var res = await _http.SendAsync(req, ct);
            string responseText = await res.Content.ReadAsStringAsync();

            _logger.Log($"API Response: {responseText}");

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
                _logger.Warn($"JSON Deserialization Error. Raw: {responseText}. Exception: {ex.Message}");

                // Return a default error response instead of throwing
                var errorResponse = new T();
                errorResponse.success = false;
                errorResponse.error = "Failed to deserialize response";
                return errorResponse;
            }
        }
    }
}
