using Career.Data.Data;
using Career.Data.Domains;
using Career.Data.Domains.Stores;
using Dapper;
using Middleware.Web.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Stores;

public class StoreMappingService : IStoreMappingService
{
    private const string StoreMappingTable = "StoreMapping";

    private readonly DbConnectionFactory _db;
    private readonly IStoreService _storeService;

    public StoreMappingService(DbConnectionFactory db,
        IStoreService storeService)
    {
        _db = db;
        _storeService = storeService;
    }

    public async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
        var store = await _storeService.GetCurrentStoreAsync();
        return Authorize(entity, store?.Id ?? 0);
    }

    public bool Authorize<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported
    {
        if (entity == null)
            return false;

        if (storeId == 0)
            return true;

        if (!entity.LimitedToStores)
            return true;

        foreach (var storeIdWithAccess in GetStoresIdsWithAccess(entity))
            if (storeId == storeIdWithAccess)
                return true;

        return false;
    }

    public async Task<int[]> GetStoresIdsWithAccessAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;

        using var conn = _db.CreateNop();
        var sql = $"SELECT StoreId FROM [{StoreMappingTable}] WHERE EntityId = @EntityId AND EntityName = @EntityName";
        var list = (await conn.QueryAsync<int>(sql, new { EntityId = entityId, EntityName = entityName })).AsList();
        return list.ToArray();
    }

    public int[] GetStoresIdsWithAccess<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
        return GetStoresIdsWithAccessAsync(entity).GetAwaiter().GetResult();
    }
}
