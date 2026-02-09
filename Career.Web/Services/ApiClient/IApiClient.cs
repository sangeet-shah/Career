using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace Career.Web.Services.ApiClient;

public interface IApiClient
{
    Task<T> GetAsync<T>(string path, object? query = null, string? cacheKey = null, TimeSpan? ttl = null, CancellationToken ct = default);

    Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct = default);

    Task<TResponse> PostMultipartAsync<TResponse>(string path, MultipartFormDataContent content, CancellationToken ct = default);
}

