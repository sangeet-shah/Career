using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Services.Stores;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Middleware.Web.Data;

namespace Middleware.Web.Services.HelloBar;

public class HelloBarService : IHelloBarService
{
    private const string HelloBarTable = "NopAdvance_HB_HelloBar";
    private const string StoreMappingTable = "StoreMapping";

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreService _storeService;

    public HelloBarService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager,
        IStoreService storeService)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
        _storeService = storeService;
    }

    public async Task<IList<HelloBars>> GetActiveHelloBarsAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.AllHellobarKey, async () =>
        {
            var centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var centralTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralTimeZone);
            var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;

            using var conn = _db.CreateNop();
            var sql = $@"
SELECT hb.* FROM [{HelloBarTable}] hb
INNER JOIN [{StoreMappingTable}] sm ON sm.EntityId = hb.Id AND sm.EntityName = @EntityName AND sm.StoreId = @StoreId
WHERE hb.Published = 1
  AND (hb.StartDateUtc <= @CentralTime OR hb.StartDateUtc IS NULL)
  AND (@CentralTime <= hb.EndDateUtc OR hb.EndDateUtc IS NULL)
ORDER BY hb.DisplayOrder, hb.StartDateUtc DESC";

            var list = (await conn.QueryAsync<HelloBars>(sql, new { EntityName = nameof(HelloBars), StoreId = storeId, CentralTime = centralTime })).AsList();
            return list;
        });
    }
}
