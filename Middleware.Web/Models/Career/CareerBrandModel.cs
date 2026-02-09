namespace Middleware.Web.Models.Career;

public record CareerBrandModel 
{
    public int Id { get; set; }

    public string PictureUrl { get; set; }

    public string Description { get; set; }

    public string AltAttribute { get; set; }

    public string TitleAttribute { get; set; }

    public string Url { get; set; }
}
