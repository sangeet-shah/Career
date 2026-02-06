using Career.Data.Data;
using Career.Data.Domains;
using Career.Data.Domains.Stores;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Stores;

public class StoreMappingService : IStoreMappingService
{

    #region Fields

    private readonly IRepository<StoreMapping> _storeMappingRepository;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public StoreMappingService(IRepository<StoreMapping> storeMappingRepository,
        IStoreService storeService)
    {
        _storeMappingRepository = storeMappingRepository;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    public async Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
        var store = await _storeService.GetCurrentStoreAsync();
        return Authorize(entity, store.Id);
    }

    public bool Authorize<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported
    {
        if (entity == null)
            return false;

        if (storeId == 0)
            //return true if no store specified/found
            return true;

        if (!entity.LimitedToStores)
            return true;

        foreach (var storeIdWithAccess in GetStoresIdsWithAccess(entity))
            if (storeId == storeIdWithAccess)
                //yes, we have such permission
                return true;

        //no permission found
        return false;
    }

    public int[] GetStoresIdsWithAccess<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;

        var query = from sm in _storeMappingRepository.Table
                    where sm.EntityId == entityId &&
                          sm.EntityName == entityName
                    select sm.StoreId;

        return query.ToArray();
    }

    #endregion
}
