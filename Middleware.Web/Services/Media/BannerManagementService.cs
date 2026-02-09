using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Banner;
using Middleware.Web.Domains.Common;
using Middleware.Web.Services.Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Middleware.Web.Data;

namespace Middleware.Web.Services.Media;

public class BannerManagementService : IBannerManagementService
{
    private const string BannerTable = "FM_Banner";
    private const string BannerDisplayTargetTable = "FM_BannerDisplayTarget";
    private const string BannerLocationMappingTable = "FM_Banner_Location_Mapping";

    private readonly DbConnectionFactory _db;
    private readonly ICommonService _commonService;
    private readonly IStaticCacheManager _staticCacheManager;

    public BannerManagementService(DbConnectionFactory db,
        ICommonService commonService,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _commonService = commonService;
        _staticCacheManager = staticCacheManager;
    }

    public async Task<IList<Banner>> GetActiveBannersAsync(int bannerTypeId)
    {
        var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);

        return await _staticCacheManager.GetAsync(CacheKeys.HomePageBannerCacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $@"
SELECT b.* FROM [{BannerTable}] b
INNER JOIN [{BannerDisplayTargetTable}] bt ON b.BannerDisplayTargetId = bt.Id
WHERE b.Published = 1 AND bt.BannerTypeId = @BannerTypeId
  AND (b.StartDateUtc <= @CurrentDate OR b.StartDateUtc IS NULL)
  AND (@CurrentDate <= b.EndDateUtc OR b.EndDateUtc IS NULL)";
            var list = (await conn.QueryAsync<Banner>(sql, new { BannerTypeId = bannerTypeId, CurrentDate = currentDate })).AsList();
            return list;
        });
    }

    public async Task<Banner> GetActiveBannerAsync(int bannerTypeId, int bannerDisplayTargetId)
    {
        var list = await GetActiveBannersAsync(bannerTypeId);
        return list.Where(b => b.BannerDisplayTargetId == bannerDisplayTargetId).OrderByDescending(x => x.StartDateUtc).FirstOrDefault();
    }

    public async Task<Banner> GetBannerByIdAsync(int bannerId)
    {
        var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.BannerByBannerIdKey, bannerId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $@"
SELECT * FROM [{BannerTable}]
WHERE Published = 1 AND Id = @BannerId
  AND (StartDateUtc <= @CurrentDate OR StartDateUtc IS NULL)
  AND (@CurrentDate <= EndDateUtc OR EndDateUtc IS NULL)";
            return await conn.QueryFirstOrDefaultAsync<Banner>(sql, new { BannerId = bannerId, CurrentDate = currentDate });
        });
    }

    public async Task<Banner> GetBannerByLocationIdAsync(int locationId = 0)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.LocationBannerByLocationIdKey, locationId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var dateTimeNow = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            using var conn = _db.CreateNop();
            var sql = $@"
SELECT TOP 1 b.* FROM [{BannerTable}] b
INNER JOIN [{BannerLocationMappingTable}] pb ON b.Id = pb.BannerId
WHERE b.Published = 1 AND pb.LocationId = @LocationId
  AND (b.StartDateUtc <= @Now OR b.StartDateUtc IS NULL) AND (@Now <= b.EndDateUtc OR b.EndDateUtc IS NULL)";
            return await conn.QueryFirstOrDefaultAsync<Banner>(sql, new { LocationId = locationId, Now = dateTimeNow });
        });
    }
}
