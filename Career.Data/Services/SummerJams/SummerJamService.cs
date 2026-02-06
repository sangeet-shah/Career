using Career.Data.Data;
using Career.Data.Domains.LandingPages;
using System.Threading.Tasks;

namespace Career.Data.Services.SummerJams;

/// <summary>
/// SummerJam service interface
/// </summary>
public class SummerJamService : ISummerJamService
{
    #region Fields

    private readonly IRepository<SummerJam> _summerJamRepository;

    #endregion

    #region Ctor

    public SummerJamService(IRepository<SummerJam> summerJamRepository)
    {
        _summerJamRepository = summerJamRepository;
    }

    #endregion

    #region Methods 

    /// <summary>
    /// Insert summerJam
    /// </summary>
    /// <param name="summerJam">summerJam</param>
    public async Task InsertSummerJamAsync(SummerJam summerJam)
    {
        await _summerJamRepository.InsertAsync(summerJam);            
    }

    #endregion
}
