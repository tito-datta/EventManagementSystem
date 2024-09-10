using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Utilities.Http;

public class HttpClientWrapper : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpClientWrapper> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpClientWrapper(HttpClient httpClient,
                             ILogger<HttpClientWrapper> logger,
                             JsonSerializerOptions jsonOptions = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions();
    }

    public Task<T> GetAsync<T>(string path,
                               string authToken = null,
                               Dictionary<string, string> queryParams = null,
                               CancellationToken cancellationToken = default) =>
        SendRequestAsync<T>(HttpMethod.Get, path, null, authToken, queryParams, cancellationToken);

    public Task<T> PostAsync<T>(string path,
                                object payload,
                                string authToken = null,
                                CancellationToken cancellationToken = default) =>
        SendRequestAsync<T>(HttpMethod.Post, path, payload, authToken, null, cancellationToken);

    public Task<T> PutAsync<T>(string path,
                               object payload,
                               string authToken = null,
                               CancellationToken cancellationToken = default) =>
        SendRequestAsync<T>(HttpMethod.Put, path, payload, authToken, null, cancellationToken);

    public Task<T> DeleteAsync<T>(string path,
                                  string authToken = null,
                                  CancellationToken cancellationToken = default) =>
        SendRequestAsync<T>(HttpMethod.Delete, path, null, authToken, null, cancellationToken);

    private async Task<T> SendRequestAsync<T>(HttpMethod method,
                                              string path,
                                              object payload,
                                              string authToken,
                                              Dictionary<string, string> queryParams,
                                              CancellationToken cancellationToken)
    {
        try
        {
            var request = new HttpRequestMessage(method, BuildUri(path, queryParams));

            if (payload != null)
            {
                var jsonPayload = JsonSerializer.Serialize(payload, _jsonOptions);
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrEmpty(authToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }

            _logger.LogInformation("Sending {0} request to {1}", method, path);

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            await EnsureSuccessStatusCodeWithLoggingAsync(response);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("Received response: {0}", content);

            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending {0} request to {1}", method, path);
            throw;
        }
    }

    private Uri BuildUri(string path,
                         Dictionary<string, string> queryParams)
    {
        var uriBuilder = new UriBuilder(_httpClient.BaseAddress)
        {
            Path = path
        };

        if (queryParams != null && queryParams.Count > 0)
        {
            var query = new StringBuilder();
            foreach (var param in queryParams)
            {
                if (query.Length > 0) query.Append('&');
                query.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}");
            }
            uriBuilder.Query = query.ToString();
        }

        return uriBuilder.Uri;
    }

    private async Task EnsureSuccessStatusCodeWithLoggingAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError("HTTP request failed with status code {0}. Response content: {1}", response.StatusCode, content);
            response.EnsureSuccessStatusCode(); // This will throw an exception
        }
    }

    public void Dispose() => _httpClient.Dispose();
}