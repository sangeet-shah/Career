namespace Middleware.Web.Models.Media;

public class PictureUploadResult
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public int PictureId { get; set; }

    public string? Image { get; set; }
}
