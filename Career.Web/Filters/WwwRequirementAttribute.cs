using Career.Web.Domains.Seo;
using Career.Web.Models.Api;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Career.Web.Filters;

public sealed class WwwRequirementAttribute : TypeFilterAttribute
{
    public WwwRequirementAttribute() : base(typeof(WwwRequirementFilter)) { }

    private class WwwRequirementFilter : IAsyncAuthorizationFilter
    {
        private readonly IApiClient _apiClient;

        public WwwRequirementFilter(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private async Task RedirectRequestAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Host.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                return;

            var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
            var storeId = store?.Id ?? 0;
            var seoSettings = await _apiClient.GetAsync<SeoSettingsDto>("api/Setting/GetSeoSettings", new { storeId });
            if (seoSettings == null || (WwwRequirement)seoSettings.WwwRequirement != WwwRequirement.WithWww)
                return;

            var request = context.HttpContext.Request;
            var wwwHost = request.Host.Host.Substring(4);
            var newUrl = $"{request.Scheme}://{wwwHost}{request.PathBase}{request.Path}{request.QueryString}";
            context.Result = new RedirectResult(newUrl, true);
        }

        private async Task CheckWwwRequirementAsync(AuthorizationFilterContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            if (!context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                return;
            if (context.HttpContext.Connection.RemoteIpAddress != null && IPAddress.IsLoopback(context.HttpContext.Connection.RemoteIpAddress))
                return;

            await RedirectRequestAsync(context);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await CheckWwwRequirementAsync(context);
        }
    }
}
