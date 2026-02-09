using Middleware.Web.Domains.LandingPages;
using System.Threading.Tasks;

namespace Middleware.Web.Services.LandingPages;

    /// <summary>
    /// Contest page log service interface
    /// </summary>
    public interface ILandingPageRecordService
    {
	/// <summary>
	/// Insert landing page record
	/// </summary>
	/// <param name="landingPageRecord">landingPageRecord</param>
	Task InsertLandingPageRecordAsync(LandingPageRecord landingPageRecord);

	/// <summary>
	/// Get all landing page records count by landing page id
	/// </summary>
	/// <param name="landingPageId">landing page identifier</param>
	/// <returns>contest page logs count</returns>
	Task<int> GetLandingPageRecordsCountBylandingPageIdAsync(int landingPageId);
    }
