using Career.Data.Domains.LandingPages;
using System.Threading.Tasks;

namespace Middleware.Web.Services.RegistrationPage;

    public interface ILandingPageClosedService
    {
	/// <summary>
	/// Gets a closed from
	/// </summary>
	/// <param name="contestId">Contest identifier</param>
	/// <returns>ClosedFrom</returns>
	Task<LandingPageClosed> GetClosedFromByLandingPageIdAsync(int landingPageId);
    }
