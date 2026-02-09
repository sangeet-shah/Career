using Middleware.Web.Domains.Banner;
using Middleware.Web.Domains.Blogs;
using Middleware.Web.Domains.CDN;
using Middleware.Web.Domains.Common;
using Middleware.Web.Domains.CorporateManagement;
using Middleware.Web.Data.Extensions;
using Middleware.Web.Models.Blogs;
using Middleware.Web.Models.CorporateManagement;
using Middleware.Web.Models.Media;
using Middleware.Web.Models.Vendors;
using Microsoft.AspNetCore.Mvc;
using Middleware.Web.Filters;
using Middleware.Web.Services.Blogs;
using Middleware.Web.Services.Career;
using Middleware.Web.Services.Common;
using Middleware.Web.Services.Customers;
using Middleware.Web.Services.Localization;
using Middleware.Web.Services.Media;
using Middleware.Web.Services.Seo;
using Middleware.Web.Services.Settings;
using Middleware.Web.Services.Stores;
using Middleware.Web.Services.Vendors;
using System.Text.RegularExpressions;
using Middleware.Web.Infrastructure;
using Middleware.Web.Domains.Media;

namespace Middleware.Web.Controllers;

[Route("api/CareerHomePage/[action]")]
[ApiController]
[ApiKeyAuthorize]
public class CareerHomePageController : ControllerBase
{
    #region Fields

    private readonly ISettingService _settingService;
    private readonly IPictureService _pictureService;
    private readonly IVendorService _vendorService;
    private readonly ICommonService _commonService;
    private readonly IBlogService _blogService;
    private readonly IGalleryService _galleryService;
    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IBannerManagementService _bannerManagementService;
    private readonly ILocalizationService _localizationService;
    private readonly ICareerService _careerService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public CareerHomePageController(ISettingService settingService,
        IPictureService pictureService,
        IVendorService vendorService,
        ICommonService commonService,
        IBlogService blogService,
        IGalleryService galleryService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IUrlRecordService urlRecordService,
        IBannerManagementService bannerManagementService,
        ILocalizationService localizationService,
        ICareerService careerService,
        IStoreService storeService)
    {
        _settingService = settingService;
        _pictureService = pictureService;
        _vendorService = vendorService;
        _commonService = commonService;
        _blogService = blogService;
        _galleryService = galleryService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _urlRecordService = urlRecordService;
        _bannerManagementService = bannerManagementService;
        _localizationService = localizationService;
        _careerService = careerService;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new CorporateManagementSettingsModel();
        var mainbanner = await _bannerManagementService.GetActiveBannerAsync((int)BannerTypeEnum.CorpHome, (int)DisplaySections.FMUSA_HomePage_TopBanner);
        if (mainbanner != null)
        {
            var picture = await _pictureService.GetPictureByIdAsync(mainbanner.PictureId);

            model.HomeWebBannerId = mainbanner.Id;
            model.HomeWebBannerUrl = await _pictureService.GetPictureUrlAsync(mainbanner.PictureId);
            model.HomeMobileBannerUrl = await _pictureService.GetPictureUrlAsync(mainbanner.MobilePictureId);
            model.HomeBannerAltText = picture.AltAttribute ?? mainbanner.Title;
            model.HomeBannerTitle = mainbanner.Title;
            model.HomeBannerLink = mainbanner.Url;
        }

        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var corporateManagementSettings = await _settingService.LoadSettingAsync<FMCorporateManagementSettings>(storeId);

        model.ContentDescription = corporateManagementSettings.ContentDesktopDescription;
        model.ContentMobileDescription = corporateManagementSettings.ContentMobileDescription;

        var leftbanner = await _bannerManagementService.GetActiveBannerAsync((int)BannerTypeEnum.CorpHome, (int)DisplaySections.FMUSA_HomePage_Banner1);
        if (leftbanner != null)
        {
            var picture = await _pictureService.GetPictureByIdAsync(mainbanner.PictureId);
            model.LeftWebBannerId = leftbanner.Id;
            model.LeftWebBannerUrl = await _pictureService.GetPictureUrlAsync(leftbanner.PictureId);
            model.LeftMobileBannerUrl = await _pictureService.GetPictureUrlAsync(leftbanner.MobilePictureId);
            model.LeftBannerAltText = picture.AltAttribute ?? leftbanner.Title;
            model.LeftBannerTitle = leftbanner.Title;
            model.LeftBannerLink = leftbanner.Url;
        }

        var rightbanner = await _bannerManagementService.GetActiveBannerAsync((int)BannerTypeEnum.CorpHome, (int)DisplaySections.FMUSA_HomePage_Banner2);
        if (rightbanner != null)
        {
            var picture = await _pictureService.GetPictureByIdAsync(mainbanner.PictureId);
            model.RightWebBannerId = rightbanner.Id;
            model.RightWebBannerUrl = await _pictureService.GetPictureUrlAsync(rightbanner.PictureId);
            model.RightMobileBannerUrl = await _pictureService.GetPictureUrlAsync(rightbanner.MobilePictureId);
            model.RightBannerAltText = picture.AltAttribute ?? rightbanner.Title;
            model.RightBannerTitle = rightbanner.Title;
            model.RightBannerLink = rightbanner.Url;
        }

        model.NumberOfNewsArticles = corporateManagementSettings.NumberOfNewsArticles > 0 ? corporateManagementSettings.NumberOfNewsArticles : 0;

        var blogPosts = await _blogService.GetBlogPostsAsync(storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0, pageSize: model.NumberOfNewsArticles);
        model.BlogPostList = await blogPosts.SelectAwait(async x =>
        {
            var fmBlog = await _blogService.GetFMBlogByBlogPostIdAsync(x.Id);
            var pictureUrl = await _pictureService.GetPictureUrlAsync(fmBlog?.ThumbnailPictureId ?? 0);
            return new BlogPostModel
            {
                Id = x.Id,
                Title = x.Title,
                StartDateUtc = x.StartDateUtc,
                BodyOverview = x.BodyOverview,
                FMUSAPictureURL = pictureUrl,
                SeName = await _urlRecordService.GetSeNameAsync(x.Id, nameof(BlogPost))
            };
        }).ToListAsync();

        return Ok(model);
    }

    [HttpGet]
    public async Task<IActionResult> OurStory()
    {
        var model = new CorporateManagementSettingsModel();
        var storeId = (await _storeService.GetCurrentStoreAsync())?.Id ?? 0;
        var corporateManagementSettings = await _settingService.LoadSettingAsync<FMCorporateManagementSettings>(storeId);
        model.OurStoryBanner = corporateManagementSettings.WebBannerId;
        var picture = await _pictureService.GetPictureByIdAsync(corporateManagementSettings.WebBannerId);
        model.OurStoryBannerUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.WebBannerId);
        if (picture != null)
        {
            model.OurStoryBannerAltText = picture.AltAttribute ?? picture.TitleAttribute;
            model.OurStoryBannerTitle = picture.TitleAttribute;
        }

        model.OurStoryMobileBanner = corporateManagementSettings.MobileBannerId;
        picture = await _pictureService.GetPictureByIdAsync(corporateManagementSettings.MobileBannerId);
        model.OurStoryMobileBannerUrl = await _pictureService.GetPictureUrlAsync(corporateManagementSettings.MobileBannerId);
        if (picture != null)
        {
            model.OurStoryMobileBannerAltText = picture.AltAttribute ?? picture.TitleAttribute;
            model.OurStoryMobileBannerTitle = picture.TitleAttribute;
        }

        model.OurStoryTitle = corporateManagementSettings.Title;
        model.OurStoryFullDescription = corporateManagementSettings.FullDescription;

        return Ok(model);
    }

    

    

    

    

    #endregion
}
