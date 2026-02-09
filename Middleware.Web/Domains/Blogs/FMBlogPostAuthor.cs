namespace Middleware.Web.Domains.Blogs;
public class FMBlogPostAuthor : BaseEntity
{
    /// <summary>
    /// Gets or sets the blogpost id
    /// </summary>
    public int BlogPostId { get; set; }

    /// <summary>
    /// Gets or sets the author id
    /// </summary>
    public int AuthorId { get; set; }
}
