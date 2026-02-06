using Career.Web.Models.Common;
using Career.Web.Models.Media;
using System.Collections.Generic;

namespace Career.Web.Models.Blogs;

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
