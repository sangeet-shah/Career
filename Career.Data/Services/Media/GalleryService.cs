using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.CorporateManagement;
using Career.Data.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Media;

public partial class GalleryService : IGalleryService
{
    #region Fields

    private readonly IRepository<CorporateGallery> _galleryRepository;
    private readonly IRepository<CorporateGalleryPicture> _galleryPictureMappingRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public GalleryService(IRepository<CorporateGallery> galleryRepository,
        IRepository<CorporateGalleryPicture> galleryPictureMappingRepository,
        IStaticCacheManager staticCacheManager)
    {
        _galleryRepository = galleryRepository;
        _galleryPictureMappingRepository = galleryPictureMappingRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets highlighted photo gallery
    /// </summary>        
    /// <returns>Photo gallery</returns>
    public async Task<CorporateGallery> GetCorporateGalleryAsync()
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CorporateGalleryCacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await (from g in _galleryRepository.Table
                          where g.Highlighted
                          orderby g.DisplayOrder
                          select g).FirstOrDefaultAsync();
        });
    }

    /// <summary>
    /// Gets highlighted photo gallery
    /// </summary>        
    /// <returns>Photo gallery</returns>
    public async Task<IList<CorporateGalleryPicture>> GetHighlightedGalleryAsync(int corporateGalleryId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CorporateGalleryPictureByCorporateGalleryIdCacheKey, corporateGalleryId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await (from g in _galleryRepository.Table
                          join gm in _galleryPictureMappingRepository.Table on g.Id equals gm.CorporateGalleryId
                          where g.Highlighted && gm.CorporateGalleryId == corporateGalleryId
                          orderby g.DisplayOrder, gm.DisplayOrder
                          select gm).ToListAsync();
        });
    }

    #endregion
}
