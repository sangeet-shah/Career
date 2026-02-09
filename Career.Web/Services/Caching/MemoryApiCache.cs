using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Career.Web.Services.Caching;

public class MemoryApiCache : IApiCache
{
    private readonly IMemoryCache _cache;

    public MemoryApiCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl)
    {
        if (_cache.TryGetValue(key, out T value))
            return value;

        value = await factory();
        _cache.Set(key, value, ttl);
        return value;
    }
}

