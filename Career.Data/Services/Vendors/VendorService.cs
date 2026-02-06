using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.FMVendors;
using Career.Data.Domains.Vendors;
using Career.Data.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Vendors;

/// <summary>
/// Vendor service
/// </summary>
public class VendorService : IVendorService
{
    #region Fields

    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IRepository<FMVendor> _fmVendorRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public VendorService(IRepository<Vendor> vendorRepository, IStaticCacheManager staticCacheManager, IRepository<FMVendor> fmVendorRepository)
    {
        _vendorRepository = vendorRepository;
        _staticCacheManager = staticCacheManager;
        _fmVendorRepository = fmVendorRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get all vendors
    /// </summary>
    /// <returns>Vendors</returns>
    public async Task<IList<FMVendor>> GetAllVendorsAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.AllVendorKey, async () =>
        {
            return await (from v in _vendorRepository.Table
                          join fmv in _fmVendorRepository.Table on v.Id equals fmv.VendorId
                          where fmv.IsCorporate && v.Active && !v.Deleted
                          orderby v.DisplayOrder, v.Name
                          select fmv).ToListAsync();
        });
    }    

    #endregion
}
