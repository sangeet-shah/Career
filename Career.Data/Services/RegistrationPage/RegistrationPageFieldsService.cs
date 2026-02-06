using Career.Data.Data;
using Career.Data.Domains.RegistrationPage;
using Career.Data.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.RegistrationPage;

public class RegistrationPageFieldsService : IRegistrationPageFieldsService
{
    #region Fields

    private readonly IRepository<RegistrationPageFields> _registrationPageFieldsRepository;

    #endregion

    #region Ctor

    public RegistrationPageFieldsService(IRepository<RegistrationPageFields> registrationPageFieldsRepository)
    {
        _registrationPageFieldsRepository = registrationPageFieldsRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a registration page fields by contest identifier
    /// </summary>
    /// <param name="contestId">Contest identifier</param>
    /// <returns>RegistrationPageFields</returns>
    public async Task<RegistrationPageFields> GetRegistrationPageFieldsByContestIdAsync(int contestId)
    {
        return await _registrationPageFieldsRepository.Table.Where(cf => cf.ContestId == contestId).FirstOrDefaultAsync();
    }

    #endregion
}
