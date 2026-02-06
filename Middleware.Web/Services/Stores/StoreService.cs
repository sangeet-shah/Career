using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.Stores;
using Dapper;
using Middleware.Web.Data;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Stores;

public class StoreService : IStoreService
{
    private const string StoreTable = "Store";
    private const string FMStoreTable = "FM_Store";

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    public StoreService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    public async Task<Store> GetCurrentStoreAsync()
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CurrentStoreCacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $@"
SELECT s.* FROM [{StoreTable}] s
INNER JOIN [{FMStoreTable}] fs ON s.Id = fs.StoreId
WHERE fs.Alias = @Alias";
            return await conn.QueryFirstOrDefaultAsync<Store>(sql, new { Alias = nameof(WebsiteEnum.FMUSA) });
        });
    }
}
