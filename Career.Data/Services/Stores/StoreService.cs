using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.Stores;
using Career.Data.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Stores;

public class StoreService : IStoreService
{

    #region Fields

    private readonly IRepository<Store> _storeRepository;
    private readonly IRepository<FMStore> _fmStoreRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public StoreService(IRepository<Store> storeRepository,
        IRepository<FMStore> fmStoreRepository,
        IStaticCacheManager staticCacheManager)
    {
        _storeRepository = storeRepository;
        _fmStoreRepository = fmStoreRepository; 
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods   

    /// <summary>
    /// Get current store
    /// </summary>
    /// <returns>store</returns>
    public async Task<Store> GetCurrentStoreAsync()
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CurrentStoreCacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await (from s in _storeRepository.Table
                    join fs in _fmStoreRepository.Table on s.Id equals fs.StoreId
                    where fs.Alias == nameof(WebsiteEnum.FMUSA)
                    select s).FirstOrDefaultAsync();
        });
    }   

    #endregion
}
