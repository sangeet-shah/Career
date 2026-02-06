using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.Stores;
using Career.Data.Services.Stores;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.HelloBar;

public class HelloBarService : IHelloBarService
{
    #region Fields

    private readonly IRepository<HelloBars> _helloBarRepository;
    private readonly IRepository<StoreMapping> _storeMappingRepository;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreService _storeService;  

    #endregion

    #region Ctor

    public HelloBarService(IRepository<HelloBars> helloBarRepository,
        IRepository<StoreMapping> storeMappingRepository,
        IStaticCacheManager staticCacheManager,
        IStoreService storeService)
    {
        _helloBarRepository = helloBarRepository;
        _storeMappingRepository = storeMappingRepository;
        _staticCacheManager = staticCacheManager;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get active hello bars
    /// </summary>
    /// <returns>HelloBars</returns>
    public async Task<IList<HelloBars>> GetActiveHelloBarsAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.AllHellobarKey, async () =>
        {
            TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime centralTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralTimeZone);

            var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;  
            var query= (from hb in _helloBarRepository.Table
                          where hb.Published
                          && ((hb.StartDateUtc <= centralTime || hb.StartDateUtc == null)
                          && (centralTime <= hb.EndDateUtc || hb.EndDateUtc == null))                                                   
                          select hb);

            query = from bp in query
                    join sm in _storeMappingRepository.Table
                    on new { c1 = bp.Id, c2 = typeof(HelloBars).Name } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                    from sm in bp_sm.DefaultIfEmpty()
                    where storeId == sm.StoreId
                    select bp;

            return await query.OrderBy(x=>x.DisplayOrder).ThenByDescending(x=>x.StartDateUtc).ToListAsync();
        });
    }

    #endregion
}