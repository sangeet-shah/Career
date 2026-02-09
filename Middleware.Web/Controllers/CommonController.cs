using Middleware.Web.Data.Caching;
using Middleware.Web.Domains.Advertisements;
using Middleware.Web.Domains.CDN;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.LegalPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Middleware.Web.Filters;
using Middleware.Web.Infrastructure;
using Middleware.Web.Models.Common;
using Middleware.Web.Models.OffersPromotions;
using Middleware.Web.Services.Advertisements;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.OffersPromotions;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using Middleware.Web.Domains.Seo;
using System.Text;

namespace Middleware.Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class CommonController : ControllerBase
{
    #region Fields

    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IAdvertisementService _advertisementService;
    private readonly ISettingService _settingService;
    private readonly INopFileProvider _fileProvider;
    private readonly IOffersPromotionsService _offersPromotionsService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IStoreService _storeService;
    private readonly IConfiguration _configuration;

    #endregion

    #region Ctor

    public CommonController(IStaticCacheManager staticCacheManager,
        IAdvertisementService advertisementService,
        ISettingService settingService,
        INopFileProvider fileProvider,
        IOffersPromotionsService offersPromotionsService,
        IGenericAttributeService genericAttributeService,
        IStoreService storeService,
        IConfiguration configuration)
    {
        _staticCacheManager = staticCacheManager;
        _advertisementService = advertisementService;
        _settingService = settingService;
        _fileProvider = fileProvider;
        _offersPromotionsService = offersPromotionsService;
        _genericAttributeService = genericAttributeService;
        _storeService = storeService;
        _configuration = configuration;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns layout data for Career.Web _Layout (store title/description, SEO separator, IsTestSite).
    /// Career.Web calls this via HttpClient instead of injecting ISettingService/IStoreService in views.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLayoutData()
    {
        var store = await _storeService.GetCurrentStoreAsync();
        var storeId = store?.Id ?? 0;
        var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>(storeId);
        var isTestSite = _configuration.GetValue<bool>("IsTestSite");
        return Ok(new LayoutDataResponse
        {
            DefaultTitle = store?.DefaultTitle ?? string.Empty,
            HomepageDescription = store?.HomepageDescription ?? string.Empty,
            PageTitleSeparator = seoSettings?.PageTitleSeparator?.Trim() ?? string.Empty,
            IsTestSite = isTestSite
        });
    }

    [HttpGet]
    public async Task<IActionResult> PageNotFound()
    {
        var model = new PageNotFoundModel();
        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var topic = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.PageNotFound }, ContentManagementDefaults.GENERIC_ATTRIBUTE_KEY_BODY, nameof(LegalPage), storeId: storeId);
        if (!string.IsNullOrWhiteSpace(topic))
        {
            var cndSettings = await _settingService.LoadSettingAsync<NopAdvanceCDNSettings>(storeId);
            var mobileBody = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.PageNotFound }, ContentManagementDefaults.GENERIC_ATTRIBUTE_KEY_GROUP, nameof(LegalPage), storeId: storeId);
            if (!string.IsNullOrEmpty(topic))
                model.Body = topic.Replace("/images/uploaded/", cndSettings.CDNImageUrl + "/images/uploaded/");
            if (!string.IsNullOrEmpty(mobileBody))
                model.MobileBody = mobileBody.Replace("/images/uploaded/", cndSettings.CDNImageUrl + "/images/uploaded/");
        }

        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> ClearCache()
    {
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
        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> WeeklyAds()
    {
        var model = new WeeklyAdModel();
        var weeklyAds = await _advertisementService.GetActiveAdvertisementByStoreAndTypeAsync((int)AdvertisementTypeEnum.Weekly, (await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
        if (!weeklyAds.Any())
            return NotFound();

        foreach (var weeklyAd in weeklyAds)
            model.URLs.Add(new SelectListItem { Text = weeklyAd.Url, Value = weeklyAd.Id.ToString() });

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var advertisementSetting = await _settingService.LoadSettingAsync<AdvertisementSettingModel>(storeId);
        model.EcommPlugin = advertisementSetting.WeeklyAdEcommPlugin;

        return Ok(model);
    }

    [HttpGet]
    public async Task<IActionResult> CatalogAds()
    {
        var model = new CatalogAdModel();
        var catalogs = await _advertisementService.GetActiveAdvertisementByStoreAndTypeAsync((int)AdvertisementTypeEnum.Catalog, (await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
        if (!catalogs.Any())
            return NotFound();

        foreach (var catalog in catalogs)
            model.URLs.Add(new SelectListItem { Text = catalog.Url, Value = catalog.Id.ToString() });

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var catalogSettings = await _settingService.LoadSettingAsync<AdvertisementSettingModel>(storeId);
        model.EcommPlugin = catalogSettings.CatalogAdEcommPlugin;

        return Ok(model);
    }

    [HttpGet]
    public IActionResult RobotsTextFile()
    {
        //if (_appSettings.IsTestSite)
        //    return Ok("User-agent: * " + Environment.NewLine + "Disallow: / ");

        var sb = new StringBuilder();
        var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/"), "robots.custom.txt");
        if (_fileProvider.FileExists(robotsFilePath))
        {
            var robotsFileContent = _fileProvider.ReadAllText(robotsFilePath, Encoding.UTF8);
            sb.Append(robotsFileContent);
        }

        return Ok(sb.ToString());
    }

    [HttpGet]
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

        return Ok(model);
    }

    #endregion
}
