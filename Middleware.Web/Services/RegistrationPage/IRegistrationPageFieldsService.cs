using Career.Data.Domains.RegistrationPage;
using System.Threading.Tasks;

namespace Middleware.Web.Services.RegistrationPage;

public interface IRegistrationPageFieldsService
{
    /// <summary>
    /// Gets a registration page fields by contest identifier
    /// </summary>
    /// <param name="contestId">Contest identifier</param>
    /// <returns>RegistrationPageFields</returns>
    Task<RegistrationPageFields> GetRegistrationPageFieldsByContestIdAsync(int contestId);
}
