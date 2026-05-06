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
    public class Client
    {
        private readonly string _apiToken;
        private readonly string _apiPrivateToken;
        private readonly string _baseUrl;
        private readonly HttpClient _http;
        private readonly ILogger? _logger;



        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };



        public Client(string apiToken, string apiPrivateToken, string baseUrl = "https://api.michitai.com/api",
                       ILogger? logger = null, HttpClient? httpClient = null)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _apiPrivateToken = apiPrivateToken ?? throw new ArgumentNullException(nameof(apiPrivateToken));
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _logger = logger;
            _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        }



        internal string Url(string endpoint, string extra = "") => $"{_baseUrl}{endpoint}?api_token={_apiToken}{extra}";

        internal string PrivateUrl(string endpoint, string extra = "") => $"{_baseUrl}{endpoint}?api_token={_apiToken}&private_token={_apiPrivateToken}{extra}";

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
