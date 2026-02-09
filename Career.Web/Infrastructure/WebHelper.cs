using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Career.Web.Infrastructure;

public class WebHelper : IWebHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WebHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetStoreLocation()
    {
        if (_httpContextAccessor?.HttpContext?.Request == null)
            return string.Empty;
        var host = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
        return string.IsNullOrEmpty(host) ? string.Empty : Uri.UriSchemeHttps + Uri.SchemeDelimiter + host;
    }

    public string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false)
    {
        if (_httpContextAccessor?.HttpContext?.Request == null)
            return string.Empty;
        var storeLocation = GetStoreLocation();
        var path = _httpContextAccessor.HttpContext.Request.Path;
        var pageUrl = $"{storeLocation.TrimEnd('/')}{path}";
        if (includeQueryString)
            pageUrl += _httpContextAccessor.HttpContext.Request.QueryString;
        if (lowercaseUrl)
            pageUrl = pageUrl.ToLowerInvariant();
        return pageUrl;
    }
}
