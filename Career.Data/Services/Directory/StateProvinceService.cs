using Career.Data.Data;
using Career.Data.Domains.Directory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Career.Data.Services.Directory;

/// <summary>
/// State province service
/// </summary>
public partial class StateProvinceService : IStateProvinceService
{
    #region Fields

    private readonly IRepository<StateProvince> _stateProvinceRepository;

    #endregion

    #region Ctor

    public StateProvinceService(IRepository<StateProvince> stateProvinceRepository)
    {
        _stateProvinceRepository = stateProvinceRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a state/province
    /// </summary>
    /// <param name="stateProvinceId">The state/province identifier</param>
    /// <returns>State/province</returns>
    public async Task<StateProvince> GetStateProvinceByIdAsync(int stateProvinceId)
    {
        return await _stateProvinceRepository.GetByIdAsync(stateProvinceId, cache => default);
    }

    /// <summary>
    /// Gets all state/province
    /// </summary>
    /// <returns>State/provinces</returns>
    public async Task<IList<StateProvince>> GetAllStateProvincesAsync()
    {
        return await _stateProvinceRepository.GetAllAsync(query => { return query; }, cache => default);
    }


    #endregion
}