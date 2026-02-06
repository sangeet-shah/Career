using Career.Data.Domains.CorporateManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Media;

public interface IGalleryService
{
    /// <summary>
    /// Gets highlighted photo gallery
    /// </summary>        
    /// <returns>Photo gallery</returns>
    Task<IList<CorporateGalleryPicture>> GetHighlightedGalleryAsync(int corporateGalleryId);

    Task<CorporateGallery> GetCorporateGalleryAsync();

	}
