using Career.Data.Domains.Directory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Career.Data.Services.Directory;

/// <summary>
/// State province service interface
/// </summary>
public partial interface IStateProvinceService
{
    /// <summary>
    /// Gets a state/province
    /// </summary>
    /// <param name="stateProvinceId">The state/province identifier</param>
    /// <returns>State/province</returns>
    Task<StateProvince> GetStateProvinceByIdAsync(int stateProvinceId);

    /// <summary>
    /// Gets all state/province
    /// </summary>
    /// <returns>State/provinces</returns>
    Task<IList<StateProvince>> GetAllStateProvincesAsync();
}