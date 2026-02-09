using System;
using System.Threading.Tasks;

namespace Career.Web.Services.Caching;

public interface IApiCache
{
    Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl);
}

