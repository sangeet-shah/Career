using System;

namespace Career.Data.Domains.Blogs;
public class FMBlogPost : BaseEntity
{
    /// <summary>
    /// Gets or sets the blogpost id
    /// </summary>
    public int BlogPostId { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail picture id
    /// </summary>
    public int ThumbnailPictureId { get; set; }

    /// <summary>
    /// Gets or sets the highlight picture id
    /// </summary>
    public int HighlightPictureId { get; set; }

    /// <summary>
    /// Gets or sets the pin blog
    /// </summary>
    public bool PinBlog { get; set; }

    /// <summary>
    /// Gets or sets the seo picture id1
    /// </summary>
    public int SEOPictureId1 { get; set; }

    /// <summary>
    /// Gets or sets the seo picture id2
    /// </summary>
    public int SEOPictureId2 { get; set; }

    /// <summary>
    /// Gets or sets the seo picture id3
    /// </summary>
    public int SEOPictureId3 { get; set; }

    /// <summary>
    /// Gets or sets the modified date
    /// </summary>
    public DateTime? ModifiedDateUtc { get; set; }
    public int MostViewCount { get; set; }
}
