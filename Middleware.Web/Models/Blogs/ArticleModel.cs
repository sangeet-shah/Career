namespace Middleware.Web.Models.Blogs;

public record ArticleModel
{
    public ArticleModel()
    {
        BlogPost = new BlogPostModel();
        BlogPostList = new List<BlogPostModel>();
        BlogAuthors = new List<BlogAuthorDetailsModel>();
    }

    public BlogPostModel BlogPost { get; set; }

    public IList<BlogPostModel> BlogPostList { get; set; }

    public string HostURL { get; set; }

    public string CurrentPageURL { get; set; }

    public IList<BlogAuthorDetailsModel> BlogAuthors { get; set; }
}
