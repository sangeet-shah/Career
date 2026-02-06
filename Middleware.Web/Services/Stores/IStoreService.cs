using Career.Data.Domains.Stores;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Stores;

public interface IStoreService
{
    #region Methods

    /// <summary>
    /// Get current store
    /// </summary>
    /// <returns>store</returns>
    Task<Store> GetCurrentStoreAsync();

    #endregion
}
