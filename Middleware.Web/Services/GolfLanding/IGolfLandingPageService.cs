using Middleware.Web.Domains.LandingPages;
using System.Threading.Tasks;

namespace Middleware.Web.Services.GolfLanding;

    /// <summary>
    /// Golf landing page service interface
    /// </summary>
    public interface IGolfLandingPageService
    {
	/// <summary>
	/// Inserts golf landing page
	/// <param name="golfLandingPage">golfLandingPage</param>
	/// </summary>
	Task InsertGolfEventLandingPageAsync(GolfEventLandingPage golfLandingPage);

}
