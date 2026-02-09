using Middleware.Web.Domains.Blogs;
using Middleware.Web.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Blogs;

/// <summary>
/// Blog service interface
/// </summary>
public interface IBlogService
{
    /// <summary>
    /// Gets blog posts
    /// </summary>
    /// <param name = "storeId">The store identifier</param>
    /// <param name = "pageIndex">pageIndex</param>
    /// <param name = "pageSize">pageSize</param>
    /// <returns>Blog posts</returns>
    Task<IPagedList<BlogPost>> GetBlogPostsAsync(int storeId, int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Gets blog posts
    /// </summary>
    /// <param name = "storeId">The store identifier</param>
    /// <param name = "blogAuthorId">The blog author identifier; pass 0 to load all records</param>
    /// <returns>Blog posts</returns>
    Task<IList<BlogPost>> GetBlogPostsByAuthorIdAsync(int storeId, int blogAuthorId = 0);

    /// <summary>
    /// Gets blog post
    /// </summary>
    /// <param name = "storeId">The store identifier</param>
    /// <param name = "blogPostId">Blog post identifier</param>
    /// <returns>Blog post</returns>
    Task<BlogPost> GetBlogPostByIdAsync(int storeId, int blogPostId);

    /// <summary>
    /// Get latest blog post list
    /// </summary>
    /// <param name="blogPostId">blog post identity</param>
    /// <returns>latest blog post list</returns>
    Task<IList<BlogPost>> GetLatestBlogsAsync(int blogPostId = 0, int storeId = 0, int numberOfBlogs = 0);

    Task<FMBlogPost> GetFMBlogByBlogPostIdAsync(int blogPostId);

    Task<IList<FMBlogPostAuthor>> GetBlogPostAuthorsByBlogPostIdAsync(int blogPostId);
}
