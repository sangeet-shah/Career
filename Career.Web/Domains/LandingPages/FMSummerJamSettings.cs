using System;

namespace Career.Web.Domains.LandingPages;

/// <summary>
/// Summer Jam settings DTO (replaced Career.Data.Domains.LandingPages.FMSummerJamSettings).
/// </summary>
public class FMSummerJamSettings
{
    public string Title1 { get; set; }
    public string Title2 { get; set; }
    public string Title3 { get; set; }
    public int DailyLineUpImageId { get; set; }
    public string DailyLineUpWebImageIdUrl { get; set; }
    public int DailyLineUpMobileImageId { get; set; }
    public string DailyLineUpMobileImageIdUrl { get; set; }
    public int ViewPatioSetImageId { get; set; }
    public string ViewPatioSetWebImageIdUrl { get; set; }
    public int ViewPatioSetMobileImageId { get; set; }
    public string ViewPatioSetMobileImageIdUrl { get; set; }
    public string Disclaimer { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string MetaKeywords { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
}
