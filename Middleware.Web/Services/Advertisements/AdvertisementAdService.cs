using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Advertisements;
using Career.Data.Domains.Common;
using Middleware.Web.Services.Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Middleware.Web.Data;

namespace Middleware.Web.Services.Advertisements;

public class AdvertisementService : IAdvertisementService
{
    private const string AdTable = "FM_Advertisement";

    #region Fields

    private readonly DbConnectionFactory _db;
    private readonly ICommonService _commonService;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public AdvertisementService(DbConnectionFactory db,
        ICommonService commonService,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _commonService = commonService;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    public async Task<IList<Advertisement>> GetActiveAdvertisementByStoreAndTypeAsync(int adTypeId, int storeId = 0)
    {
        return await _staticCacheManager.GetAsync(CacheKeys.AllAdvertisementKey, async () =>
        {
            var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);

            using var conn = _db.CreateNop();
            var sql = $@"
SELECT Id, Title, StoreId, Url, StartDateUtc, EndDateUtc, AdTypeId, Published
FROM [{AdTable}]
WHERE Published = 1
  AND (StartDateUtc <= @CurrentDate OR StartDateUtc IS NULL)
  AND (@CurrentDate <= EndDateUtc OR EndDateUtc IS NULL)
  AND StoreId = @StoreId AND AdTypeId = @AdTypeId
ORDER BY EndDateUtc";

            var list = (await conn.QueryAsync<Advertisement>(sql, new
            {
                CurrentDate = currentDate,
                StoreId = storeId,
                AdTypeId = adTypeId
            })).AsList();

            return list;
        });
    }

    #endregion
}
