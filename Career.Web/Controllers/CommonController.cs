using Career.Web.Domains.Common;
using Career.Web.Domains.LegalPages;
using Career.Web.Models.Api;
using Career.Web.Models.Common;
using Career.Web.Models.OffersPromotions;
using Career.Web.Services.ApiClient;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class CommonController : BaseController
{
    private readonly IApiClient _apiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly AppSettings _appSettings;

    public CommonController(
        IApiClient apiClient,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHostEnvironment,
        AppSettings appSettings)
    {
        _apiClient = apiClient;
        _httpContextAccessor = httpContextAccessor;
        _webHostEnvironment = webHostEnvironment;
        _appSettings = appSettings;
    }

    public async Task<IActionResult> PageNotFound()
    {
        Response.StatusCode = 404;
        Response.ContentType = "text/html";

        var model = new PageNotFoundModel();
        var storeRes = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = storeRes?.Id ?? 0;

        var topicRes = await _apiClient.GetAsync<GenericAttributeValueResponse>("api/GenericAttribute/GetAttribute", new
        {
            entityId = (int)LegalPageEnum.PageNotFound,
            keyGroup = "LegalPage",
            key = ContentManagement.GENERIC_ATTRIBUTE_KEY_BODY,
            storeId
        });
        var topic = topicRes?.Value;

        if (!string.IsNullOrWhiteSpace(topic))
        {
            var cdnSettings = await _apiClient.GetAsync<NopAdvanceCDNSettingsDto>("api/Setting/LoadSetting", new
            {
                typeName = "Middleware.Web.Domains.CDN.NopAdvanceCDNSettings, Middleware.Web",
                storeId
            });
            var cdnUrl = cdnSettings?.CDNImageUrl?.TrimEnd('/') ?? "";

            var mobileRes = await _apiClient.GetAsync<GenericAttributeValueResponse>("api/GenericAttribute/GetAttribute", new
            {
                entityId = (int)LegalPageEnum.PageNotFound,
                keyGroup = "LegalPage",
                key = ContentManagement.GENERIC_ATTRIBUTE_KEY_GROUP,
                storeId
            });
            var mobileBody = mobileRes?.Value;

            if (!string.IsNullOrEmpty(topic))
                model.Body = topic.Replace("/images/uploaded/", cdnUrl + "/images/uploaded/");
            if (!string.IsNullOrEmpty(mobileBody))
                model.MobileBody = mobileBody.Replace("/images/uploaded/", cdnUrl + "/images/uploaded/");
        }

        return View(model);
    }

    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionFeature != null)
        {
            var ex = exceptionFeature.Error;
            _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer] = Uri.UriSchemeHttps + Uri.SchemeDelimiter + _httpContextAccessor.HttpContext.Request.Host.Value + exceptionFeature.Path;
            // Log locally; no Career.Data ILogService
            // Consider using ILogger<CommonController> if you want structured logging
        }

        return View();
    }

    public async Task<IActionResult> ClearCache()
    {
        if (!Request.Headers.TryGetValue("XApiKey", out var apiKeyHeader))
            return Unauthorized("Missing API key.");

        var apiKey = apiKeyHeader.ToString();
        var storeRes = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = storeRes?.Id ?? 0;

        await _apiClient.PostAsync<ClearCacheRequest, object>("api/Common/ClearCache", new ClearCacheRequest { StoreId = storeId, ApiKey = apiKey });
        return Ok();
    }

    public async Task<IActionResult> WeeklyAds()
    {
        var storeRes = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = storeRes?.Id ?? 0;
        // AdvertisementTypeEnum.Weekly = 1 typically; use same value as Middleware
        var ads = await _apiClient.GetAsync<AdvertisementDto[]>("api/Advertisement/GetActiveAdvertisementByStoreAndType", new { adTypeId = 1, storeId });
        if (ads == null || !ads.Any())
            return InvokeHttp404();

        var model = new WeeklyAdModel();
        foreach (var ad in ads)
            model.URLs.Add(new SelectListItem { Text = ad.Url, Value = ad.Id.ToString() });

        var setting = await _apiClient.GetAsync<AdvertisementSettingDto>("api/Setting/LoadSetting", new
        {
            typeName = "Middleware.Web.Domains.Advertisements.AdvertisementSettingModel, Middleware.Web",
            storeId
        });
        model.EcommPlugin = setting?.WeeklyAdEcommPlugin;

        return View(model);
    }

    public async Task<IActionResult> CatalogAds()
    {
        var storeRes = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = storeRes?.Id ?? 0;
        var ads = await _apiClient.GetAsync<AdvertisementDto[]>("api/Advertisement/GetActiveAdvertisementByStoreAndType", new { adTypeId = 2, storeId });
        if (ads == null || !ads.Any())
            return InvokeHttp404();

        var model = new CatalogAdModel();
        foreach (var ad in ads)
            model.URLs.Add(new SelectListItem { Text = ad.Url, Value = ad.Id.ToString() });

        var setting = await _apiClient.GetAsync<AdvertisementSettingDto>("api/Setting/LoadSetting", new
        {
            typeName = "Middleware.Web.Domains.Advertisements.AdvertisementSettingModel, Middleware.Web",
            storeId
        });
        model.EcommPlugin = setting?.CatalogAdEcommPlugin;

        return View(model);
    }

    public ContentResult RobotsTextFile()
    {
        if (_appSettings.IsTestSite)
            return Content("User-agent: * " + Environment.NewLine + "Disallow: / ");

        var sb = new StringBuilder();
        var root = _webHostEnvironment.ContentRootPath;
        var robotsFilePath = Path.Combine(root, "robots.custom.txt");
        if (System.IO.File.Exists(robotsFilePath))
        {
            var robotsFileContent = System.IO.File.ReadAllText(robotsFilePath, Encoding.UTF8);
            sb.Append(robotsFileContent);
        }

        return Content(sb.ToString());
    }

    public async Task<IActionResult> OffersPromotions()
    {
        var storeRes = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = storeRes?.Id ?? 0;
        var cdnSettings = await _apiClient.GetAsync<NopAdvanceCDNSettingsDto>("api/Setting/LoadSetting", new
        {
            typeName = "Middleware.Web.Domains.CDN.NopAdvanceCDNSettings, Middleware.Web",
            storeId
        });
        var cdnUrl = cdnSettings?.CDNImageUrl?.TrimEnd('/') ?? "";

        var list = await _apiClient.GetAsync<OffersPromotionDto[]>("api/OffersPromotions/GetAllActive");
        var model = new OffersPromotionListModel
        {
            OffersPromotions = (list ?? Array.Empty<OffersPromotionDto>()).Select(x => new OffersPromotionModel
            {
                Id = x.Id,
                Title = x.Title,
                Anchor = x.Anchor,
                Description = x.Description?.Replace("/images/uploaded/", cdnUrl + "/images/uploaded/") ?? ""
            }).ToList()
        };

        return View(model);
    }
}

internal class ClearCacheRequest
{
    public int StoreId { get; set; }
    public string ApiKey { get; set; }
}
