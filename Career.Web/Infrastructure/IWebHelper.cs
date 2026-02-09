namespace Career.Web.Infrastructure;

/// <summary>
/// Web helper for URL building (replaced Career.Data.IWebHelper when removing Career.Data).
/// </summary>
public interface IWebHelper
{
    string GetStoreLocation();
    string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);
}
