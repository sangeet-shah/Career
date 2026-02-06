using Career.Data.Domains;
using Career.Data.Domains.Stores;
using System.Threading.Tasks;

namespace Career.Data.Services.Stores;

public interface IStoreMappingService
{
    #region Methods

    Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported;

    bool Authorize<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported;

    int[] GetStoresIdsWithAccess<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported;

    #endregion
}
