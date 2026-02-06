using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Common;
using Career.Data.Domains.Seo;
using Career.Data.Domains.Stores;
using Career.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Seo;

public class UrlRecordService : IUrlRecordService
{
    #region Fields

    private readonly IRepository<UrlRecord> _urlRecordRepository;
    private readonly IRepository<StoreMapping> _storeMappingRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public UrlRecordService(IRepository<UrlRecord> urlRecordRepository,
        IRepository<StoreMapping> storeMappingRepository,
        IStaticCacheManager staticCacheManager)
    {
        _urlRecordRepository = urlRecordRepository;
        _storeMappingRepository = storeMappingRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Map
    /// </summary>
    /// <param name="record">UrlRecord</param>
    /// <returns>UrlRecordForCaching</returns>
    protected UrlRecord Map(UrlRecord record)
    {
        if (record == null)
            throw new ArgumentNullException(nameof(record));

        var urlRecord = new UrlRecord
        {
            Id = record.Id,
            EntityId = record.EntityId,
            EntityName = record.EntityName,
            Slug = record.Slug,
            IsActive = record.IsActive,
            LanguageId = record.LanguageId
        };
        return urlRecord;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Find URL record
    /// </summary>
    /// <param name="slug">Slug</param>
    /// <param name="storeid">storeId</param>
    /// <returns>Found URL record</returns>
    public async Task<UrlRecord> GetBySlugAsync(string slug, int storeId = 0)
    {
        if (string.IsNullOrEmpty(slug))
            return null;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.UrlRecordBySlugCacheKey, slug);
        var query = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await (from ur in _urlRecordRepository.Table
                    where ur.Slug == slug || ur.Slug == slug.TrimEnd('/')
                    orderby ur.IsActive descending, ur.Id
                    select ur).ToListAsync();
        });

        // filter with store id 
        if (storeId != 0)
        {
            query = (from ur in query
                     join sm in _storeMappingRepository.Table on ur.EntityId equals sm.EntityId
                     where (ur.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) || ur.Slug.Equals(slug.TrimEnd('/'), StringComparison.InvariantCultureIgnoreCase))
                     && sm.StoreId == storeId
                     //first, try to find an active record
                     orderby ur.IsActive descending, ur.Id
                     select ur).ToList();
        }

        // get first or default slug
        var urlRecord = query.FirstOrDefault();

        // custom url record return
        if (urlRecord == null && slug == "news" || slug == "news/")
            return new UrlRecord { EntityName = "news", IsActive = true };

        return urlRecord;
    }

    /// <summary>
    /// Get search engine friendly name (slug)
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="entityName">Entity name</param>        
    /// <returns>Search engine  name (slug)</returns>
    public async Task<string> GetSeNameAsync(int entityId, string entityName)
    {
        if (entityId == 0 || string.IsNullOrEmpty(entityName))
            return null;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.UrlRecordCacheKey, entityId, entityName);
        //gradual loading
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await (from ur in _urlRecordRepository.Table
                    where ur.EntityId == entityId && ur.EntityName == entityName
                    //first, try to find an active record
                    orderby ur.IsActive descending, ur.Id
                    select ur.Slug).FirstOrDefaultAsync();
        });
    }

    public async Task<IList<UrlRecord>> GetSlugsAsync(string entityName, int storeId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.UrlRecordByEntityCacheKey, entityName, storeId);
        //gradual loading
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            //load all records (we know they are cached)
            var query = await (from ur in _urlRecordRepository.Table
                               where ur.EntityName == entityName
                               select ur).ToListAsync();

            var list = new List<UrlRecord>();
            foreach (var ur in query)
            {
                var urlRecordForCaching = Map(ur);
                list.Add(urlRecordForCaching);
            }

            var urlRecord = (from ur in list
                             join sm in _storeMappingRepository.Table on ur.EntityId equals sm.EntityId
                             where sm.StoreId == storeId 
                             //first, try to find an active record
                             orderby ur.IsActive descending, ur.Id
                             select ur).ToList();

            return urlRecord;
        });
    }

    #endregion
}
