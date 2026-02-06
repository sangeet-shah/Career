using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.OffersPromotions;
using Career.Data.Extensions;
using Career.Data.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.OffersPromotions;

/// <summary>
/// Represents the offers promotions service implementation
/// </summary>
public class OffersPromotionsService : IOffersPromotionsService
{
    #region Fields

    private readonly IRepository<OffersPromotion> _offersPromotionRepository;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly ICommonService _commonService;

    #endregion

    #region Ctor

    public OffersPromotionsService(IRepository<OffersPromotion> offersPromotionRepository,
                                   IStaticCacheManager staticCacheManager,
                                   ICommonService commonService)
    {
        _offersPromotionRepository = offersPromotionRepository;
        _staticCacheManager = staticCacheManager;
        _commonService = commonService;
    }

    #endregion

    #region Methods        

    /// <summary>
    /// Gets all active offers and promotions
    /// </summary>
    /// <returns></returns>
    public async Task<IList<OffersPromotion>> GetAllActiveOffersPromotionsCachedAsync()
    {            
        return await _staticCacheManager.GetAsync(CacheKeys.OffersPromotionsKey, async () =>
        {
            var currentDate = DateTime.UtcNow;
            return await (from op in _offersPromotionRepository.Table
                    where ((op.StartDateUtc <= currentDate || op.StartDateUtc == null) &&
                           (currentDate <= op.EndDateUtc || op.EndDateUtc == null))
                    orderby op.DisplayOrder
                    select op).ToListAsync();
        });
    }

    #endregion
}