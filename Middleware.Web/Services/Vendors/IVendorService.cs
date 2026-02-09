using Middleware.Web.Domains.FMVendors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Vendors;

/// <summary>
/// Vendor service interface
/// </summary>
public interface IVendorService
{
    /// <summary>
    /// Get all vendors
    /// </summary>
    /// <returns>Vendors</returns>
    Task<IList<FMVendor>> GetAllVendorsAsync();
}
