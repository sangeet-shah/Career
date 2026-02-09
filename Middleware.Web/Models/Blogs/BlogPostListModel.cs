using Middleware.Web.Models.Common;
using Middleware.Web.Models.Media;

namespace Middleware.Web.Models.Blogs;

public record BlogPostListModel : BaseSearchModel
{
    public BlogPostListModel()
    {
        BlogPostList = new List<BlogPostModel>();
        GalleryPictures = new List<GalleryPictureMappingModel>();
        // set default page size
        PageSize = 6;
    }

    public string PhotoGalleryTitle { get; set; }

    public IList<GalleryPictureMappingModel> GalleryPictures {get; set;}

    public IList<BlogPostModel> BlogPostList { get; set; }
}
