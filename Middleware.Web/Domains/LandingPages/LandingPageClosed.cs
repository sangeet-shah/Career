namespace Middleware.Web.Domains.LandingPages;
public class LandingPageClosed : BaseEntity
{
    public int LandingPageId { get; set; }
    public int BannerPictureId { get; set; }
    public int BannerMobilePictureId { get; set; }
    public string Description { get; set; }
    public string MobileDescription { get; set; }
    public bool EventListEnabled { get; set; }
    public bool NewsLetterEnabled { get; set; }
    public bool SMSEnabled { get; set; }
    public bool TitleEnabled { get; set; }
}