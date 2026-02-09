using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Career.Web.Infrastructure;

/// <summary>
/// Forwards the current request's Cookie header to the API so auth context is preserved.
/// </summary>
public class ForwardCookiesHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ForwardCookiesHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cookie = _httpContextAccessor?.HttpContext?.Request?.Headers.Cookie.ToString();
        if (!string.IsNullOrEmpty(cookie))
            request.Headers.TryAddWithoutValidation("Cookie", cookie);

        return await base.SendAsync(request, cancellationToken);
    }
}
