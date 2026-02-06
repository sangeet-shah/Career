using Career.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains.Advertisements;
using Career.Data.Domains.CDN;
using Career.Data.Domains.Common;
using Career.Data.Domains.LegalPages;
using Career.Data.Domains.Stores;
using Career.Data.Infrastructure;
using Career.Data.Services.Advertisements;
using Career.Data.Services.Common;
using Career.Data.Services.Logs;
using Career.Data.Services.Media;
using Career.Data.Services.OffersPromotions;
using Career.Data.Services.Settings;
using Career.Data.Services.Stores;
using Career.Web.Models.Common;
using Career.Web.Models.OffersPromotions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class CommonController : BaseController
{
    #region Fields

    private readonly ILogService _logService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IAdvertisementService _advertisementService;
    private readonly ISettingService _settingService;
    private readonly INopFileProvider _fileProvider;
    private readonly AppSettings _appSettings;
    private readonly IOffersPromotionsService _offersPromotionsService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public CommonController(ILogService logService,
        IHttpContextAccessor httpContextAccessor,
        IStaticCacheManager staticCacheManager,
        IAdvertisementService advertisementService,
        ISettingService settingService,
        INopFileProvider fileProvider,
        AppSettings appSettings,
        IOffersPromotionsService offersPromotionsService,
        IGenericAttributeService genericAttributeService,
        IStoreService storeService)
    {
        _logService = logService;
        _httpContextAccessor = httpContextAccessor;
        _staticCacheManager = staticCacheManager;
        _advertisementService = advertisementService;
        _settingService = settingService;
        _fileProvider = fileProvider;
        _appSettings = appSettings;
        _offersPromotionsService = offersPromotionsService;
        _genericAttributeService = genericAttributeService;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> PageNotFound()
    {
        Response.StatusCode = 404;
        Response.ContentType = "text/html";

        var model = new PageNotFoundModel();
        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;        
        var topic = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.PageNotFound }, ContentManagement.GENERIC_ATTRIBUTE_KEY_BODY, nameof(LegalPage), storeId: storeId);
        if (!string.IsNullOrWhiteSpace(topic))
        {
            var cndSettings = await _settingService.LoadSettingAsync<NopAdvanceCDNSettings>(storeId);
            var mobileBody = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.PageNotFound }, ContentManagement.GENERIC_ATTRIBUTE_KEY_GROUP, nameof(LegalPage), storeId: storeId);
            if (!string.IsNullOrEmpty(topic))
                model.Body = topic.Replace("/images/uploaded/", cndSettings.CDNImageUrl + "/images/uploaded/");
            if (!string.IsNullOrEmpty(mobileBody))
                model.MobileBody = mobileBody.Replace("/images/uploaded/", cndSettings.CDNImageUrl + "/images/uploaded/");
        }

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
            _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer] = Uri.UriSchemeHttps + Uri.SchemeDelimiter + _httpContextAccessor.HttpContext.Request.Host.Value + exceptionFeature.Path;

            // TODO: Do something with the exception
            // Log it with Serilog?
            // Send an e-mail, text, fax, or carrier pidgeon?  Maybe all of the above?
            // Whatever you do, be careful to catch any exceptions, otherwise you'll end up with a blank page and throwing a 500
            _logService.Error(exceptionThatOccurred.Message, exceptionThatOccurred);
        }

        return View();
    }

    public async Task<IActionResult> ClearCache()
    {
        if (!Request.Headers.TryGetValue("XApiKey", out var apiKey))
            return Unauthorized("Missing API key.");

        var store = await _storeService.GetCurrentStoreAsync();
        var fmStoreSettings = await _settingService.LoadSettingAsync<FMStoreSettings>(store?.Id ?? 0);
        if (!fmStoreSettings.ApiGatewayKey.Equals(apiKey, StringComparison.OrdinalIgnoreCase))
            return Unauthorized("Invalid API key.");

        await _staticCacheManager.RemoveByPrefixAsync(CacheKeys.PatternCacheKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.ImageKitCacheKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.HomePageBannerCacheKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.GenericTopicCacheKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.StatesKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.CitiesKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.DepartmentsKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.UrlRecordCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(CacheKeys.JobsPattern);
        await _staticCacheManager.RemoveAsync(CacheKeys.FaviconIconCacheKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.PictureCacheKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.SitemapCacheKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.OffersPromotionsKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.AllLocationsKey);
        await _staticCacheManager.RemoveByPrefixAsync(CacheKeys.LocationHoursCacheKeyPattern);
        await _staticCacheManager.RemoveAsync(CacheKeys.AllStateProvincesKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.AllVendorKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.LocaleStringResourceCacheKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.CareerBrandsKey);
        await _staticCacheManager.RemoveAsync(CacheKeys.AllAdvertisementKey);
        await _staticCacheManager.RemoveByPrefixAsync(CacheKeys.BlogPostsKeyPattern);
        await _staticCacheManager.RemoveAsync(CacheKeys.GenericAttributeCacheKey);
        await _staticCacheManager.RemoveByPrefixAsync(CacheKeys.GenericAttributeKeyPattern);
        await _staticCacheManager.RemoveByPrefixAsync(CacheKeys.FMCustomerCacheKeyPattern);
        await _staticCacheManager.RemoveByPrefixAsync(CacheKeys.BannersKeyPattern);
        await _staticCacheManager.RemoveByPrefixAsync(CacheKeys.CorporateGalleryPictureCacheKeyPattern);
        

        await _staticCacheManager.ClearAsync();
        return Ok();
    }

    public async Task<IActionResult> WeeklyAds()
    {
        var model = new WeeklyAdModel();
        var weeklyAds = await _advertisementService.GetActiveAdvertisementByStoreAndTypeAsync((int)AdvertisementTypeEnum.Weekly, (await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
        if (!weeklyAds.Any())
            return InvokeHttp404();

        foreach (var weeklyAd in weeklyAds)
            model.URLs.Add(new SelectListItem { Text = weeklyAd.Url, Value = weeklyAd.Id.ToString() });

        var advertisementSetting = await _settingService.LoadSettingAsync<AdvertisementSettingModel>();
        model.EcommPlugin = advertisementSetting.WeeklyAdEcommPlugin;

        return View(model);
    }

    public async Task<IActionResult> CatalogAds()
    {
        var model = new CatalogAdModel();
        var catalogs = await _advertisementService.GetActiveAdvertisementByStoreAndTypeAsync((int)AdvertisementTypeEnum.Catalog, (await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
        if (!catalogs.Any())
            return InvokeHttp404();

        foreach (var catalog in catalogs)
            model.URLs.Add(new SelectListItem { Text = catalog.Url, Value = catalog.Id.ToString() });

        var catalogSettings = await _settingService.LoadSettingAsync<AdvertisementSettingModel>();
        model.EcommPlugin = catalogSettings.CatalogAdEcommPlugin;

        return View(model);
    }

    public ContentResult RobotsTextFile()
    {
        if (_appSettings.IsTestSite)
            return Content("User-agent: * " + Environment.NewLine + "Disallow: / ");

        var sb = new StringBuilder();
        var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.custom.txt");
        if (_fileProvider.FileExists(robotsFilePath))
        {
            //the robots.txt file exists
            var robotsFileContent = _fileProvider.ReadAllText(robotsFilePath, Encoding.UTF8);
            sb.Append(robotsFileContent);
        }

        return Content(sb.ToString());
    }

    public async Task<IActionResult> OffersPromotions()
    {
        var nopAdvanceCDNSettings = await _settingService.LoadSettingAsync<NopAdvanceCDNSettings>((await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
        var model = new OffersPromotionListModel
        {
            OffersPromotions = (await _offersPromotionsService.GetAllActiveOffersPromotionsCachedAsync()).Select(x => new OffersPromotionModel
            {
                Id = x.Id,
                Title = x.Title,
                Anchor = x.Anchor,
                Description = x.Description.Replace("/images/uploaded/", nopAdvanceCDNSettings.CDNImageUrl.TrimEnd('/') + "/images/uploaded/")
            }).ToList()
        };

        return View(model);
    }

    #endregion
}