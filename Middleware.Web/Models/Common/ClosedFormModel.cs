namespace Middleware.Web.Models.Common;

public record ClosedFormModel
{
    public bool TitleEnabled { get; set; }
    public string PictureUrl { get; set; }
    public string MobilePictureUrl { get; set; }
    public string Description { get; set; }
    public string MobileDescription { get; set; }
    public bool EventListEnabled { get; set; }
    public bool NewsLetterEnabled { get; set; }
    public bool SMSEnabled { get; set; }
}
