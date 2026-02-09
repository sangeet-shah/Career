namespace Middleware.Web.Models.Common;

/// <summary>
/// Layout data for _Layout.cshtml (store + SEO + app flags).
/// Consumed by Career.Web via api/Common/GetLayoutData.
/// </summary>
public class LayoutDataResponse
{
    public string DefaultTitle { get; set; } = string.Empty;
    public string HomepageDescription { get; set; } = string.Empty;
    public string PageTitleSeparator { get; set; } = string.Empty;
    public bool IsTestSite { get; set; }
}
