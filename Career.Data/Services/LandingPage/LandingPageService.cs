using Career.Data.Data;
using Career.Data.Domains.LandingPages;
using System.Threading.Tasks;

namespace Career.Data.Services.LandingPages;

/// <summary>
/// landing page service interface
/// </summary>
public partial class LandingPageService : ILandingPageService
{
    #region Fields

    private readonly IRepository<LandingPage> _landingPageRepository;

    #endregion

    #region Ctor

    public LandingPageService(IRepository<LandingPage> landingPageRepository)
    {
        _landingPageRepository = landingPageRepository;
    }

    #endregion

    /// <summary>
    /// Gets a landing page
    /// </summary>
    /// <param name="landingPageId">landing page identifier</param>
    /// <returns>landing page</returns>
    public async Task<LandingPage> GetlandingPageByIdAsync(int landingPageId)
    {
        if (landingPageId == 0)
            return null;

        return await _landingPageRepository.GetByIdAsync(landingPageId, cache => default);
    }
}
