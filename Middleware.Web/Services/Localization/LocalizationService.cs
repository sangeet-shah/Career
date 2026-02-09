using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.Localization;
using Dapper;
using Middleware.Web.Data;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Localization;

public class LocalizationService : ILocalizationService
{
    private const string LocaleStringResourceTable = "LocaleStringResource";

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    public LocalizationService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    public async Task<string> GetLocaleStringResourceByNameAsync(string resourceName)
    {
        if (string.IsNullOrEmpty(resourceName))
            return string.Empty;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.LocaleStringResourceCacheKey, resourceName);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT ResourceValue FROM [{LocaleStringResourceTable}] WHERE ResourceName = @ResourceName";
            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { ResourceName = resourceName });
        });
    }
}
