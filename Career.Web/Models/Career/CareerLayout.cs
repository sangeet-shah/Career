namespace Career.Web.Models.Career;

public record CareerLayout
{
    public int JobId { get; set; }
    public string PageLogo { get; set; }
    public string PageBannerImage { get; set; }
    public string PageTitle { get; set; }
    public string ThankyouPageText { get; set; }
}
