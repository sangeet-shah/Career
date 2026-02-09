using Middleware.Web.Domains.Advertisements;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Advertisements;

/// <summary>
/// Weekly ad service interface
/// </summary>
public interface IAdvertisementService
{
    /// <summary>
    /// Gets active weekly ads by store identifier
    /// </summary>
    /// <returns>WeeklyAds</returns>
    Task<IList<Advertisement>> GetActiveAdvertisementByStoreAndTypeAsync(int adTypeId, int storeId = 0);

}
