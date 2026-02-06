using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains;
using Career.Data.Domains.Common;
using Dapper;
using Middleware.Web.Data;
using Middleware.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Common;

public class GenericAttributeService : IGenericAttributeService
{
    private const string GenericAttributeTable = "GenericAttribute";

    #region Fields

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public GenericAttributeService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    public async Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.GenericAttributeCacheKey, entityId, keyGroup);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT Id, EntityId, KeyGroup, [Key], Value, StoreId FROM [{GenericAttributeTable}] WHERE EntityId = @EntityId AND KeyGroup = @KeyGroup";
            var list = (await conn.QueryAsync<GenericAttribute>(sql, new { EntityId = entityId, KeyGroup = keyGroup })).AsList();
            return list;
        });
    }

    public async Task<TPropType> GetAttributeAsync<TPropType>(BaseEntity entity, string key, string keyGroup, int storeId = 0)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var props = await GetAttributesForEntityAsync(entity.Id, keyGroup);
        if (props == null)
            return default;

        props = props.Where(x => x.StoreId == storeId).ToList();
        if (!props.Any())
            return default;

        var prop = props.FirstOrDefault(ga => ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        if (prop == null || string.IsNullOrEmpty(prop.Value))
            return default;

        return CommonHelper.To<TPropType>(prop.Value);
    }

    #endregion
}
