using Career.Web.Models.Common;
using Career.Web.Models.OffersPromotions;
using Career.Web.Infrastructure;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class CommonController : BaseController
{
    private readonly IApiClient _apiClient;
    private readonly ILogger<CommonController> _logger;
    private readonly AppSettings _appSettings;
    private readonly IUserAgentHelper _userAgentHelper;
    private readonly IConfiguration _configuration;

    public CommonController(IApiClient apiClient,
        ILogger<CommonController> logger,
        AppSettings appSettings,
        IUserAgentHelper userAgentHelper,
        IConfiguration configuration)
    {
        _apiClient = apiClient;
        _logger = logger;
        _appSettings = appSettings;
        _userAgentHelper = userAgentHelper;
        _configuration = configuration;
    }

    #region Methods

    public async Task<IActionResult> PageNotFound()
    {
        Response.StatusCode = 404;
        Response.ContentType = "text/html";

        var model = await _apiClient.GetAsync<PageNotFoundModel>("api/Common/PageNotFound")
                   ?? new PageNotFoundModel();
        model.IsMobileDevice = _userAgentHelper.IsMobileDevice();

        return View(model);
    }

    public async Task<IActionResult> Error()
    {
        // Get the details of the exception that occurred
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionFeature != null)
        {
            // Get the exception that occurred
            Exception exceptionThatOccurred = exceptionFeature.Error;
            _logger.LogError(exceptionThatOccurred, "Unhandled exception at {Path}", exceptionFeature.Path);
        }

        return View();
    }

    public async Task<IActionResult> ClearCache()
    {
        if (!Request.Headers.TryGetValue("XApiKey", out var apiKey))
            return Unauthorized("Missing API key.");

        var expectedApiKey = _configuration["Api:XApiKey"];
        if (string.IsNullOrWhiteSpace(expectedApiKey) || !expectedApiKey.Equals(apiKey, StringComparison.OrdinalIgnoreCase))
            return Unauthorized("Invalid API key.");

        var result = await _apiClient.PostAsync<object, object>("api/Common/ClearCache", new { });
        if (result == null)
            return StatusCode(500, "Failed to clear cache.");

        return Ok();
    }

    public async Task<IActionResult> WeeklyAds()
    {
        var model = await _apiClient.GetAsync<WeeklyAdModel>("api/Common/WeeklyAds");
        if (model?.URLs == null || model.URLs.Count == 0)
            return InvokeHttp404();

        return View(model);
    }

    public async Task<IActionResult> CatalogAds()
    {
        var model = await _apiClient.GetAsync<CatalogAdModel>("api/Common/CatalogAds");
        if (model?.URLs == null || model.URLs.Count == 0)
            return InvokeHttp404();

        return View(model);
    }

    public async Task<ContentResult> RobotsTextFile()
    {
        if (_appSettings.IsTestSite)
            return Content("User-agent: * " + Environment.NewLine + "Disallow: / ");

        var content = await _apiClient.GetAsync<string>("api/Common/RobotsTextFile") ?? string.Empty;
        return Content(content);
    }

    public async Task<IActionResult> OffersPromotions()
    {
        var model = await _apiClient.GetAsync<OffersPromotionListModel>("api/Common/OffersPromotions");
        var maybe = NotFoundIfNull(model);
        if (maybe != null) return maybe;

        return View(model);
    }

    #endregion
}