using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.OffersPromotions;
using Dapper;
using Middleware.Web.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.OffersPromotions;

public class OffersPromotionsService : IOffersPromotionsService
{
    private const string OffersPromotionTable = "FM_OfferAndPromotion";

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    public OffersPromotionsService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    public async Task<IList<OffersPromotion>> GetAllActiveOffersPromotionsCachedAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.OffersPromotionsKey, async () =>
        {
            var currentDate = DateTime.UtcNow;
            using var conn = _db.CreateNop();
            var sql = $@"
SELECT * FROM [{OffersPromotionTable}]
WHERE (StartDateUtc <= @CurrentDate OR StartDateUtc IS NULL)
  AND (@CurrentDate <= EndDateUtc OR EndDateUtc IS NULL)
ORDER BY DisplayOrder";
            var list = (await conn.QueryAsync<OffersPromotion>(sql, new { CurrentDate = currentDate })).AsList();
            return list;
        });
    }
}
