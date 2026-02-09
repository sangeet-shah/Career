using Career.Web.Domains.Banner;
using Career.Web.Domains.Common;
using Career.Web.Domains.LegalPages;
using Career.Web.Domains.Media;
using Career.Web.Infrastructure;
using Career.Web.Models.Api;
using Career.Web.Models.Blogs;
using Career.Web.Models.CorporateManagement;
using Career.Web.Models.Legal;
using Career.Web.Models.Media;
using Career.Web.Models.Vendors;
using Career.Web.Rss;
using Career.Web.Services.ApiClient;
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

    private readonly IApiClient _apiClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAgentHelper _userAgentHelper;
    private readonly AppSettings _appSettings;

    #endregion

    #region Ctor

    public HomeController(
        IApiClient apiClient,
        IHttpContextAccessor httpContextAccessor,
        IUserAgentHelper userAgentHelper,
        AppSettings appSettings)
    {
        _apiClient = apiClient;
        _httpContextAccessor = httpContextAccessor;
        _userAgentHelper = userAgentHelper;
        _appSettings = appSettings;
    }

    #endregion

    #region Methods

    private async Task<string> GetPictureUrlAsync(int pictureId, int targetSize = 0, bool showDefaultPicture = true)
    {
        if (pictureId <= 0)
            return string.Empty;
        var urlRes = await _apiClient.GetAsync<PictureUrlResponse>("api/Picture/GetPictureUrl", new { pictureId, targetSize, showDefaultPicture });
        return urlRes?.Url ?? string.Empty;
    }

    public async Task<IActionResult> Index()
    {
        var model = new CorporateManagementSettingsModel();
        var mainBanner = await _apiClient.GetAsync<BannerDto>("api/BannerManagement/GetActiveBanner", new { bannerTypeId = (int)BannerTypeEnum.CorpHome, bannerDisplayTargetId = (int)DisplaySections.FMUSA_HomePage_TopBanner });
        if (mainBanner != null)
        {
            var picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = mainBanner.PictureId });

            model.HomeWebBannerId = mainBanner.Id;
            model.HomeWebBannerUrl = await GetPictureUrlAsync(mainBanner.PictureId);
            model.HomeMobileBannerUrl = await GetPictureUrlAsync(mainBanner.MobilePictureId);
            model.HomeBannerAltText = picture?.AltAttribute ?? mainBanner.Title;
            model.HomeBannerTitle = mainBanner.Title;
            model.HomeBannerLink = mainBanner.Url;
        }

        var corporateManagementSettings = await _apiClient.GetAsync<FMCorporateManagementSettingsDto>("api/Setting/GetFMCorporateManagementSettings", new { storeId = 0 });

        model.ContentDescription = corporateManagementSettings?.ContentDesktopDescription;
        model.ContentMobileDescription = corporateManagementSettings?.ContentMobileDescription;

        var leftBanner = await _apiClient.GetAsync<BannerDto>("api/BannerManagement/GetActiveBanner", new { bannerTypeId = (int)BannerTypeEnum.CorpHome, bannerDisplayTargetId = (int)DisplaySections.FMUSA_HomePage_Banner1 });
        if (leftBanner != null)
        {
            var picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = leftBanner.PictureId });
            model.LeftWebBannerId = leftBanner.Id;
            model.LeftWebBannerUrl = await GetPictureUrlAsync(leftBanner.PictureId);
            model.LeftMobileBannerUrl = await GetPictureUrlAsync(leftBanner.MobilePictureId);
            model.LeftBannerAltText = picture?.AltAttribute ?? leftBanner.Title;
            model.LeftBannerTitle = leftBanner.Title;
            model.LeftBannerLink = leftBanner.Url;
        }

        var rightBanner = await _apiClient.GetAsync<BannerDto>("api/BannerManagement/GetActiveBanner", new { bannerTypeId = (int)BannerTypeEnum.CorpHome, bannerDisplayTargetId = (int)DisplaySections.FMUSA_HomePage_Banner2 });
        if (rightBanner != null)
        {
            var picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = rightBanner.PictureId });
            model.RightWebBannerId = rightBanner.Id;
            model.RightWebBannerUrl = await GetPictureUrlAsync(rightBanner.PictureId);
            model.RightMobileBannerUrl = await GetPictureUrlAsync(rightBanner.MobilePictureId);
            model.RightBannerAltText = picture?.AltAttribute ?? rightBanner.Title;
            model.RightBannerTitle = rightBanner.Title;
            model.RightBannerLink = rightBanner.Url;
        }

        model.NumberOfNewsArticles = corporateManagementSettings?.NumberOfNewsArticles > 0 ? corporateManagementSettings.NumberOfNewsArticles : 0;
        model.IsMobile = _userAgentHelper.IsMobileDevice();

        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;
        var blogPostsPaged = await _apiClient.GetAsync<BlogPostPagedResultDto>("api/Blog/GetBlogPostsPaged", new { storeId, pageIndex = 0, pageSize = model.NumberOfNewsArticles });
        if (blogPostsPaged?.Items != null)
        {
            var list = new List<BlogPostModel>();
            foreach (var post in blogPostsPaged.Items)
            {
                var fmBlog = await _apiClient.GetAsync<FMBlogPostDto>("api/Blog/GetFMBlogByBlogPostId", new { blogPostId = post.Id });
                var pictureUrl = await GetPictureUrlAsync(fmBlog?.ThumbnailPictureId ?? 0);
                var seNameRes = await _apiClient.GetAsync<UrlRecordSeNameResponse>("api/UrlRecord/GetSeName", new { entityId = post.Id, entityName = "BlogPost" });
                list.Add(new BlogPostModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    StartDateUtc = post.StartDateUtc,
                    BodyOverview = post.BodyOverview,
                    FMUSAPictureURL = pictureUrl,
                    SeName = seNameRes?.SeName ?? string.Empty
                });
            }
            model.BlogPostList = list;
        }

        return View(model);
    }

    public async Task<IActionResult> OurStory()
    {
        var model = new CorporateManagementSettingsModel();
        var corporateManagementSettings = await _apiClient.GetAsync<FMCorporateManagementSettingsDto>("api/Setting/GetFMCorporateManagementSettings", new { storeId = 0 });
        model.OurStoryBanner = corporateManagementSettings?.WebBannerId ?? 0;
        var picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = model.OurStoryBanner });
        model.OurStoryBannerUrl = await GetPictureUrlAsync(model.OurStoryBanner);
        if (picture != null)
        {
            model.OurStoryBannerAltText = picture.AltAttribute ?? picture.TitleAttribute;
            model.OurStoryBannerTitle = picture.TitleAttribute;
        }

        model.OurStoryMobileBanner = corporateManagementSettings?.MobileBannerId ?? 0;
        picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = model.OurStoryMobileBanner });
        model.OurStoryMobileBannerUrl = await GetPictureUrlAsync(model.OurStoryMobileBanner);
        if (picture != null)
        {
            model.OurStoryMobileBannerAltText = picture.AltAttribute ?? picture.TitleAttribute;
            model.OurStoryMobileBannerTitle = picture.TitleAttribute;
        }

        model.OurStoryTitle = corporateManagementSettings?.Title;
        model.OurStoryFullDescription = corporateManagementSettings?.FullDescription;
        model.IsMobile = _userAgentHelper.IsMobileDevice();

        return View(model);
    }

    public async Task<IActionResult> OurBrands()
    {
        var model = new CorporateManagementSettingsModel();

        var careerBrands = await _apiClient.GetAsync<CorporateBrandPageDto[]>("api/Career/GetAllCorporateBrandPages");
        if (careerBrands != null)
        {
            foreach (var careerBrand in careerBrands)
            {
                var picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = careerBrand.PictureId });
                model.CareerBrandListModel.Add(new Models.Career.CareerBrandModel
                {
                    Id = careerBrand.Id,
                    Description = careerBrand.Description,
                    PictureUrl = await GetPictureUrlAsync(careerBrand.PictureId),
                    AltAttribute = picture?.AltAttribute,
                    TitleAttribute = picture?.TitleAttribute,
                    Url = careerBrand.Url
                });
            }
        }

        var vendors = await _apiClient.GetAsync<FMVendorDto[]>("api/Vendor/GetAllVendors");
        if (vendors != null)
        {
            foreach (var vendor in vendors)
            {
                var picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = vendor.CorporatePictureId });
                model.CorporateVendors.Add(new VendorModel
                {
                    IsCorporate = vendor.IsCorporate,
                    CorporatePictureId = vendor.CorporatePictureId,
                    CorporatePictureUrl = await GetPictureUrlAsync(vendor.CorporatePictureId),
                    CorporatePictureAltText = picture?.AltAttribute,
                    CorporatePictureTitle = picture?.TitleAttribute,
                    CorporateShortDescription = vendor.CorporateShortDescription
                });
            }
        }

        return View(model);
    }

    public async Task<IActionResult> PrivacyPolicy()
    {
        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;

        var titleRes = await _apiClient.GetAsync<LocaleStringResponse>("api/Localization/GetLocaleStringResourceByName",
            new { resourceName = string.Format(NopLocalizationDefaults.PUBLIC_LEGAL_PAGE_TITLE, LegalPageEnum.PrivacyPolicy.ToString()) });
        var bodyRes = await _apiClient.GetAsync<GenericAttributeValueResponse>("api/GenericAttribute/GetAttribute",
            new { entityId = (int)LegalPageEnum.PrivacyPolicy, keyGroup = ContentManagement.GENERIC_ATTRIBUTE_KEY_GROUP, key = ContentManagement.GENERIC_ATTRIBUTE_KEY_BODY, storeId });

        var model = new LegalModel
        {
            Title = titleRes?.Value,
            Body = bodyRes?.Value,
            SeName = "privacy-policy"
        };

        return View("GenericTopicDetail", model);
    }

    public async Task<IActionResult> TermsConditions()
    {
        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;

        var titleRes = await _apiClient.GetAsync<LocaleStringResponse>("api/Localization/GetLocaleStringResourceByName",
            new { resourceName = string.Format(NopLocalizationDefaults.PUBLIC_LEGAL_PAGE_TITLE, LegalPageEnum.TermsAndConditions.ToString()) });
        var bodyRes = await _apiClient.GetAsync<GenericAttributeValueResponse>("api/GenericAttribute/GetAttribute",
            new { entityId = (int)LegalPageEnum.TermsAndConditions, keyGroup = ContentManagement.GENERIC_ATTRIBUTE_KEY_GROUP, key = ContentManagement.GENERIC_ATTRIBUTE_KEY_BODY, storeId });

        var model = new LegalModel
        {
            Title = titleRes?.Value,
            Body = bodyRes?.Value,
            SeName = "terms-conditions"
        };

        return View("GenericTopicDetail", model);
    }

    public async Task<IActionResult> TermsOfUse()
    {
        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;

        var titleRes = await _apiClient.GetAsync<LocaleStringResponse>("api/Localization/GetLocaleStringResourceByName",
            new { resourceName = string.Format(NopLocalizationDefaults.PUBLIC_LEGAL_PAGE_TITLE, LegalPageEnum.TermsOfUse.ToString()) });
        var bodyRes = await _apiClient.GetAsync<GenericAttributeValueResponse>("api/GenericAttribute/GetAttribute",
            new { entityId = (int)LegalPageEnum.TermsOfUse, keyGroup = ContentManagement.GENERIC_ATTRIBUTE_KEY_GROUP, key = ContentManagement.GENERIC_ATTRIBUTE_KEY_BODY, storeId });

        var model = new LegalModel
        {
            Title = titleRes?.Value,
            Body = bodyRes?.Value,
            SeName = "conditions-of-use"
        };

        return View("GenericTopicDetail", model);
    }

    public async Task<IActionResult> ArticleList(BlogPostListModel model)
    {
        var query = HttpContext.Request.Query;
        if (query.ContainsKey("page") && int.TryParse(query["page"], out var parsedPage))
            model.Page = parsedPage;

        var corporateGallery = await _apiClient.GetAsync<CorporateGalleryDto>("api/Gallery/GetCorporateGallery");
        if (corporateGallery != null)
        {
            model.PhotoGalleryTitle = corporateGallery.Title;
            var galleries = await _apiClient.GetAsync<CorporateGalleryPictureDto[]>("api/Gallery/GetHighlightedGallery", new { corporateGalleryId = corporateGallery.Id });
            if (galleries != null && galleries.Any())
            {
                var list = new List<GalleryPictureMappingModel>();
                foreach (var g in galleries)
                {
                    var picture = await _apiClient.GetAsync<PictureDto>("api/Picture/GetById", new { pictureId = g.PictureId });
                    list.Add(new GalleryPictureMappingModel
                    {
                        Title = picture?.TitleAttribute,
                        AltText = picture?.AltAttribute,
                        ShortDescription = g.ShortDescription,
                        PictureURL = await GetPictureUrlAsync(g.PictureId),
                        Link = g.LinkUrl,
                        DisplayOrder = g.DisplayOrder
                    });
                }
                model.GalleryPictures = list;
            }
        }

        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;
        var blogPostsPaged = await _apiClient.GetAsync<BlogPostPagedResultDto>("api/Blog/GetBlogPostsPaged", new { storeId, pageIndex = model.Page - 1, pageSize = model.PageSize });
        if (blogPostsPaged?.Items != null)
        {
            var list = new List<BlogPostModel>();
            foreach (var post in blogPostsPaged.Items)
            {
                var fmBlog = await _apiClient.GetAsync<FMBlogPostDto>("api/Blog/GetFMBlogByBlogPostId", new { blogPostId = post.Id });
                var pictureUrl = await GetPictureUrlAsync(fmBlog?.ThumbnailPictureId ?? 0);
                var seNameRes = await _apiClient.GetAsync<UrlRecordSeNameResponse>("api/UrlRecord/GetSeName", new { entityId = post.Id, entityName = "BlogPost" });
                list.Add(new BlogPostModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    StartDateUtc = post.StartDateUtc,
                    BodyOverview = post.BodyOverview,
                    FMUSAPictureURL = pictureUrl,
                    SeName = seNameRes?.SeName ?? string.Empty
                });
            }
            model.BlogPostList = list;
        }

        model.TotalCount = blogPostsPaged?.TotalCount ?? 0;
        model.TotalPages = model.PageSize > 0 ? (int)Math.Ceiling(decimal.Divide(model.TotalCount, model.PageSize)) : 0;

        return View("ArticleList", model);
    }

    public async Task<IActionResult> NewsArticle(int id)
    {
        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;

        var nopAdvanceCDNSettings = await _apiClient.GetAsync<NopAdvanceCDNSettingsDto>("api/Setting/GetNopAdvanceCDNSettings", new { storeId });
        var blogPost = await _apiClient.GetAsync<BlogPostDto>("api/Blog/GetBlogPostById", new { storeId, blogPostId = id });
        if (blogPost == null)
            return InvokeHttp404();

        var fmBlog = await _apiClient.GetAsync<FMBlogPostDto>("api/Blog/GetFMBlogByBlogPostId", new { blogPostId = blogPost.Id });
        var model = new ArticleModel();
        model.BlogPost.Title = blogPost.Title;
        var seNameRes = await _apiClient.GetAsync<UrlRecordSeNameResponse>("api/UrlRecord/GetSeName", new { entityId = blogPost.Id, entityName = "BlogPost" });
        model.BlogPost.SeName = (seNameRes?.SeName ?? string.Empty).Replace("inspiration/", "").TrimEnd('/');
        model.BlogPost.MetaTitle = blogPost.MetaTitle;
        model.BlogPost.StartDateUtc = blogPost.StartDateUtc;
        var cdnUrl = nopAdvanceCDNSettings?.CDNImageUrl?.TrimEnd('/');
        if (!string.IsNullOrEmpty(blogPost.Body) && !string.IsNullOrEmpty(cdnUrl))
        {
            if (blogPost.Body.Contains("/images/uploaded/"))
                model.BlogPost.Body = blogPost.Body.Replace("/images/uploaded/", cdnUrl + "/images/uploaded/");
            else
                model.BlogPost.Body = blogPost.Body.Replace("/Images/uploaded/", cdnUrl + "/images/uploaded/");
        }
        else
        {
            model.BlogPost.Body = blogPost.Body;
        }
        model.BlogPost.BodyOverview = blogPost.BodyOverview;
        model.BlogPost.UpdatedDateUtc = fmBlog?.ModifiedDateUtc;

        var blogPictureId = (fmBlog?.SEOPictureId1 ?? 0) > 0 ? fmBlog.SEOPictureId1 : (fmBlog?.ThumbnailPictureId ?? 0);
        model.BlogPost.BlogBoxPicturePictureURL = await GetPictureUrlAsync(blogPictureId);
        model.BlogPost.SEOPictureUrl1 = await GetPictureUrlAsync(fmBlog?.SEOPictureId1 ?? 0);
        model.BlogPost.SEOPictureUrl2 = await GetPictureUrlAsync(fmBlog?.SEOPictureId2 ?? 0);
        model.BlogPost.SEOPictureUrl3 = await GetPictureUrlAsync(fmBlog?.SEOPictureId3 ?? 0);

        var httpContext = _httpContextAccessor.HttpContext?.Request;
        if (httpContext != null)
        {
            model.HostURL = string.Format("{0}://{1}", httpContext.Scheme, httpContext.Host.Value);
            model.CurrentPageURL = string.Format("{0}://{1}{2}{3}", httpContext.Scheme, httpContext.Host.Value, httpContext.Path, httpContext.QueryString);
        }

        var authors = await _apiClient.GetAsync<BlogPostAuthorDto[]>("api/Blog/GetBlogPostAuthorsByBlogPostId", new { blogPostId = blogPost.Id });
        if (authors != null && authors.Any())
        {
            var customers = await _apiClient.PostAsync<int[], CustomerDto[]>("api/Customer/GetCustomersByIds", authors.Select(a => a.AuthorId).Distinct().ToArray());
            if (customers == null || !customers.Any())
                return InvokeHttp404();

            foreach (var customer in customers)
            {
                var fmCustomer = await _apiClient.GetAsync<FMCustomerDto>("api/Customer/GetFMCustomerByCustomerId", new { customerId = customer.Id });
                var firstName = customer.FirstName;
                var lastName = customer.LastName;
                var roleRes = await _apiClient.GetAsync<IsInCustomerRoleResponse>("api/Customer/IsInCustomerRoleById", new { customerId = customer.Id, roleName = NopCustomerDefaults.AuthorRoleName });
                var isAuthorRole = roleRes?.IsInRole ?? false;
                var authorUrl = isAuthorRole && fmCustomer != null && fmCustomer.FMUSABio
                    ? model.HostURL + "/ourteam/" + firstName.ToLower().Replace(" ", "-") + "-" + lastName.ToLower().Replace(" ", "-") + "/"
                    : string.Empty;

                var avatarAttr = await _apiClient.GetAsync<GenericAttributeValueResponse>("api/GenericAttribute/GetAttribute", new { entityId = customer.Id, keyGroup = "Customer", key = NopCustomerDefaults.AvatarPictureIdAttribute, storeId = 0 });
                var avatarId = 0;
                int.TryParse(avatarAttr?.Value, out avatarId);
                var avatarUrl = await GetPictureUrlAsync(avatarId);

                var blogAutherDetailsModel = new BlogAuthorDetailsModel
                {
                    AuthorId = customer.Id,
                    AuthorFirstName = firstName,
                    AuthorLastName = lastName,
                    FMUSABio = fmCustomer?.FMUSABio ?? false,
                    AuthorPictureUrl = avatarUrl,
                    AuthorPosition = fmCustomer?.AuthorPosition ?? string.Empty,
                    AuthorType = fmCustomer?.IsOrganization ?? false,
                    AuthorUrl = authorUrl
                };

                model.BlogAuthors.Add(blogAutherDetailsModel);
            }
        }

        var blogPostsPaged = await _apiClient.GetAsync<BlogPostPagedResultDto>("api/Blog/GetBlogPostsPaged", new { storeId, pageIndex = 0, pageSize = 3 });
        if (blogPostsPaged?.Items != null)
        {
            var list = new List<BlogPostModel>();
            foreach (var post in blogPostsPaged.Items)
            {
                var seNameListRes = await _apiClient.GetAsync<UrlRecordSeNameResponse>("api/UrlRecord/GetSeName", new { entityId = post.Id, entityName = "BlogPost" });
                list.Add(new BlogPostModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    MetaTitle = post.MetaTitle,
                    StartDateUtc = post.StartDateUtc,
                    BodyOverview = post.BodyOverview,
                    SeName = seNameListRes?.SeName ?? string.Empty
                });
            }
            model.BlogPostList = list;
        }

        return View("NewsArticle", model);
    }

    public async Task<IActionResult> NewsRss()
    {
        var httpContext = _httpContextAccessor.HttpContext?.Request;
        var storeLocation = httpContext != null
            ? string.Format("{0}://{1}", httpContext.Scheme, httpContext.Host.Value)
            : string.Empty;

        var feed = new RssFeed(
            "News from The Furniture Mart USA",
            "The latest news articles from The Furniture Mart USA",
            new Uri(storeLocation.TrimEnd('/') + "/news/"),
            DateTime.UtcNow);

        var items = new List<RssItem>();
        var store = await _apiClient.GetAsync<StoreDto>("api/Store/GetCurrentStore");
        var storeId = store?.Id ?? 0;
        var recentBlogPosts = await _apiClient.GetAsync<BlogPostDto[]>("api/Blog/GetLatestBlogs", new { storeId, numberOfBlogs = 12 });
        if (recentBlogPosts != null)
        {
            foreach (var recentBlogPost in recentBlogPosts)
            {
                var seNameRes = await _apiClient.GetAsync<UrlRecordSeNameResponse>("api/UrlRecord/GetSeName", new { entityId = recentBlogPost.Id, entityName = "BlogPost" });
                var blogUrl = storeLocation.TrimEnd('/') + "/" + (seNameRes?.SeName ?? string.Empty);
                var content = string.IsNullOrEmpty(recentBlogPost.BodyOverview) ? string.Empty : Regex.Replace(recentBlogPost.BodyOverview, "<.*?>", string.Empty);
                var itemId = $"urn:store:{storeId}:news:blog:{recentBlogPost.Id}";
                items.Add(new RssItem(
                    (recentBlogPost.Title ?? string.Empty).Replace("<br>", "").Replace("<br/>", "").Replace("<br />", ""),
                    content,
                    new Uri(blogUrl.Replace("/inspiration/", "/news/")),
                    itemId,
                    recentBlogPost.StartDateUtc ?? DateTime.UtcNow));
            }
        }
        feed.Items = items;

        var pageUrl = httpContext != null
            ? string.Format("{0}://{1}{2}{3}", httpContext.Scheme, httpContext.Host.Value, httpContext.Path, httpContext.QueryString)
            : storeLocation;
        return new RssActionResult(feed, pageUrl);
    }

    #endregion
}
