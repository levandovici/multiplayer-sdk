package com.michitai.multiplayer;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.JavaType;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.michitai.multiplayer.errors.Logger;
import com.michitai.multiplayer.errors.ConsoleLogger;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;

/**
 * Main HTTP client for communicating with the Michitai Multiplayer API.
 * Handles authentication, serialization with Jackson, and HTTP requests with comprehensive error handling.
 */
public class Client {
    private final String apiToken;
    private final String apiPrivateToken;
    private final String baseUrl;
    private final HttpClient httpClient;
    private final Logger logger;
    private final ObjectMapper objectMapper;

    /**
     * JSON object mapper configured for camelCase property naming and case-insensitive deserialization.
     */
    public static final ObjectMapper JSON_MAPPER = new ObjectMapper()
        .setPropertyNamingStrategy(com.fasterxml.jackson.databind.PropertyNamingStrategies.SNAKE_CASE)
        .configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false)
        .configure(DeserializationFeature.ACCEPT_EMPTY_ARRAY_AS_NULL_OBJECT, true);

    /**
     * Initializes a new instance of the Client class.
     *
     * @param apiToken Public API token for game identification.
     * @param apiPrivateToken Private API token for admin operations.
     * @param baseUrl Base URL for the API (default: https://api.michitai.com/api).
     * @param logger Optional logger for debugging and error tracking.
     * @throws IllegalArgumentException if apiToken or apiPrivateToken is null.
     */
    public Client(String apiToken, String apiPrivateToken, String baseUrl, Logger logger) {
        if (apiToken == null) {
            throw new IllegalArgumentException("apiToken cannot be null");
        }
        if (apiPrivateToken == null) {
            throw new IllegalArgumentException("apiPrivateToken cannot be null");
        }

        this.apiToken = apiToken;
        this.apiPrivateToken = apiPrivateToken;
        this.baseUrl = baseUrl.endsWith("/") ? baseUrl : baseUrl + "/";
        this.logger = logger != null ? logger : new ConsoleLogger();
        this.objectMapper = JSON_MAPPER;
        this.httpClient = HttpClient.newBuilder()
            .connectTimeout(Duration.ofSeconds(30))
            .build();
    }

    /**
     * Initializes a new instance of the Client class with default base URL.
     */
    public Client(String apiToken, String apiPrivateToken, Logger logger) {
        this(apiToken, apiPrivateToken, "https://api.michitai.com/api", logger);
    }

    /**
     * Generates a URL for public API endpoints.
     *
     * @param endpoint The API endpoint path.
     * @param extra Additional query parameters.
     * @return Complete URL with API token.
     */
    public String url(String endpoint, String extra) {
        return baseUrl + endpoint + "?api_token=" + apiToken + extra;
    }

    /**
     * Generates a URL for public API endpoints without extra parameters.
     */
    public String url(String endpoint) {
        return url(endpoint, "");
    }

    /**
     * Generates a URL for private API endpoints requiring admin access.
     *
     * @param endpoint The API endpoint path.
     * @param extra Additional query parameters.
     * @return Complete URL with API token and private token.
     */
    public String privateUrl(String endpoint, String extra) {
        return baseUrl + endpoint + "?api_token=" + apiToken + "&private_token=" + apiPrivateToken + extra;
    }

    /**
     * Generates a URL for private API endpoints without extra parameters.
     */
    public String privateUrl(String endpoint) {
        return privateUrl(endpoint, "");
    }

    /**
     * Sends an HTTP request to the API and deserializes the response using Jackson.
     *
     * @param <T> The response type, must inherit from ApiResponse.
     * @param method The HTTP method (GET, POST, PUT, DELETE).
     * @param url The complete URL to send the request to.
     * @param body Optional request body to serialize as JSON.
     * @param responseClass The class to deserialize the response into.
     * @return Deserialized API response of type T.
     * @throws IOException if the request fails or deserialization fails.
     */
    public <T extends ApiResponse> T send(String method, String url, Object body, Class<T> responseClass) throws IOException {
        return send(method, url, body, objectMapper.getTypeFactory().constructType(responseClass));
    }

    /**
     * Builds a parameterized JavaType (e.g. PlayerDataResponse&lt;MyData&gt;) for typed deserialization.
     *
     * @param raw The raw response class.
     * @param params The type parameters to bind.
     * @return A JavaType describing the parameterized response type.
     */
    public JavaType parametricType(Class<?> raw, Class<?>... params) {
        return objectMapper.getTypeFactory().constructParametricType(raw, params);
    }

    /**
     * Sends an HTTP request to the API and deserializes the response using Jackson.
     *
     * @param <T> The response type, must inherit from ApiResponse.
     * @param method The HTTP method (GET, POST, PUT, DELETE).
     * @param url The complete URL to send the request to.
     * @param body Optional request body to serialize as JSON.
     * @param responseType The JavaType to deserialize the response into (supports generics).
     * @return Deserialized API response of type T.
     * @throws IOException if the request fails or deserialization fails.
     */
    @SuppressWarnings("unchecked")
    public <T extends ApiResponse> T send(String method, String url, Object body, JavaType responseType) throws IOException {
        try {
            HttpRequest.Builder requestBuilder = HttpRequest.newBuilder()
                .uri(URI.create(url))
                .timeout(Duration.ofSeconds(30));

            if ("GET".equalsIgnoreCase(method)) {
                requestBuilder.GET();
            } else if ("POST".equalsIgnoreCase(method)) {
                requestBuilder.POST(body != null ? 
                    HttpRequest.BodyPublishers.ofString(objectMapper.writeValueAsString(body)) :
                    HttpRequest.BodyPublishers.noBody());
            } else if ("PUT".equalsIgnoreCase(method)) {
                requestBuilder.PUT(body != null ? 
                    HttpRequest.BodyPublishers.ofString(objectMapper.writeValueAsString(body)) :
                    HttpRequest.BodyPublishers.noBody());
            } else if ("DELETE".equalsIgnoreCase(method)) {
                requestBuilder.DELETE();
            }

            if (body != null) {
                requestBuilder.header("Content-Type", "application/json");
            }

            HttpRequest request = requestBuilder.build();
            HttpResponse<String> response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());

            String responseText = response.body();
            logger.log("API Response: " + responseText);

            try {
                T apiResponse = objectMapper.readValue(responseText, responseType);

                if (!apiResponse.isSuccess()) {
                    logger.error("API Error: " + (apiResponse.getError() != null ? apiResponse.getError() : "Unknown error"));
                }

                return apiResponse;
            } catch (Exception e) {
                logger.warn("JSON Deserialization Error. Raw: " + responseText + ". Exception: " + e.getMessage());

                // Return a default error response instead of throwing
                T errorResponse = (T) responseType.getRawClass().getDeclaredConstructor().newInstance();
                errorResponse.setSuccess(false);
                errorResponse.setError("Failed to deserialize response");
                return errorResponse;
            }
        } catch (Exception e) {
            throw new IOException("HTTP request failed: " + e.getMessage(), e);
        }
    }

    /**
     * Sends an HTTP GET request.
     */
    public <T extends ApiResponse> T get(String url, Class<T> responseClass) throws IOException {
        return send("GET", url, null, responseClass);
    }

    /**
     * Sends an HTTP GET request with a parameterized response type.
     */
    public <T extends ApiResponse> T get(String url, JavaType responseType) throws IOException {
        return send("GET", url, null, responseType);
    }

    /**
     * Sends an HTTP POST request.
     */
    public <T extends ApiResponse> T post(String url, Object body, Class<T> responseClass) throws IOException {
        return send("POST", url, body, responseClass);
    }

    /**
     * Sends an HTTP POST request with a parameterized response type.
     */
    public <T extends ApiResponse> T post(String url, Object body, JavaType responseType) throws IOException {
        return send("POST", url, body, responseType);
    }

    /**
     * Sends an HTTP PUT request.
     */
    public <T extends ApiResponse> T put(String url, Object body, Class<T> responseClass) throws IOException {
        return send("PUT", url, body, responseClass);
    }

    /**
     * Sends an HTTP PUT request with a parameterized response type.
     */
    public <T extends ApiResponse> T put(String url, Object body, JavaType responseType) throws IOException {
        return send("PUT", url, body, responseType);
    }

    /**
     * Sends an HTTP DELETE request.
     */
    public <T extends ApiResponse> T delete(String url, Class<T> responseClass) throws IOException {
        return send("DELETE", url, null, responseClass);
    }

    /**
     * Sends an HTTP DELETE request with a parameterized response type.
     */
    public <T extends ApiResponse> T delete(String url, JavaType responseType) throws IOException {
        return send("DELETE", url, null, responseType);
    }

    /**
     * Decodes string-encoded JSON fields inside a raw response tree.
     * Some API responses embed objects as JSON strings (e.g. "rules" in matchmaking
     * lobby lists); this rewrites those nodes in place so they bind as objects.
     *
     * @param root The parsed response root node.
     * @param arrayField Array property containing items to fix (e.g. "lobbies"), or null to fix the root itself.
     * @param fieldNames Field names inside each item to decode when textual.
     */
    public void decodeStringFields(JsonNode root, String arrayField, String... fieldNames) {
        if (root == null) return;
        if (arrayField == null) {
            decodeFieldsOn((ObjectNode) root, fieldNames);
            return;
        }
        JsonNode arr = root.get(arrayField);
        if (arr == null || !arr.isArray()) return;
        for (JsonNode item : arr) {
            if (item instanceof ObjectNode) {
                decodeFieldsOn((ObjectNode) item, fieldNames);
            }
        }
    }

    private void decodeFieldsOn(ObjectNode node, String... fieldNames) {
        for (String field : fieldNames) {
            JsonNode value = node.get(field);
            if (value != null && value.isTextual()) {
                try {
                    node.set(field, objectMapper.readTree(value.asText()));
                } catch (Exception ignored) {
                    // Leave the raw string in place if it isn't valid JSON
                }
            }
        }
    }
}
