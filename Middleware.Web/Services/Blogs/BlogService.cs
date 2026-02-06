using Career.Data.Data.Caching;
using Career.Data.Domains.Blogs;
using Career.Data.Domains.Common;
using Career.Data.Repository;
using Dapper;
using Middleware.Web.Data;
using Middleware.Web.Services.Common;

namespace Middleware.Web.Services.Blogs;

public class BlogService : IBlogService
{
    private const string BlogPostTable = "BlogPost";
    private const string FMBlogPostTable = "FM_BlogPost";
    private const string FMBlogPostAuthorTable = "FM_BlogPost_Author_Mapping";
    private const string StoreMappingTable = "StoreMapping";
    private const string EntityName = "BlogPost";

    private readonly DbConnectionFactory _db;
    private readonly ICommonService _commonService;
    private readonly IStaticCacheManager _staticCacheManager;

    public BlogService(DbConnectionFactory db,
        ICommonService commonService,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _commonService = commonService;
        _staticCacheManager = staticCacheManager;
    }

    public async Task<IPagedList<BlogPost>> GetBlogPostsAsync(int storeId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.BlogPostsKey, storeId, pageIndex, pageSize);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            var storeFilter = storeId > 0
                ? " AND (bp.LimitedToStores = 0 OR EXISTS (SELECT 1 FROM [" + StoreMappingTable + "] sm WHERE sm.EntityId = bp.Id AND sm.EntityName = @EntityName AND sm.StoreId = @StoreId))"
                : "";

            using var conn = _db.CreateNop();
            var countSql = $@"
SELECT COUNT(1) FROM [{BlogPostTable}] bp
INNER JOIN [{FMBlogPostTable}] fmbp ON bp.Id = fmbp.BlogPostId
WHERE (bp.StartDateUtc IS NOT NULL AND bp.StartDateUtc <= @CurrentDate) AND (bp.EndDateUtc IS NULL OR @CurrentDate <= bp.EndDateUtc){storeFilter}";
            var totalCount = await conn.ExecuteScalarAsync<int>(countSql, new { CurrentDate = currentDate, EntityName, StoreId = storeId });

            pageSize = Math.Max(pageSize, 1);
            var offset = pageIndex * pageSize;
            var dataSql = $@"
SELECT bp.* FROM [{BlogPostTable}] bp
INNER JOIN [{FMBlogPostTable}] fmbp ON bp.Id = fmbp.BlogPostId
WHERE (bp.StartDateUtc IS NOT NULL AND bp.StartDateUtc <= @CurrentDate) AND (bp.EndDateUtc IS NULL OR @CurrentDate <= bp.EndDateUtc){storeFilter}
ORDER BY bp.StartDateUtc DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var list = (await conn.QueryAsync<BlogPost>(dataSql, new { CurrentDate = currentDate, EntityName, StoreId = storeId, Offset = offset, PageSize = pageSize })).AsList();

            return new PagedList<BlogPost>(list, pageIndex, pageSize, totalCount);
        });
    }

    public async Task<IList<BlogPost>> GetBlogPostsByAuthorIdAsync(int storeId, int blogAuthorId = 0)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.BlogPostsByAuthorIdCacheKey, storeId, blogAuthorId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            var storeFilter = storeId > 0
                ? " AND (bp.LimitedToStores = 0 OR EXISTS (SELECT 1 FROM [" + StoreMappingTable + "] sm WHERE sm.EntityId = bp.Id AND sm.EntityName = @EntityName AND sm.StoreId = @StoreId))"
                : "";
            var authorJoin = blogAuthorId > 0
                ? $" INNER JOIN [{FMBlogPostAuthorTable}] bcm ON bp.Id = bcm.BlogPostId AND bcm.AuthorId = @BlogAuthorId"
                : "";

            using var conn = _db.CreateNop();
            var sql = $@"
SELECT bp.* FROM [{BlogPostTable}] bp
INNER JOIN [{FMBlogPostTable}] fmbp ON bp.Id = fmbp.BlogPostId{authorJoin}
WHERE (bp.StartDateUtc IS NOT NULL AND bp.StartDateUtc <= @CurrentDate) AND (bp.EndDateUtc IS NULL OR @CurrentDate <= bp.EndDateUtc){storeFilter}
ORDER BY bp.StartDateUtc DESC";
            var list = (await conn.QueryAsync<BlogPost>(sql, new { CurrentDate = currentDate, EntityName, StoreId = storeId, BlogAuthorId = blogAuthorId })).AsList();
            return list;
        });
    }

    public async Task<BlogPost> GetBlogPostByIdAsync(int storeId, int blogPostId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.BlogPostByIdCacheKey, storeId, blogPostId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            var storeFilter = storeId > 0
                ? " AND (bp.LimitedToStores = 0 OR EXISTS (SELECT 1 FROM [" + StoreMappingTable + "] sm WHERE sm.EntityId = bp.Id AND sm.EntityName = @EntityName AND sm.StoreId = @StoreId))"
                : "";

            using var conn = _db.CreateNop();
            var sql = $@"
SELECT bp.* FROM [{BlogPostTable}] bp
WHERE bp.Id = @BlogPostId
  AND (bp.StartDateUtc IS NOT NULL AND bp.StartDateUtc <= @CurrentDate) AND (bp.EndDateUtc IS NULL OR @CurrentDate <= bp.EndDateUtc){storeFilter}";
            return await conn.QueryFirstOrDefaultAsync<BlogPost>(sql, new { BlogPostId = blogPostId, CurrentDate = currentDate, EntityName, StoreId = storeId });
        });
    }

    public async Task<IList<BlogPost>> GetLatestBlogsAsync(int blogPostId = 0, int storeId = 0, int numberOfBlogs = 0)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.AllLatestBlogsCacheKey, blogPostId, storeId, numberOfBlogs);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var currentDate = DateTime.UtcNow;
            var storeFilter = storeId > 0
                ? " AND (bp.LimitedToStores = 0 OR EXISTS (SELECT 1 FROM [" + StoreMappingTable + "] sm WHERE sm.EntityId = bp.Id AND sm.EntityName = @EntityName AND sm.StoreId = @StoreId))"
                : "";

            using var conn = _db.CreateNop();
            var sql = $@"
SELECT bp.* FROM [{BlogPostTable}] bp
WHERE bp.Id != @BlogPostId
  AND (bp.StartDateUtc IS NOT NULL AND bp.StartDateUtc <= @CurrentDate) AND (bp.EndDateUtc IS NULL OR @CurrentDate <= bp.EndDateUtc){storeFilter}
ORDER BY bp.StartDateUtc DESC";
            if (numberOfBlogs > 0)
                sql += $" OFFSET 0 ROWS FETCH NEXT @NumberOfBlogs ROWS ONLY";
            var list = (await conn.QueryAsync<BlogPost>(sql, new { BlogPostId = blogPostId, CurrentDate = currentDate, EntityName, StoreId = storeId, NumberOfBlogs = numberOfBlogs })).AsList();
            return list;
        });
    }

    public async Task<FMBlogPost> GetFMBlogByBlogPostIdAsync(int blogPostId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.FMBlogByBlogPostIdCacheKey, blogPostId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT TOP 1 * FROM [{FMBlogPostTable}] WHERE BlogPostId = @BlogPostId";
            return await conn.QueryFirstOrDefaultAsync<FMBlogPost>(sql, new { BlogPostId = blogPostId });
        });
    }

    public async Task<IList<FMBlogPostAuthor>> GetBlogPostAuthorsByBlogPostIdAsync(int blogPostId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.FMBlogPostAuthorByBlogPostIdCacheKey, blogPostId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var sql = $"SELECT * FROM [{FMBlogPostAuthorTable}] WHERE BlogPostId = @BlogPostId";
            var list = (await conn.QueryAsync<FMBlogPostAuthor>(sql, new { BlogPostId = blogPostId })).AsList();
            return list;
        });
    }
}
