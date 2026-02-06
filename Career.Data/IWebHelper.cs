namespace Career.Data;

/// <summary>
/// Represents a web helper
/// </summary>
public partial interface IWebHelper
{
    /// <summary>
    /// Get URL referrer if exists
    /// </summary>
    /// <returns>URL referrer</returns>
    string GetUrlReferrer();

    /// <summary>
    /// Gets this page URL
    /// </summary>
    /// <param name="includeQueryString">Value indicating whether to include query strings</param>
    /// <param name="useSsl">Value indicating whether to get SSL secured page URL. Pass null to determine automatically</param>
    /// <param name="lowercaseUrl">Value indicating whether to lowercase URL</param>
    /// <returns>Page URL</returns>
    string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);

    /// <summary>
    /// Gets store location
    /// </summary>   
    /// <param name="useSsl">Value indicating whether to get SSL secured page URL. Pass null to determine automatically</param>
    /// <returns>Store location</returns>
    string GetStoreLocation();

    /// <summary>
    /// GetCurrentIpAddress
    /// </summary>
    /// <returns></returns>
    string GetCurrentIpAddress();
}
