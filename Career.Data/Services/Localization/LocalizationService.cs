using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.Localization;
using Career.Data.Extensions;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Localization;

/// <summary>
/// Localization service interface
/// </summary>
public class LocalizationService : ILocalizationService
{
    #region Fields

    private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public LocalizationService(IRepository<LocaleStringResource> localeStringResourceRepository, 
        IStaticCacheManager staticCacheManager)
    {
        _localeStringResourceRepository = localeStringResourceRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Method

    /// <summary>
    /// Get locale string value by name
    /// </summary>
    /// <param name="resourceName">resourceName</param>
    /// <returns>Value</returns>
    public async Task<string> GetLocaleStringResourceByNameAsync(string resourceName)
    {
        if (string.IsNullOrEmpty(resourceName))
            return string.Empty;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.LocaleStringResourceCacheKey, resourceName);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await (from p in _localeStringResourceRepository.Table
                          where p.ResourceName.Equals(resourceName)
                          select p.ResourceValue).FirstOrDefaultAsync();
        });
    }

    #endregion
}
