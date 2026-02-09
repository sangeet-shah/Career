using Middleware.Web.Data;
using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.Seo;
using Middleware.Web.Domains.Stores;
using Dapper;
using Middleware.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Seo;

public class UrlRecordService : IUrlRecordService
{
    private const string UrlRecordTable = "UrlRecord";
    private const string StoreMappingTable = "StoreMapping";

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    public UrlRecordService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    protected UrlRecord Map(UrlRecord record)
    {
        if (record == null)
            throw new ArgumentNullException(nameof(record));
        return new UrlRecord
        {
            Id = record.Id,
            EntityId = record.EntityId,
            EntityName = record.EntityName,
            Slug = record.Slug,
            IsActive = record.IsActive,
            LanguageId = record.LanguageId
        };
    }

    public async Task<UrlRecord> GetBySlugAsync(string slug, int storeId = 0)
    {
        if (string.IsNullOrEmpty(slug))
            return null;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.UrlRecordBySlugCacheKey, slug);
        var query = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT * FROM [{UrlRecordTable}] WHERE Slug = @Slug OR Slug = @SlugTrim ORDER BY IsActive DESC, Id";
            var list = (await conn.QueryAsync<UrlRecord>(sql, new { Slug = slug, SlugTrim = slug.TrimEnd('/') })).AsList();
            return list;
        });

        if (storeId != 0)
        {
            using (var conn = _db.CreateNop())
            {
                var storeMappingSql = $"SELECT EntityId, EntityName FROM [{StoreMappingTable}] WHERE StoreId = @StoreId";
                var mappings = (await conn.QueryAsync<StoreMappingKey>(storeMappingSql, new { StoreId = storeId }))
                    .Select(m => (m.EntityId, m.EntityName)).ToHashSet();
                query = query.Where(ur => mappings.Contains((ur.EntityId, ur.EntityName))).OrderByDescending(ur => ur.IsActive).ThenBy(ur => ur.Id).ToList();
            }
        }

        var urlRecord = query.FirstOrDefault(ur => string.Equals(ur.Slug, slug, StringComparison.InvariantCultureIgnoreCase) || string.Equals(ur.Slug, slug.TrimEnd('/'), StringComparison.InvariantCultureIgnoreCase));

        if (urlRecord == null && (slug == "news" || slug == "news/"))
            return new UrlRecord { EntityName = "news", IsActive = true };

        return urlRecord;
    }

    public async Task<string> GetSeNameAsync(int entityId, string entityName)
    {
        if (entityId == 0 || string.IsNullOrEmpty(entityName))
            return null;

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.UrlRecordCacheKey, entityId, entityName);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT TOP 1 Slug FROM [{UrlRecordTable}] WHERE EntityId = @EntityId AND EntityName = @EntityName ORDER BY IsActive DESC, Id";
            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { EntityId = entityId, EntityName = entityName });
        });
    }

    public async Task<IList<UrlRecord>> GetSlugsAsync(string entityName, int storeId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.UrlRecordByEntityCacheKey, entityName, storeId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT * FROM [{UrlRecordTable}] WHERE EntityName = @EntityName";
            var urlRecords = (await conn.QueryAsync<UrlRecord>(sql, new { EntityName = entityName })).AsList();

            if (storeId != 0 && urlRecords.Count > 0)
            {
                var entityIds = urlRecords.Select(ur => ur.EntityId).Distinct().ToList();
                var mapSql = $"SELECT EntityId FROM [{StoreMappingTable}] WHERE StoreId = @StoreId AND EntityName = @EntityName AND EntityId IN @EntityIds";
                var allowedIds = (await conn.QueryAsync<int>(mapSql, new { StoreId = storeId, EntityName = entityName, EntityIds = entityIds })).ToHashSet();
                urlRecords = urlRecords.Where(ur => allowedIds.Contains(ur.EntityId)).OrderByDescending(ur => ur.IsActive).ThenBy(ur => ur.Id).ToList();
            }

            return urlRecords.Select(Map).ToList();
        });
    }

    private class StoreMappingKey
    {
        public int EntityId { get; set; }
        public string EntityName { get; set; }
    }
}
