namespace Career.Data.Domains.CorporateManagement;

public class CorporateGalleryPicture : BaseEntity
{
    /// <summary>
    /// Gets or sets the corporate gallery identifier
    /// </summary>
    public int CorporateGalleryId { get; set; }

    /// <summary>
    /// Gets or sets the picture identifier
    /// </summary>
    public int PictureId { get; set; }

    /// <summary>
    /// Gets or sets the short description
    /// </summary>
    public string ShortDescription { get; set; }

    /// <summary>
    /// Gets or sets the link url
    /// </summary>
    public string LinkUrl { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}
