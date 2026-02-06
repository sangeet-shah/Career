using Career.Data.Domains.Seo;
using Career.Data.Services.Settings;
using Career.Data.Services.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Career.Web.Filters;

/// <summary>
/// Represents a filter attribute that checks WWW at the beginning of the URL and properly redirect if necessary
/// </summary>
public sealed class WwwRequirementAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public WwwRequirementAttribute() : base(typeof(WwwRequirementFilter))
    {
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that checks WWW at the beginning of the URL and properly redirect if necessary
    /// </summary>
    private class WwwRequirementFilter : IAsyncAuthorizationFilter
    {

        #region Fields

        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public WwwRequirementFilter(IStoreService storeService,
            ISettingService settingService)
        {
            _storeService = storeService;
            _settingService = settingService;
        }

        #endregion


        #region Utilities

        /// <summary>
        /// Check WWW prefix at the beginning of the URL and properly redirect if necessary
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <param name="withWww">Whether URL must start with WWW</param>
        private void RedirectRequest(AuthorizationFilterContext context)
        {            
            if (context.HttpContext.Request.Host.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                var storeId = (_storeService.GetCurrentStoreAsync().Result)?.Id ?? 0;
                var seoSettings = _settingService.LoadSettingAsync<SeoSettings>(storeId).Result;
                if (seoSettings.WwwRequirement != WwwRequirement.WithWww)
                    return;

                var request = context.HttpContext.Request;
                var wwwHost = request.Host.Host.Substring(4);
                var newUrl = $"{request.Scheme}://{wwwHost}{request.PathBase}{request.Path}{request.QueryString}";
                context.Result = new RedirectResult(newUrl, true);
            }
        }

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        private void CheckWwwRequirement(AuthorizationFilterContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            //only in GET requests, otherwise the browser might not propagate the verb and request body correctly.
            if (!context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                return;

            //ignore this rule for localhost
            if (context.HttpContext.Connection.RemoteIpAddress == null && context.HttpContext.Connection.LocalIpAddress == null)
                return;

            //Ignore for localhost or ::1
            if (context.HttpContext.Connection.RemoteIpAddress != null && IPAddress.IsLoopback(context.HttpContext.Connection.RemoteIpAddress))
                return;

            RedirectRequest(context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized
        /// </summary>
        /// <param name="context">Authorization filter context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            CheckWwwRequirement(context);
            return Task.CompletedTask;
        }

        #endregion
    }

    #endregion
}