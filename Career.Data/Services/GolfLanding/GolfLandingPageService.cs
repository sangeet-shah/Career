using Career.Data.Data;
using Career.Data.Domains.LandingPages;
using System;
using System.Threading.Tasks;

namespace Career.Data.Services.GolfLanding;

/// <summary>
/// Golf landing page service interface
/// </summary>
public class GolfLandingPageService : IGolfLandingPageService
{
    #region Fields

    private readonly IRepository<GolfEventLandingPage> _golfEventLandingPageRepository;

    #endregion

    #region Ctor

    public GolfLandingPageService(IRepository<GolfEventLandingPage> golfEventLandingPageRepository)
    {
			_golfEventLandingPageRepository = golfEventLandingPageRepository;
    }

    #endregion

    #region Methods 

    /// <summary>
    /// Inserts golf landing page
    /// <param name="golfLandingPage">golfLandingPage</param>
    /// </summary>
    public async Task InsertGolfEventLandingPageAsync(GolfEventLandingPage golfLandingPage)
    {
        if (golfLandingPage == null)
            throw new ArgumentNullException("golfLandingPage");

        await _golfEventLandingPageRepository.InsertAsync(golfLandingPage);
    }

    #endregion
}
