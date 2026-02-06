using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.FMVendors;
using Career.Data.Domains.Vendors;
using Dapper;
using Middleware.Web.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Vendors;

public class VendorService : IVendorService
{
    private const string VendorTable = "Vendor";
    private const string FMVendorTable = "FM_Vendor";

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    public VendorService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    public async Task<IList<FMVendor>> GetAllVendorsAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.AllVendorKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $@"
SELECT fmv.* FROM [{FMVendorTable}] fmv
INNER JOIN [{VendorTable}] v ON v.Id = fmv.VendorId
WHERE fmv.IsCorporate = 1 AND v.Active = 1 AND v.Deleted = 0
ORDER BY v.DisplayOrder, v.Name";
            var list = (await conn.QueryAsync<FMVendor>(sql)).AsList();
            return list;
        });
    }
}
