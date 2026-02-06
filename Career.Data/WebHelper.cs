using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;

namespace Career.Data;

/// <summary>
/// Represents a web helper
/// </summary>
public partial class WebHelper : IWebHelper
{
    #region Fields 

    private readonly IHttpContextAccessor _httpContextAccessor;

    #endregion

    #region Ctor

    public WebHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Check whether current HTTP request is available
    /// </summary>
    /// <returns>True if available; otherwise false</returns>
    protected  bool IsRequestAvailable()
    {
        if (_httpContextAccessor?.HttpContext == null)
            return false;

        try
        {
            if (_httpContextAccessor.HttpContext.Request == null)
                return false;
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get URL referrer if exists
    /// </summary>
    /// <returns>URL referrer</returns>
    public  string GetUrlReferrer()
    {
        if (!IsRequestAvailable())
            return string.Empty;

        //URL referrer is null in some case (for example, in IE 8)
        return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
    }

    /// <summary>
    /// Gets this page URL
    /// </summary>
    /// <param name="includeQueryString">Value indicating whether to include query strings</param>
    /// <param name="useSsl">Value indicating whether to get SSL secured page URL. Pass null to determine automatically</param>
    /// <param name="lowercaseUrl">Value indicating whether to lowercase URL</param>
    /// <returns>Page URL</returns>
    public  string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false)
    {
        if (!IsRequestAvailable())
            return string.Empty;

        //get store location
        var storeLocation = GetStoreLocation();

        //add local path to the URL
        var pageUrl = $"{storeLocation.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.Path}";

        //add query string to the URL
        if (includeQueryString)
            pageUrl = $"{pageUrl}{_httpContextAccessor.HttpContext.Request.QueryString}";

        //whether to convert the URL to lower case
        if (lowercaseUrl)
            pageUrl = pageUrl.ToLowerInvariant();

        return pageUrl;
    }

    /// <summary>
    /// Gets store location
    /// </summary>   
    /// <param name="useSsl">Value indicating whether to get SSL secured page URL. Pass null to determine automatically</param>
    /// <returns>Store location</returns>
    public  string GetStoreLocation()
    {
        return Uri.UriSchemeHttps + Uri.SchemeDelimiter + _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
    }
    /// <summary>
    /// GetCurrentIpAddress
    /// </summary>
    /// <returns></returns>
    public  string GetCurrentIpAddress()
    {
        if (!IsRequestAvailable())
            return string.Empty;

        if (!(_httpContextAccessor.HttpContext.Connection?.RemoteIpAddress is IPAddress remoteIp))
            return "";

        if (remoteIp.Equals(IPAddress.IPv6Loopback))
            return IPAddress.Loopback.ToString();

        return remoteIp.MapToIPv4().ToString();
    }

    #endregion
}
