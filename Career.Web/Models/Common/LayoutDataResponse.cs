namespace Career.Web.Models.Common;

/// <summary>
/// Layout data from api/Common/GetLayoutData for _Layout.cshtml and _GoogleAnalytics.
/// </summary>
public class LayoutDataResponse
{
    public string DefaultTitle { get; set; } = string.Empty;
    public string HomepageDescription { get; set; } = string.Empty;
    public string PageTitleSeparator { get; set; } = string.Empty;
    public bool IsTestSite { get; set; }
}
