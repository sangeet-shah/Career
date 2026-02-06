using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains;
using Career.Data.Domains.Common;
using Career.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Common;

/// <summary>
/// Generic attribute service
/// </summary>
public partial class GenericAttributeService : IGenericAttributeService
{
    #region Fields

    private readonly IRepository<GenericAttribute> _genericAttributeRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public GenericAttributeService(IRepository<GenericAttribute> genericAttributeRepository,
        IStaticCacheManager staticCacheManager)
    {
        _genericAttributeRepository = genericAttributeRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods           

    /// <summary>
    /// Get attributes
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="keyGroup">Key group</param>
    /// <returns>Get attributes</returns>
    public async Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.GenericAttributeCacheKey, entityId,keyGroup);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await (from ga in _genericAttributeRepository.Table
                          where ga.EntityId == entityId &&
                          ga.KeyGroup == keyGroup
                          select ga).ToListAsync();
        });
    }

    /// <summary>
    /// Get an attribute of an entity
    /// </summary>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="key">Key</param>
    /// <param name="keyGroup">keyGroup</param>
    /// <param name="storeId">Load a value specific for a certain store; pass 0 to load a value shared for all stores</param>
    /// <returns>Attribute</returns>
    public async Task<TPropType> GetAttributeAsync<TPropType>(BaseEntity entity, string key, string keyGroup, int storeId = 0)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var props = await GetAttributesForEntityAsync(entity.Id, keyGroup);

        //little hack here (only for unit testing). we should write expect-return rules in unit tests for such cases
        if (props == null)
            return default(TPropType);

        props = props.Where(x => x.StoreId == storeId).ToList();
        if (!props.Any())
            return default(TPropType);

        var prop = props.FirstOrDefault(ga =>
            ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); //should be culture invariant

        if (prop == null || string.IsNullOrEmpty(prop.Value))
            return default(TPropType);

        return CommonHelper.To<TPropType>(prop.Value);
    }

    #endregion
}
