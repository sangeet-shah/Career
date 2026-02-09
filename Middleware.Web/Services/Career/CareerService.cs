using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Career;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.CorporateManagement;
using Dapper;
using Middleware.Web.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Career;

public class CareerService : ICareerService
{
    private const string DepartmentTable = "Department";
    private const string CorporateBrandPageTable = "FM_Corporate_BrandPage";

    #region Fields

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public CareerService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    public async Task<IList<Department>> GetAllDepartmentAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.DepartmentsKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT Id, Name FROM [{DepartmentTable}] ORDER BY Name";
            var list = (await conn.QueryAsync<Department>(sql)).AsList();
            return list;
        });
    }

    public async Task<IList<CorporateBrandPage>> GetAllCorporateBrandPagesAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.CareerBrandsKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT Id, PictureId, Description, Url, DisplayOrder FROM [{CorporateBrandPageTable}] ORDER BY DisplayOrder";
            var list = (await conn.QueryAsync<CorporateBrandPage>(sql)).AsList();
            return list;
        });
    }

    #endregion
}
