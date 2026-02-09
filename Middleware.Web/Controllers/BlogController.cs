using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Services.Blogs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    /// <summary>
    /// Get blog posts
    /// </summary>
    [HttpGet("GetBlogPosts")]
    public async Task<IActionResult> GetBlogPosts([FromQuery] int storeId, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = int.MaxValue)
    {
        var posts = await _blogService.GetBlogPostsAsync(storeId, pageIndex, pageSize);
        return Ok(posts);
    }

    /// <summary>
    /// Get blog posts with paging metadata
    /// </summary>
    [HttpGet("GetBlogPostsPaged")]
    public async Task<IActionResult> GetBlogPostsPaged([FromQuery] int storeId, [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = int.MaxValue)
    {
        var posts = await _blogService.GetBlogPostsAsync(storeId, pageIndex, pageSize);
        var response = new BlogPostsPagedResponse
        {
            TotalCount = posts.TotalCount,
            TotalPages = posts.TotalPages,
            PageIndex = posts.PageIndex,
            PageSize = posts.PageSize,
            Items = posts
        };
        return Ok(response);
    }

    /// <summary>
    /// Get blog posts by author ID
    /// </summary>
    [HttpGet("GetBlogPostsByAuthorId")]
    public async Task<IActionResult> GetBlogPostsByAuthorId([FromQuery] int storeId, [FromQuery] int blogAuthorId = 0)
    {
        var posts = await _blogService.GetBlogPostsByAuthorIdAsync(storeId, blogAuthorId);
        return Ok(posts);
    }

    /// <summary>
    /// Get blog post by ID
    /// </summary>
    [HttpGet("GetBlogPostById")]
    public async Task<IActionResult> GetBlogPostById([FromQuery] int storeId, [FromQuery] int blogPostId)
    {
        var post = await _blogService.GetBlogPostByIdAsync(storeId, blogPostId);
        if (post == null)
            return NotFound();
        return Ok(post);
    }

    /// <summary>
    /// Get latest blogs
    /// </summary>
    [HttpGet("GetLatestBlogs")]
    public async Task<IActionResult> GetLatestBlogs([FromQuery] int blogPostId = 0, [FromQuery] int storeId = 0, [FromQuery] int numberOfBlogs = 0)
    {
        var blogs = await _blogService.GetLatestBlogsAsync(blogPostId, storeId, numberOfBlogs);
        return Ok(blogs);
    }

    /// <summary>
    /// Get FM blog by blog post ID
    /// </summary>
    [HttpGet("GetFMBlogByBlogPostId")]
    public async Task<IActionResult> GetFMBlogByBlogPostId([FromQuery] int blogPostId)
    {
        var blog = await _blogService.GetFMBlogByBlogPostIdAsync(blogPostId);
        if (blog == null)
            return NotFound();
        return Ok(blog);
    }

    /// <summary>
    /// Get blog post authors by blog post ID
    /// </summary>
    [HttpGet("GetBlogPostAuthorsByBlogPostId")]
    public async Task<IActionResult> GetBlogPostAuthorsByBlogPostId([FromQuery] int blogPostId)
    {
        var authors = await _blogService.GetBlogPostAuthorsByBlogPostIdAsync(blogPostId);
        return Ok(authors);
    }
}

public class BlogPostsPagedResponse
{
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public IList<Middleware.Web.Domains.Blogs.BlogPost> Items { get; set; }
}
