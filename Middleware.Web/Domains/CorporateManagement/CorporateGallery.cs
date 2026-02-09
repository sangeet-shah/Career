namespace Middleware.Web.Domains.CorporateManagement;

public class CorporateGallery : BaseEntity
{
    /// <summary>
    /// Gets or sets the title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the high lighted
    /// </summary>
    public bool Highlighted { get; set; }
}
