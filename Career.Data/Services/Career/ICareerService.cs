using Career.Data.Domains.Career;
using Career.Data.Domains.CorporateManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Career.Data.Services.Career;

/// <summary>
/// Carrer service interface
/// </summary>
public partial interface ICareerService
{
    /// <summary>
    /// Gets all departments
    /// </summary>
    Task<IList<Department>> GetAllDepartmentAsync();
      
    /// <summary>
    /// Get all career brands
    /// </summary>
    /// <returns>career brands</returns>
    Task<IList<CorporateBrandPage>> GetAllCorporateBrandPagesAsync();
}
