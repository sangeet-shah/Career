using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.Stores;
using Dapper;
using Middleware.Web.Data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Stores;

public class StoreService : IStoreService
{
    private const string StoreTable = "Store";
    private const string FMStoreTable = "FM_Store";

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StoreService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Store> GetCurrentStoreAsync()
    {
        var alias = _httpContextAccessor.HttpContext?.Items[Infrastructure.StoreAliasMiddleware.StoreAliasItemsKey]?.ToString();
        if (string.IsNullOrWhiteSpace(alias))
            alias = nameof(WebsiteEnum.FMUSA);

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CurrentStoreCacheKey, alias);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $@"
SELECT s.* FROM [{StoreTable}] s
INNER JOIN [{FMStoreTable}] fs ON s.Id = fs.StoreId
WHERE fs.Alias = @Alias";
            return await conn.QueryFirstOrDefaultAsync<Store>(sql, new { Alias = alias });
        });
    }
}
