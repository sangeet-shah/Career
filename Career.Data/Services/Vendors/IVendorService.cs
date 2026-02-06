using Career.Data.Domains.FMVendors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Career.Data.Services.Vendors;

/// <summary>
/// Vendor service interface
/// </summary>
public partial interface IVendorService
{
    /// <summary>
    /// Get all vendors
    /// </summary>
    /// <returns>Vendors</returns>
    Task<IList<FMVendor>> GetAllVendorsAsync();
}
