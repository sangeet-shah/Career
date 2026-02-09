namespace Middleware.Web.Domains.Career;

public class CareerBrand : BaseEntity
{
    /// <summary>
    /// Gets or sets PictureId
    /// </summary>
    public int PictureId { get; set; }

    /// <summary>
    /// Gets or sets description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a url
    /// </summary>
    public string Url { get; set; }
}
