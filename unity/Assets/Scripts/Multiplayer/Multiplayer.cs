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
    public class Multiplayer
    {
        private readonly string _apiToken;
        private readonly string _apiPrivateToken;
        private readonly string _baseUrl;
        private readonly HttpClient _http;
        private readonly Errors.ILogger _logger;
        private readonly bool _useUnityFormat;



        public Multiplayer(string apiToken, string apiPrivateToken, string baseUrl = "https://api.michitai.com/api",
                       Errors.ILogger logger = null, HttpClient httpClient = null, bool useUnityFormat = true)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _apiPrivateToken = apiPrivateToken ?? throw new ArgumentNullException(nameof(apiPrivateToken));
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            _logger = logger ?? new ConsoleLogger();
            _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _useUnityFormat = useUnityFormat;
        }

        internal string Url(string endpoint, string extra = "")
        {
            string format = _useUnityFormat ? "unity" : "json";
            return $"{_baseUrl}{endpoint}?api_token={_apiToken}&format={format}{extra}";
        }

        internal string PrivateUrl(string endpoint, string extra = "")
        {
            string format = _useUnityFormat ? "unity" : "json";
            return $"{_baseUrl}{endpoint}?api_token={_apiToken}&private_token={_apiPrivateToken}&format={format}{extra}";
        }

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
