using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Middleware.Web.Infrastructure;
using Middleware.Web.Options;

namespace Middleware.Web.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAuthorizeAttribute : TypeFilterAttribute
{
    public ApiKeyAuthorizeAttribute(bool ignore = false)
        : base(typeof(ApiKeyAuthorizeFilter))
    {
        Arguments = new object[] { ignore };
    }
}

public sealed class ApiKeyAuthorizeFilter : IAsyncAuthorizationFilter
{
    private readonly MiddlewareOptions _options;
    private readonly bool _ignore;

    // ✅ THIS constructor MUST exist
    public ApiKeyAuthorizeFilter(
        IOptions<MiddlewareOptions> options,
        bool ignore)
    {
        _options = options.Value;
        _ignore = ignore;
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (_ignore)
            return Task.CompletedTask;

        if (!context.HttpContext.Request.Headers.TryGetValue(
            AuthenticationDefaults.API_KEY_NAME,
            out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                StatusCode = 401,
                Message = "X-Api-Key header is missing"
            });
            return Task.CompletedTask;
        }

        if (!string.Equals(extractedApiKey, _options.XApiKey, StringComparison.Ordinal))
        {
            context.Result = new ObjectResult(new
            {
                StatusCode = 403,
                Message = "Invalid API Key"
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
