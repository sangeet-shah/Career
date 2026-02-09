using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.CorporateManagement;
using Dapper;
using Middleware.Web.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Media;

public class GalleryService : IGalleryService
{
    private const string CorporateGalleryTable = "FM_CorporateGallery";
    private const string CorporateGalleryPictureTable = "FM_CorporateGalleryPicture";

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    public GalleryService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    public async Task<CorporateGallery> GetCorporateGalleryAsync()
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CorporateGalleryCacheKey);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT TOP 1 * FROM [{CorporateGalleryTable}] WHERE Highlighted = 1 ORDER BY DisplayOrder";
            return await conn.QueryFirstOrDefaultAsync<CorporateGallery>(sql);
        });
    }

    public async Task<IList<CorporateGalleryPicture>> GetHighlightedGalleryAsync(int corporateGalleryId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.CorporateGalleryPictureByCorporateGalleryIdCacheKey, corporateGalleryId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $@"
SELECT gm.* FROM [{CorporateGalleryPictureTable}] gm
INNER JOIN [{CorporateGalleryTable}] g ON g.Id = gm.CorporateGalleryId
WHERE g.Highlighted = 1 AND gm.CorporateGalleryId = @CorporateGalleryId
ORDER BY g.DisplayOrder, gm.DisplayOrder";
            var list = (await conn.QueryAsync<CorporateGalleryPicture>(sql, new { CorporateGalleryId = corporateGalleryId })).AsList();
            return list;
        });
    }
}
