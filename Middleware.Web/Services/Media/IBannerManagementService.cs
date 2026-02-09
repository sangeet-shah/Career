using Middleware.Web.Domains.Banner;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Media;

    /// <summary>
    /// Represents the banner management model service
    /// </summary>
    public interface IBannerManagementService
    {
	/// <summary>
	/// Get active banner
	/// </summary>
	/// <returns>banner</returns>
	Task<Banner> GetActiveBannerAsync(int bannerTypeId, int bannerDisplayTargetId);

	Task<Banner> GetBannerByIdAsync(int bannerId);

	Task<Banner> GetBannerByLocationIdAsync(int locationId = 0);

}
