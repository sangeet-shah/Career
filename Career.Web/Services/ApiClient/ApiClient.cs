using Career.Web.Services.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Career.Web.Services.ApiClient;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IApiCache _cache;
    private readonly ILogger<ApiClient> _logger;
    private static readonly TimeSpan DefaultCacheTtl = TimeSpan.FromMinutes(30);
    private static readonly JsonSerializerOptions DefaultJsonOptions = new(JsonSerializerDefaults.Web);
    private readonly string? _baseUrl;

    public ApiClient(HttpClient httpClient, IApiCache cache, IConfiguration configuration, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _baseUrl = configuration["Api:BaseUrl"]?.TrimEnd('/') ?? httpClient.BaseAddress?.ToString().TrimEnd('/');

    }

    public async Task<T> GetAsync<T>(string path, object? query = null, string? cacheKey = null, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        try
        {
            var url = AppendQueryString(path, BuildQueryString(query));
            var resolvedCacheKey = string.IsNullOrWhiteSpace(cacheKey)
                ? $"api-client:get:{url}"
                : $"api-client:get:{cacheKey}";
            var cacheTtl = ttl ?? DefaultCacheTtl;

            return await _cache.GetOrAddAsync(resolvedCacheKey, async () =>
            {
                var response = await _httpClient.GetAsync(BuildRequestUrl(url), ct);
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return default; // or null, or new T()

                response.EnsureSuccessStatusCode();
                var result = await ReadJsonOrDefaultAsync<T>(response.Content, ct);
                return result;
            }, cacheTtl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API GET failed: {Path}", path);
            return default;
        }
    }

    public async Task<string> GetStringAsync(string path, object? query = null, CancellationToken ct = default)
    {
        try
        {
            var url = AppendQueryString(path, BuildQueryString(query));
            var response = await _httpClient.GetAsync(BuildRequestUrl(url), ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API GET string failed: {Path}", path);
            return null;
        }
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct = default)
    {
        try
        {
            var url = AppendQueryString(path, BuildQueryString(null));
            var requestUrl = BuildRequestUrl(url);

            // create request so we can log/inspect headers and content if needed
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = JsonContent.Create(body)
            };

            _logger.LogDebug("Sending POST {Url} with headers: {Headers}", requestUrl, request.Headers.ToString());

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("API POST returned non-success: {Status} {Path} - Response: {Response}", response.StatusCode, path, content);
                response.EnsureSuccessStatusCode(); // will throw
            }

            var result = await ReadJsonOrDefaultAsync<TResponse>(response.Content, ct);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API POST failed: {Path}", path);
            return default;
        }
    }

    public async Task<TResponse> PostMultipartAsync<TResponse>(string path, MultipartFormDataContent content, CancellationToken ct = default)
    {
        try
        {
            var url = AppendQueryString(path, BuildQueryString(null));
            var response = await _httpClient.PostAsync(BuildRequestUrl(url), content, ct);
            response.EnsureSuccessStatusCode();

            var result = await ReadJsonOrDefaultAsync<TResponse>(response.Content, ct);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API POST multipart failed: {Path}", path);
            return default;
        }
    }

    private static string BuildQueryString(object? query)
    {
        var props = query == null
            ? Array.Empty<(string Name, object? Value)>()
            : query.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead)
                .Select(p => (Name: p.Name, Value: p.GetValue(query)))
                .ToArray();

        var pairs = props
            .Where(p => p.Value != null)
            .Select(p => $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.Value!.ToString()!)}")
            .ToList();

        return pairs.Count == 0 ? string.Empty : "?" + string.Join("&", pairs);
    }

    private string BuildRequestUrl(string relativeUrl)
    {
        if (string.IsNullOrWhiteSpace(_baseUrl))
            return relativeUrl;

        return $"{_baseUrl}/{relativeUrl.TrimStart('/')}";
    }

    private static string AppendQueryString(string path, string queryString)
    {
        if (string.IsNullOrEmpty(queryString))
            return path;

        if (path.Contains("?", StringComparison.Ordinal))
            return path + "&" + queryString.TrimStart('?');

        return path + queryString;
    }

    private static async Task<T> ReadJsonOrDefaultAsync<T>(HttpContent content, CancellationToken ct)
    {
        if (content == null)
            return default;

        if (content.Headers.ContentLength == 0)
            return default;

        var payload = await content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(payload))
            return default;

        return JsonSerializer.Deserialize<T>(payload, DefaultJsonOptions);
    }   
}

