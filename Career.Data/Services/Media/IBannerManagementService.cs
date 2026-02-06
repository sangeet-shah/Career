using Career.Data.Domains.Banner;
using System.Threading.Tasks;

namespace Career.Data.Services.Media;

    /// <summary>
    /// Represents the banner management model service
    /// </summary>
    public partial interface IBannerManagementService
    {
	/// <summary>
	/// Get active banner
	/// </summary>
	/// <returns>banner</returns>
	Task<Banner> GetActiveBannerAsync(int bannerTypeId, int bannerDisplayTargetId);

	Task<Banner> GetBannerByIdAsync(int bannerId);

	Task<Banner> GetBannerByLocationIdAsync(int locationId = 0);

}
