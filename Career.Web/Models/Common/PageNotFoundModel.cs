namespace Career.Web.Models.Common;

public record PageNotFoundModel
{
    public string Body { get; set; }

    public bool IsMobileDevice { get; set; }

    public string MobileBody { get; set; }
}