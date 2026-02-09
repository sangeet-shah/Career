using Middleware.Web.Domains.Career;
using Middleware.Web.Domains.CorporateManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Career;

/// <summary>
/// Carrer service interface
/// </summary>
public interface ICareerService
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
