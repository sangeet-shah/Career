using Career.Data.Domains.LandingPages;
using System.Threading.Tasks;

namespace Career.Data.Services.LandingPages;

/// <summary>
/// landing page service interface
/// </summary>
public partial interface ILandingPageService
    {
	/// <summary>
	/// Gets a landing page
	/// </summary>
	/// <param name="landingPageId">landing page identifier</param>
	/// <returns>landing page</returns>
	Task<LandingPage> GetlandingPageByIdAsync(int landingPageId);
    }
