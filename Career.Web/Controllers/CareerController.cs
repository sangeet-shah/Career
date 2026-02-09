using Career.Web.Infrastructure;
using Career.Web.Models.Career;
using Career.Web.Models.Security;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace Career.Web.Controllers;

public class CareerController : BaseController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IApiClient _apiClient;
    private readonly IUserAgentHelper _userAgentHelper;

    public CareerController(IHttpContextAccessor httpContextAccessor,
        IApiClient apiClient,
        IUserAgentHelper userAgentHelper)
    {
        _httpContextAccessor = httpContextAccessor;
        _apiClient = apiClient;
        _userAgentHelper = userAgentHelper;
    }    

    #region Utilities

    public static Dictionary<string, string> ParseQueryString(String query)
    {
        var queryDict = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(query))
            return queryDict;

        var tokens = query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (String token in tokens)
        {
            var parts = token.Split('=', 2); // split only once
            var key = parts.Length > 0 ? parts[0].Trim() : "";
            var value = parts.Length > 1 ? HttpUtility.UrlDecode(parts[1]).Trim() : "";

            // avoid empty key like "?="
            if (!string.IsNullOrEmpty(key))
            {
                queryDict[key] = value;
            }
        }
        return queryDict;
    }

    #endregion

    #region Method

    public async Task<IActionResult> List(CareerSearchModel searchModel)
    {
        var apiModel = await _apiClient.GetAsync<CareerSearchModel>("api/Career/List", new
        {
            searchModel.State,
            searchModel.City,
            searchModel.JobCategory,
            searchModel.Page,
            searchModel.PageSize
        });
        var maybe = NotFoundIfNull(apiModel);
        if (maybe != null) return maybe;

        apiModel.IsMobile = _userAgentHelper.IsMobileDevice();
        if (apiModel.IsMobile)
        {
            apiModel.AvailableStates.Insert(0, new SelectListItem { Text = "State", Value = "0" });
            apiModel.AvailableCities.Insert(0, new SelectListItem { Text = "City", Value = "0" });
            apiModel.AvailableJobCategories.Insert(0, new SelectListItem { Text = "Job Category", Value = "0" });
        }

        apiModel.Parameters = ParseQueryString(_httpContextAccessor.HttpContext.Request.QueryString.Value);

        return View(apiModel);
    }

    public async Task<IActionResult> CareerDetail(string id)
    {
        if (string.IsNullOrEmpty(id))
            return InvokeHttp404();

        CareerJobDetailModel model;
        try
        {
            model = await _apiClient.GetAsync<CareerJobDetailModel>($"api/Career/CareerDetail/{id}");
            var maybe = NotFoundIfNull(model);
            if (maybe != null) return maybe;
        }
        catch
        {
            return InvokeHttp404();
        }

        model.IsMobile = _userAgentHelper.IsMobileDevice();
        var httpContext = _httpContextAccessor.HttpContext.Request;
        model.baseURL = string.Format("{0}://{1}", httpContext.Scheme, httpContext.Host.Value);
        model.CurrentPageURL = string.Format("{0}://{1}{2}{3}", httpContext.Scheme, httpContext.Host.Value, httpContext.Path, httpContext.QueryString);

        return View(model);
    }

    public async Task<IActionResult> SitemapXml()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
        var siteMap = await _apiClient.GetAsync<string>("api/Career/SitemapXml", new { baseUrl });
        if (!string.IsNullOrEmpty(siteMap))
            return Content(siteMap, "text/xml");

        return View();
    }

    #endregion
}
