using Career.Data.Data;
using Career.Data.Domains.LandingPages;
using Career.Data.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.RegistrationPage;

public class LandingPageClosedService : ILandingPageClosedService
{
    #region Fields

    private readonly IRepository<LandingPageClosed> _landingPageClosedRepository;

    #endregion

    #region Ctor

    public LandingPageClosedService(IRepository<LandingPageClosed> landingPageClosedRepository)
    {
			_landingPageClosedRepository = landingPageClosedRepository;
    }

		#endregion

		#region Methods

		/// <summary>
		/// Gets a closed from
		/// </summary>
		/// <param name="landingPageId">Landing identifier</param>
		/// <returns>ClosedFrom</returns>
		public async Task<LandingPageClosed> GetClosedFromByLandingPageIdAsync(int landingPageId)
    {
        return await _landingPageClosedRepository.Table.Where(cf => cf.LandingPageId == landingPageId).FirstOrDefaultAsync();
    }

    #endregion
}
