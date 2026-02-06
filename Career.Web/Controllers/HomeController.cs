using Career.Data;
using Career.Data.Domains.Banner;
using Career.Data.Domains.Blogs;
using Career.Data.Domains.CDN;
using Career.Data.Domains.Common;
using Career.Data.Domains.CorporateManagement;
using Career.Data.Domains.LegalPages;
using Career.Data.Domains.Media;
using Career.Data.Extensions;
using Career.Data.Infrastructure;
using Career.Data.Services.Blogs;
using Career.Data.Services.Career;
using Career.Data.Services.Common;
using Career.Data.Services.Customers;
using Career.Data.Services.Localization;
using Career.Data.Services.Media;
using Career.Data.Services.Security;
using Career.Data.Services.Seo;
using Career.Data.Services.Settings;
using Career.Data.Services.Stores;
using Career.Data.Services.Vendors;
using Career.Web.Models.Blogs;
using Career.Web.Models.CorporateManagement;
using Career.Web.Models.Legal;
using Career.Web.Models.Media;
using Career.Web.Models.Vendors;
using Career.Web.Rss;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Career.Web.Controllers;

public class HomeController : BaseController
{
    #region Fields

    private readonly ISettingService _settingService;
    private readonly IPictureService _pictureService;
    private readonly IVendorService _vendorService;
    private readonly IPermissionService _permissionService;
    private readonly ICommonService _commonService;
    private readonly IBlogService _blogService;
    private readonly IGalleryService _galleryService;
    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBannerManagementService _bannerManagementService;
    private readonly ILocalizationService _localizationService;
    private readonly IWebHelper _webHelper;
    private readonly ICareerService _careerService;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public HomeController(ISettingService settingService,
        IPictureService pictureService,
        IVendorService vendorService,
        IPermissionService permissionService,
        ICommonService commonService,
        IBlogService blogService,
        IGalleryService galleryService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        IUrlRecordService urlRecordService,
        IHttpContextAccessor httpContextAccessor,
        IBannerManagementService bannerManagementService,
        ILocalizationService localizationService,
        IWebHelper webHelper,
        ICareerService careerService,
        IStoreService storeService)
    {
        _settingService = settingService;
        _pictureService = pictureService;
        _vendorService = vendorService;
        _permissionService = permissionService;
        _commonService = commonService;
        _blogService = blogService;
        _galleryService = galleryService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _urlRecordService = urlRecordService;
        _httpContextAccessor = httpContextAccessor;
        _bannerManagementService = bannerManagementService;
        _localizationService = localizationService;
        _webHelper = webHelper;
        _careerService = careerService;
        _storeService = storeService;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Index()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

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

        var corporateManagementSettings = await _settingService.LoadSettingAsync<FMCorporateManagementSettings>();

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

        model.IsMobile = _commonService.IsMobileDevice();

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

        return View(model);
    }

    public async Task<IActionResult> OurStory()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        var model = new CorporateManagementSettingsModel();
        var corporateManagementSettings = await _settingService.LoadSettingAsync<FMCorporateManagementSettings>();
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
        model.IsMobile = _commonService.IsMobileDevice();

        return View(model);
    }

    public async Task<IActionResult> OurBrands()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        var model = new CorporateManagementSettingsModel();

        // career brand
        var careerBrands = await _careerService.GetAllCorporateBrandPagesAsync();
        foreach (var careerBrand in careerBrands)
        {
            var picture = await _pictureService.GetPictureByIdAsync(careerBrand.PictureId);
            model.CareerBrandListModel.Add(new Models.Career.CareerBrandModel
            {
                Id = careerBrand.Id,
                Description = careerBrand.Description,
                PictureUrl = await _pictureService.GetPictureUrlCachingAsync(careerBrand.PictureId),
                AltAttribute = picture?.AltAttribute,
                TitleAttribute = picture?.TitleAttribute,
                Url = careerBrand.Url
            });
        }

        var vendors = await _vendorService.GetAllVendorsAsync();
        foreach (var vendor in vendors)
        {
            var picture = await _pictureService.GetPictureByIdAsync(vendor.CorporatePictureId);
            model.CorporateVendors.Add(new VendorModel
            {
                IsCorporate = vendor.IsCorporate,
                CorporatePictureId = vendor.CorporatePictureId,
                CorporatePictureUrl = await _pictureService.GetPictureUrlAsync(vendor.CorporatePictureId),
                CorporatePictureAltText = picture?.AltAttribute,
                CorporatePictureTitle = picture?.TitleAttribute,
                CorporateShortDescription = vendor.CorporateShortDescription
            });
        }

        return View(model);
    }

    public async Task<IActionResult> PrivacyPolicy()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        var model = new LegalModel
        {
            Title = await _localizationService.GetLocaleStringResourceByNameAsync(string.Format(NopLocalizationDefaults.PUBLIC_LEGAL_PAGE_TITLE, LegalPageEnum.PrivacyPolicy.ToString())),
            Body = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.PrivacyPolicy }, ContentManagement.GENERIC_ATTRIBUTE_KEY_BODY, nameof(LegalPage), storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0),
            SeName = "privacy-policy"
        };

        return View("GenericTopicDetail", model);
    }

    public async Task<IActionResult> TermsConditions()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        var model = new LegalModel
        {
            Title = await _localizationService.GetLocaleStringResourceByNameAsync(string.Format(NopLocalizationDefaults.PUBLIC_LEGAL_PAGE_TITLE, LegalPageEnum.TermsAndConditions.ToString())),
            Body = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.TermsAndConditions }, ContentManagement.GENERIC_ATTRIBUTE_KEY_BODY, nameof(LegalPage), storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0),
            SeName = "terms-conditions"
        };

        return View("GenericTopicDetail", model);
    }

    public async Task<IActionResult> TermsOfUse()
    {
        if (!await _permissionService.AuthorizeAsync())
            return RedirectToAction("Login", "Customer");

        var model = new LegalModel
        {
            Title = await _localizationService.GetLocaleStringResourceByNameAsync(string.Format(NopLocalizationDefaults.PUBLIC_LEGAL_PAGE_TITLE, LegalPageEnum.TermsOfUse.ToString())),
            Body = await _genericAttributeService.GetAttributeAsync<string>(new LegalPage { Id = (int)LegalPageEnum.TermsOfUse }, ContentManagement.GENERIC_ATTRIBUTE_KEY_BODY, nameof(LegalPage), storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0),
            SeName = "conditions-of-use"
        };

        return View("GenericTopicDetail", model);
    }

    public async Task<IActionResult> ArticleList(BlogPostListModel model)
    {
        // Try to get "page" from route
        var query = HttpContext.Request.Query;
        if (query.ContainsKey("page") && int.TryParse(query["page"], out var parsedPage))
            model.Page = parsedPage;

        var corporateGallery = await _galleryService.GetCorporateGalleryAsync();
        if (corporateGallery != null)
        {
            model.PhotoGalleryTitle = corporateGallery?.Title;

            var galleries = await _galleryService.GetHighlightedGalleryAsync(corporateGallery.Id);
            if (galleries.Any())
            {
                model.GalleryPictures = await galleries.SelectAwait(async x =>
                {
                    var picture = await _pictureService.GetPictureByIdAsync(x.PictureId);
                    return new GalleryPictureMappingModel
                    {
                        Title = picture?.TitleAttribute,
                        AltText = picture?.AltAttribute,
                        ShortDescription = x.ShortDescription,
                        PictureURL = await _pictureService.GetPictureUrlAsync(x.PictureId),
                        Link = x.LinkUrl,
                        DisplayOrder = x.DisplayOrder
                    };
                }).ToListAsync();
            }
        }
        var blogPosts = await _blogService.GetBlogPostsAsync(storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0, pageIndex: model.Page - 1, pageSize: model.PageSize);
        if (blogPosts.Any())
        {
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
        }

        model.TotalCount = blogPosts.TotalCount;
        model.TotalPages = (int)Math.Ceiling(decimal.Divide(model.TotalCount, model.PageSize));

        return View("ArticleList", model);
    }

    public async Task<IActionResult> NewsArticle(int id)
    {

        var nopAdvanceCDNSettings = await _settingService.LoadSettingAsync<NopAdvanceCDNSettings>((await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
        var blogPost = await _blogService.GetBlogPostByIdAsync((await _storeService.GetCurrentStoreAsync())?.Id ?? 0, id);
        if (blogPost == null)
            return InvokeHttp404();

        var fmBlog = await _blogService.GetFMBlogByBlogPostIdAsync(blogPost.Id);
        var model = new ArticleModel();
        model.BlogPost.Title = blogPost.Title;
        model.BlogPost.SeName = (await _urlRecordService.GetSeNameAsync(blogPost.Id, nameof(BlogPost))).Replace("inspiration/", "").TrimEnd('/');
        model.BlogPost.MetaTitle = blogPost.MetaTitle;
        model.BlogPost.StartDateUtc = blogPost.StartDateUtc;
        if (blogPost.Body != null && blogPost.Body.Contains("/images/uploaded/"))
            model.BlogPost.Body = blogPost.Body.Replace("/images/uploaded/", nopAdvanceCDNSettings.CDNImageUrl.TrimEnd('/') + "/images/uploaded/");
        else
            model.BlogPost.Body = blogPost.Body.Replace("/Images/uploaded/", nopAdvanceCDNSettings.CDNImageUrl.TrimEnd('/') + "/images/uploaded/");
        model.BlogPost.BodyOverview = blogPost.BodyOverview;
        model.BlogPost.UpdatedDateUtc = fmBlog?.ModifiedDateUtc ?? null;

        var blogPictureId = fmBlog?.SEOPictureId1 > 0 ? fmBlog?.SEOPictureId1 : (fmBlog?.ThumbnailPictureId ?? 0);
        model.BlogPost.BlogBoxPicturePictureURL = await _pictureService.GetPictureUrlAsync(blogPictureId.HasValue ? blogPictureId.Value : 0);

        model.BlogPost.SEOPictureUrl1 = await _pictureService.GetPictureUrlAsync(fmBlog?.SEOPictureId1 ?? 0);
        model.BlogPost.SEOPictureUrl2 = await _pictureService.GetPictureUrlAsync(fmBlog?.SEOPictureId2 ?? 0);
        model.BlogPost.SEOPictureUrl3 = await _pictureService.GetPictureUrlAsync(fmBlog?.SEOPictureId3 ?? 0);

        var httpContext = _httpContextAccessor.HttpContext.Request;
        model.HostURL = string.Format("{0}://{1}", httpContext.Scheme, httpContext.Host.Value);
        model.CurrentPageURL = string.Format("{0}://{1}{2}{3}", httpContext.Scheme, httpContext.Host.Value, httpContext.Path, httpContext.QueryString);

        // authors
        var authors = await _blogService.GetBlogPostAuthorsByBlogPostIdAsync(blogPost.Id);

        if (authors.Any())
        {
            var customers = await _customerService.GetCustomersByIdsAsync(authors.Select(c => c.AuthorId).ToArray());
            if (!customers.Any())
                return InvokeHttp404();

            foreach (var customer in customers)
            {
                var fmCustomer = await _customerService.GetFMCustomersByCustomerIdAsync(customer.Id);
                var fisrtName = customer.FirstName;
                var lastName = customer.LastName;
                var isAuthorRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AuthorRoleName, true);
                var authorUrl = isAuthorRole && fmCustomer != null && fmCustomer.FMUSABio ? model.HostURL + "/ourteam/" + fisrtName.ToLower().Replace(" ", "-") + "-" + lastName.ToLower().Replace(" ", "-") + "/" : string.Empty;

                var blogAutherDetailsModel = new BlogAuthorDetailsModel
                {
                    AuthorId = customer.Id,
                    AuthorFirstName = fisrtName,
                    AuthorLastName = lastName,
                    FMUSABio = fmCustomer?.FMUSABio ?? false,
                    AuthorPictureUrl = await _pictureService.GetPictureUrlAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute, "Customer")),
                    AuthorPosition = fmCustomer != null ? fmCustomer.AuthorPosition : string.Empty,
                    AuthorType = fmCustomer != null ? fmCustomer.IsOrganization : false,
                    AuthorUrl = authorUrl
                };

                model.BlogAuthors.Add(blogAutherDetailsModel);
            }
        }

        var blogPosts = await _blogService.GetBlogPostsAsync(storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0, pageSize: 3);
        model.BlogPostList = await blogPosts.SelectAwait(async x => new BlogPostModel
        {
            Id = x.Id,
            Title = x.Title,
            MetaTitle = x.MetaTitle,
            StartDateUtc = x.StartDateUtc,
            BodyOverview = x.BodyOverview,
            SeName = await _urlRecordService.GetSeNameAsync(x.Id, nameof(BlogPost))
        }).ToListAsync();

        return View("NewsArticle", model);
    }

    public async Task<IActionResult> NewsRss()
    {
        var feed = new RssFeed(
        "News from The Furniture Mart USA",
        "The latest news articles from The Furniture Mart USA",
        new Uri(_webHelper.GetStoreLocation() + "/news/"),
        DateTime.UtcNow);

        var items = new List<RssItem>();
        var recentBlogPosts = await _blogService.GetLatestBlogsAsync(storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0, numberOfBlogs: 12);
        foreach (var recentBlogPost in recentBlogPosts)
        {
            var blogUrl = _webHelper.GetStoreLocation() + "/" + await _urlRecordService.GetSeNameAsync(recentBlogPost.Id, nameof(BlogPost));
            var content = string.IsNullOrEmpty(recentBlogPost.BodyOverview) ? string.Empty : Regex.Replace(recentBlogPost.BodyOverview, "<.*?>", String.Empty);
            items.Add(new RssItem(recentBlogPost.Title.Replace("<br>", "").Replace("<br/>", "").Replace("<br />", ""), content, new Uri(blogUrl.Replace("/inspiration/", "/news/")), $"urn:store:{(await _storeService.GetCurrentStoreAsync())?.Id ?? 0}:news:blog:{recentBlogPost.Id}", recentBlogPost.StartDateUtc ?? DateTime.UtcNow));
        }
        feed.Items = items;

        return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
    }

    #endregion
}
