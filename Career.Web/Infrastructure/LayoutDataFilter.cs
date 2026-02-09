using Career.Web.Models.Common;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Career.Web.Infrastructure;

/// <summary>
/// Fetches layout data (store + SEO + IsTestSite) from the API and sets ViewData
/// so _Layout.cshtml and _GoogleAnalytics.cshtml do not inject ISettingService/IStoreService/AppSettings.
/// </summary>
public class LayoutDataFilter : IAsyncResultFilter
{
    private readonly IApiClient _apiClient;

    public LayoutDataFilter(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ViewResult viewResult)
        {
            try
            {
                var layoutData = await _apiClient.GetAsync<LayoutDataResponse>("api/Common/GetLayoutData");
                viewResult.ViewData["LayoutData"] = layoutData ?? new LayoutDataResponse();
            }
            catch
            {
                viewResult.ViewData["LayoutData"] = new LayoutDataResponse();
            }
        }

        await next();
    }
}
