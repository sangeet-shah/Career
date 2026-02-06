using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Career;
using Career.Data.Domains.Common;
using Career.Data.Domains.CorporateManagement;
using Career.Data.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Career;

/// <summary>
/// Career service
/// </summary>
public class CareerService : ICareerService
{
    #region Fields

    private readonly IRepository<Department> _departmentRepository;   
    private readonly IRepository<CorporateBrandPage> _corporateBrandPageRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public CareerService(IRepository<Department> departmentRepository,        
        IStaticCacheManager staticCacheManager,
        IRepository<CorporateBrandPage> corporateBrandPageRepository)
    {
        _departmentRepository = departmentRepository;
        _staticCacheManager = staticCacheManager;
        _corporateBrandPageRepository = corporateBrandPageRepository;
    }

    #endregion

    #region Methods        

    /// <summary>
    /// Gets all departments
    /// </summary>
    public async Task<IList<Department>> GetAllDepartmentAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.DepartmentsKey, async () =>
        {
            return await _departmentRepository.Table.OrderBy(d => d.Name).ToListAsync();            
        });
    }
       
    /// <summary>
    /// Get all career brands
    /// </summary>
    /// <returns>career brands</returns>
    public async Task<IList<CorporateBrandPage>> GetAllCorporateBrandPagesAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.CareerBrandsKey, async () =>
        {
            return await _corporateBrandPageRepository.Table.OrderBy(c => c.DisplayOrder).ToListAsync();
        });
    }

    #endregion
}
