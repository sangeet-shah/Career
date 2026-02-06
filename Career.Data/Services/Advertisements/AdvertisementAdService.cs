using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Advertisements;
using Career.Data.Domains.Common;
using Career.Data.Extensions;
using Career.Data.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Career.Data.Services.Advertisements;

/// <summary>
/// Weekly ad service interface
/// </summary>
public class AdvertisementService : IAdvertisementService
{
    #region Fields

    private readonly IRepository<Advertisement> _advertisementRepository;
    private readonly ICommonService _commonService;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public AdvertisementService(IRepository<Advertisement> advertisementRepository,
        ICommonService commonService,
        IStaticCacheManager staticCacheManager)
    {
        _advertisementRepository = advertisementRepository;
        _commonService = commonService;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets Advertisements
    /// </summary>
    /// <returns>Advertisement</returns>
    public async Task<IList<Advertisement>> GetActiveAdvertisementByStoreAndTypeAsync(int adTypeId, int storeId = 0)
    {
        return await _staticCacheManager.GetAsync(CacheKeys.AllAdvertisementKey, async () =>
        {
            var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            return await (from w in _advertisementRepository.Table
                          where w.Published && ((w.StartDateUtc <= currentDate || w.StartDateUtc == null)
                          && (currentDate <= w.EndDateUtc || w.EndDateUtc == null))
                          && w.StoreId == storeId && w.AdTypeId == adTypeId
                          orderby w.EndDateUtc
                          select w).ToListAsync();

        });
    }

    #endregion
}
