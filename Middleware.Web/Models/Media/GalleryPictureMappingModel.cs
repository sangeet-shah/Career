namespace Middleware.Web.Models.Media;

public record GalleryPictureMappingModel
{
    public int GalleryId { get; set; }
    
    public string Title { get; set; }

    public string AltText { get; set; }

    public string ShortDescription { get; set; }

    public int PictureId { get; set; }

    public string PictureURL { get; set; }

    public string Link { get; set; }

    public int DisplayOrder { get; set; }
}
