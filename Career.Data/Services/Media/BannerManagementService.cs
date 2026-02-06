using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Banner;
using Career.Data.Domains.Common;
using Career.Data.Extensions;
using Career.Data.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Media;

/// <summary>
/// Represents the banner management model service implementation
/// </summary>
public partial class BannerManagementService : IBannerManagementService
{
    #region Fields

    private readonly IRepository<Banner> _bannerManagementRepository;
    private readonly IRepository<BannerDisplayTarget> _bannerDisplayTargetRepository;
    private readonly IRepository<BannerLocationMapping> _bannerLocationMappingRepository;
    private readonly ICommonService _commonService;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public BannerManagementService(IRepository<Banner> bannerManagementRepository,
        ICommonService commonService,
        IStaticCacheManager staticCacheManager,
        IRepository<BannerDisplayTarget> bannerDisplayTargetRepository,
        IRepository<BannerLocationMapping> bannerLocationMappingRepository)
    {
        _bannerManagementRepository = bannerManagementRepository;
        _commonService = commonService;
        _staticCacheManager = staticCacheManager;
        _bannerDisplayTargetRepository = bannerDisplayTargetRepository;
        _bannerLocationMappingRepository = bannerLocationMappingRepository;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get active banner
    /// </summary>
    /// <returns>banner</returns>
    public async Task<IList<Banner>> GetActiveBannersAsync(int bannerTypeId)
    {
        var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);

        return await _staticCacheManager.GetAsync(CacheKeys.HomePageBannerCacheKey, async () =>
        {
            return await (from b in _bannerManagementRepository.Table
                          join bt in _bannerDisplayTargetRepository.Table on b.BannerDisplayTargetId equals bt.Id
                          where b.Published && bt.BannerTypeId == bannerTypeId &&
                              ((b.StartDateUtc <= currentDate || b.StartDateUtc == null)
                                  && (currentDate <= b.EndDateUtc || b.EndDateUtc == null))
                          select b).ToListAsync();
        });
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get active banner
    /// </summary>
    /// <returns>banner</returns>
    public async Task<Banner> GetActiveBannerAsync(int bannerTypeId, int bannerDisplayTargetId)
    {
        return (from b in await GetActiveBannersAsync(bannerTypeId)
                where b.BannerDisplayTargetId == bannerDisplayTargetId
                select b).OrderByDescending(x => x.StartDateUtc).FirstOrDefault();
    }

    /// <summary>
    /// Get active banner
    /// </summary>
    /// <returns>banner</returns>
    public async Task<Banner> GetBannerByIdAsync(int bannerId)
    {
        var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.BannerByBannerIdKey, bannerId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await (from b in _bannerManagementRepository.Table
                          where b.Published && b.Id == bannerId &&
                          ((b.StartDateUtc <= currentDate || b.StartDateUtc == null)
                          && (currentDate <= b.EndDateUtc || b.EndDateUtc == null))
                          select b).FirstOrDefaultAsync();
        });
    }

    public async Task<Banner> GetBannerByLocationIdAsync(int locationId = 0)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.LocationBannerByLocationIdKey, locationId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var dateTimeNow = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            var banners = await (from b in _bannerManagementRepository.Table
                                 where b.Published == true &&
                                 (b.StartDateUtc <= dateTimeNow || b.StartDateUtc == null) && (dateTimeNow <= b.EndDateUtc || b.EndDateUtc == null)
                                 select b).ToListAsync();

            var result = (from b in banners
                          join pb in _bannerLocationMappingRepository.Table on b.Id equals pb.BannerId
                          where pb.LocationId == locationId
                          select b).FirstOrDefault();

            return result;
        });
    }

    #endregion
}
