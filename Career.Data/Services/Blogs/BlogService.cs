using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Blogs;
using Career.Data.Domains.Common;
using Career.Data.Domains.Stores;
using Career.Data.Extensions;
using Career.Data.Repository;
using Career.Data.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Blogs;

/// <summary>
/// Blog service
/// </summary>
public partial class BlogService : IBlogService
{
    #region Fields

    private readonly IRepository<BlogPost> _blogPostRepository;
    private readonly IRepository<FMBlogPost> _fmBlogPostRepository;
    private readonly ICommonService _commonService;
    private readonly IRepository<StoreMapping> _storeMappingRepository;
    private readonly IRepository<FMBlogPostAuthor> _fmBlogPostAuthorRepository;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly string _entityName;

    #endregion

    #region Ctor

    public BlogService(IRepository<BlogPost> blogPostRepository,
                       ICommonService commonService,
                       IRepository<StoreMapping> storeMappingRepository,
                       IRepository<FMBlogPost> fmBlogPostRepository,
                       IRepository<FMBlogPostAuthor> fmBlogPostAuthorRepository,
                       IStaticCacheManager staticCacheManager)
    {
        _blogPostRepository = blogPostRepository;
        _commonService = commonService;
        _storeMappingRepository = storeMappingRepository;
        _fmBlogPostRepository = fmBlogPostRepository;
        _fmBlogPostAuthorRepository = fmBlogPostAuthorRepository;
        _entityName = typeof(BlogPost).Name;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods


    /// <summary>
    /// Gets blog posts
    /// </summary>
    /// <param name = "storeId">The store identifier</param>
    /// <param name = "pageIndex">pageIndex</param>
    /// <param name = "pageSize">pageSize</param>
    /// <returns>Blog posts</returns>
    public async Task<IPagedList<BlogPost>> GetBlogPostsAsync(int storeId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.BlogPostsKey, storeId, pageIndex, pageSize);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            return await _blogPostRepository.GetAllPagedAsync(query =>
            {
                query = from bp in query
                        join fmbp in _fmBlogPostRepository.Table on bp.Id equals fmbp.BlogPostId
                        where ((bp.StartDateUtc <= currentDate && bp.StartDateUtc.HasValue) && (currentDate <= bp.EndDateUtc || !bp.EndDateUtc.HasValue))
                        orderby bp.StartDateUtc descending
                        select bp;

                if (storeId > 0)
                {
                    //Store mapping
                    query = from bp in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = bp.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                            from sm in bp_sm.DefaultIfEmpty()
                            where !bp.LimitedToStores || storeId == sm.StoreId
                            select bp;
                }

                return query;
            }, pageIndex, pageSize);
        });
    }

    /// <summary>
    /// Gets blog posts
    /// </summary>        
    /// <param name = "storeId">The store identifier</param>
    /// <param name = "blogAuthorId" > The blog author identifier; pass 0 to load all records</param>
    /// <returns>Blog posts</returns>
    public async Task<IList<BlogPost>> GetBlogPostsByAuthorIdAsync(int storeId, int blogAuthorId = 0)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.BlogPostsByAuthorIdCacheKey, storeId, blogAuthorId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            var query = from bp in _blogPostRepository.Table
                        where ((bp.StartDateUtc <= currentDate && bp.StartDateUtc.HasValue)
                         && (currentDate <= bp.EndDateUtc || !bp.EndDateUtc.HasValue))
                        select bp;

            if (storeId > 0)
            {
                //Store mapping
                query = from bp in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = bp.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                        from sm in bp_sm.DefaultIfEmpty()
                        where !bp.LimitedToStores || storeId == sm.StoreId
                        select bp;
            }

            if (blogAuthorId > 0)
            {
                query = (from bp in query
                         join bcm in _fmBlogPostAuthorRepository.Table on bp.Id equals bcm.BlogPostId
                         where bcm.AuthorId == blogAuthorId
                         select bp);
            }

            return await query.OrderByDescending(x => x.StartDateUtc).ToListAsync();
        });
    }

    /// <summary>
    /// Gets blog post
    /// </summary>
    /// <param name = "storeId">The store identifier</param>
    /// <param name = "blogPostId">Blog post identifier</param>
    /// <returns>Blog post</returns>
    public async Task<BlogPost> GetBlogPostByIdAsync(int storeId, int blogPostId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.BlogPostByIdCacheKey, storeId, blogPostId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            var query = from bp in _blogPostRepository.Table
                        where bp.Id == blogPostId &&
                        ((bp.StartDateUtc <= currentDate && bp.StartDateUtc.HasValue) &&
                        (currentDate <= bp.EndDateUtc || !bp.EndDateUtc.HasValue))
                        select bp;

            if (storeId > 0)
            {
                //Store mapping
                query = from bp in query
                        join sm in _storeMappingRepository.Table
                        on new { c1 = bp.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                        from sm in bp_sm.DefaultIfEmpty()
                        where !bp.LimitedToStores || storeId == sm.StoreId
                        select bp;
            }

            return await query.FirstOrDefaultAsync();
        });
    }

    /// <summary>
    /// Get latest blog post list
    /// </summary>
    /// <param name="blogPostId">blog post identity</param>
    /// <returns>latest blog post list</returns>
    public async Task<IList<BlogPost>> GetLatestBlogsAsync(int blogPostId = 0, int storeId = 0, int numberOfBlogs = 0)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.AllLatestBlogsCacheKey, blogPostId, storeId, numberOfBlogs);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var query = await (from b in _blogPostRepository.Table
                               where b.Id != blogPostId &&
                               (b.StartDateUtc <= DateTime.UtcNow && b.StartDateUtc.HasValue)
                               && (DateTime.UtcNow <= b.EndDateUtc || !b.EndDateUtc.HasValue)
                               orderby b.StartDateUtc descending
                               select b).ToListAsync();

            if (storeId > 0)
            {
                //Store mapping
                query = (from b in query
                         join sm in _storeMappingRepository.Table
                         on new { c1 = b.Id, c2 = _entityName } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into bp_sm
                         from sm in bp_sm.DefaultIfEmpty()
                         where !b.LimitedToStores || storeId == sm.StoreId
                         select b).ToList();
            }

            if (query != null && numberOfBlogs > 0)
                return query.Take(numberOfBlogs).ToList();

            return null;
        });
    }

    public async Task<FMBlogPost> GetFMBlogByBlogPostIdAsync(int blogPostId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.FMBlogByBlogPostIdCacheKey, blogPostId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            return await _fmBlogPostRepository.Table.Where(b => b.BlogPostId == blogPostId).FirstOrDefaultAsync();
        });
    }

    public async Task<IList<FMBlogPostAuthor>> GetBlogPostAuthorsByBlogPostIdAsync(int blogPostId)
    {
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(CacheKeys.FMBlogPostAuthorByBlogPostIdCacheKey, blogPostId);
        return await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var query = _fmBlogPostAuthorRepository.Table.Where(bc => bc.BlogPostId == blogPostId);
            return await query.ToListAsync();
        });
    }

    #endregion

}
