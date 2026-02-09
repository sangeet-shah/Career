using Career.Web.Domains.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Career.Web.Infrastructure.Filters;

/// <summary>
/// Global authorization filter that enforces authentication only when the
/// Middleware reports the site is a test site. If IsTestSite == false the
/// filter allows all requests. When IsTestSite == true the filter redirects
/// unauthenticated users to the Customer Login page.
/// </summary>
public class TestSiteAuthorizeFilter : IAsyncAuthorizationFilter
{
    public TestSiteAuthorizeFilter()
    {
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Allow Login page and the Customer controller's actions (so login itself is reachable)
        var controller = context.RouteData.Values["controller"]?.ToString();
        var action = context.RouteData.Values["action"]?.ToString();
        if (!string.IsNullOrEmpty(controller) && string.Equals(controller, "Customer", StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var isTestSite = config.GetValue<bool?>("IsTestSite") ?? false;
            if (!isTestSite)
                return; // allow when not a test site

            // test site -> require authentication cookie
            var cookie = context.HttpContext.Request.Cookies?[NopDefaults.AuthenticationKey];
            if (string.IsNullOrEmpty(cookie))
            {
                context.Result = new RedirectToActionResult("Login", "Customer", null);
            }
        }
        catch
        {
            // If we cannot determine test-mode, deny access to be safe
            context.Result = new RedirectToActionResult("Login", "Customer", null);
        }
    }
}

