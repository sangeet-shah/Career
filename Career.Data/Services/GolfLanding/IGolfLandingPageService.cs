using Career.Data.Domains.LandingPages;
using System.Threading.Tasks;

namespace Career.Data.Services.GolfLanding;

    /// <summary>
    /// Golf landing page service interface
    /// </summary>
    public partial interface IGolfLandingPageService
    {
	/// <summary>
	/// Inserts golf landing page
	/// <param name="golfLandingPage">golfLandingPage</param>
	/// </summary>
	Task InsertGolfEventLandingPageAsync(GolfEventLandingPage golfLandingPage);

}
