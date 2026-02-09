using Middleware.Web.Domains;
using Middleware.Web.Domains.Stores;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Stores;

public interface IStoreMappingService
{
    #region Methods

    Task<bool> AuthorizeAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported;

    bool Authorize<TEntity>(TEntity entity, int storeId) where TEntity : BaseEntity, IStoreMappingSupported;

    int[] GetStoresIdsWithAccess<TEntity>(TEntity entity) where TEntity : BaseEntity, IStoreMappingSupported;

    #endregion
}
