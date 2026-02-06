using Career.Data.Data.Caching;
using Career.Data.Domains.Blogs;
using Career.Data.Domains.Common;
using Career.Data.Domains.Customers;
using Career.Data.Extensions;
using Career.Data.Services.Blogs;
using Career.Data.Services.Common;
using Career.Data.Services.Customers;
using Career.Data.Services.LandingPages;
using Career.Data.Services.Locations;
using Career.Data.Services.PaycorAPI;
using Career.Data.Services.Seo;
using Career.Data.Services.Stores;
using Career.Web.Models.Customers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Career.Web.Models.Career;

public record SitemapGenerator
{
    #region Fields

    private XmlTextWriter _writer;
    private readonly IBlogService _blogService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly ILocationService _locationService;
    private readonly ICustomerService _customerService;
    private readonly ILandingPageService _landingPageService;
    private readonly ICommonService _commonService;
    private readonly IPaycorAPIService _paycorAPIService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreService _storeService;

    #endregion

    #region Ctor

    public SitemapGenerator(
        IBlogService blogService,
        IUrlRecordService urlRecordService,
        ICustomerService customerService,
        ILandingPageService landingPageService,
        ICommonService commonService,
        IPaycorAPIService paycorAPIService,
        IStaticCacheManager staticCacheManager,
        ILocationService locationService,
        IStoreService storeService)
    {
        _blogService = blogService;
        _urlRecordService = urlRecordService;
        _customerService = customerService;
        _landingPageService = landingPageService;
        _commonService = commonService;
        _paycorAPIService = paycorAPIService;
        _staticCacheManager = staticCacheManager;
        _locationService = locationService;
        _storeService = storeService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Method that is overridden, that handles creation of child urls.
    /// Use the method WriteUrlLocation() within this method.
    /// </summary>
    /// <param name="urlHelper">URL helper</param>
    protected async Task GenerateUrlNodesAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;

        // our story
        var ourstoryPageUrl = urlHelper.RouteUrl("ourstory").ToLower();
        WriteUrlLocation(scheme + ourstoryPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // careers
        var careersUrl = urlHelper.RouteUrl("careers").ToLower();
        WriteUrlLocation(scheme + careersUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // All jobs detail and Apply job
        await WriteJobDetailsAsync(urlHelper);

        // our brands
        var ourbrandsPageUrl = urlHelper.RouteUrl("brands").ToLower();
        WriteUrlLocation(scheme + ourbrandsPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // locations
        var locationsPageUrl = urlHelper.RouteUrl("locations").ToLower();
        WriteUrlLocation(scheme + locationsPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
        await WritePhysicalStoreDetailsAsync(urlHelper);

        // news
        var newsUrl = "/news/";
        WriteUrlLocation(scheme + newsUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // articles
        await WriteArticlesAsync(urlHelper);

        // privacy-policy
        var privacypolicyPageUrl = urlHelper.RouteUrl("privacy-policy").ToLower();
        WriteUrlLocation(scheme + privacypolicyPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // terms-conditions
        var termsconditionsPageUrl = urlHelper.RouteUrl("terms-conditions").ToLower();
        WriteUrlLocation(scheme + termsconditionsPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // conditions-of-use
        var conditionsofusePageUrl = urlHelper.RouteUrl("conditions-of-use").ToLower();
        WriteUrlLocation(scheme + conditionsofusePageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // summerjam
        var summerjamPageUrl = urlHelper.RouteUrl("summerjam").ToLower();
        WriteUrlLocation(scheme + summerjamPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // contest
        await WriteContestsAsync(urlHelper);

        // golf
        var golfPageUrl = urlHelper.RouteUrl("golf").ToLower();
        WriteUrlLocation(scheme + golfPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // golfSponsor
        var golfSponsorPageUrl = urlHelper.RouteUrl("golfSponsor").ToLower();
        WriteUrlLocation(scheme + golfSponsorPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // golf
        var golfRegisterPageUrl = urlHelper.RouteUrl("golfRegister").ToLower();
        WriteUrlLocation(scheme + golfRegisterPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // ourteam
        var ourteamPageUrl = urlHelper.RouteUrl("ourteam").ToLower();
        WriteUrlLocation(scheme + ourteamPageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);

        // bio
        await WriteBioAsync(urlHelper);

        var furnitureMartUsa = "http://furnituremartusa.com";
        WriteUrlLocation(furnitureMartUsa, UpdateFrequency.Weekly, DateTime.UtcNow);
    }

    protected void WriteUrlLocation(string url, UpdateFrequency updateFrequency, DateTime lastUpdated)
    {

        _writer.WriteStartElement("url");
        string loc = XmlHelper.XmlEncode(url);
        _writer.WriteElementString("loc", loc);
        _writer.WriteEndElement();
    }

    protected async Task WriteJobDetailsAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;
        var jobs = await _paycorAPIService.GetAllJobsAsync();
        if (jobs != null)
        {
            foreach (var opportunity in jobs)
            {
                var url = "/careers/careerdetail/?id=" + opportunity.Id;
                WriteUrlLocation(scheme + url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
        }
    }

    protected async Task WriteArticlesAsync(IUrlHelper urlHelper)
    {
        var scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;
        var blogPosts = await _blogService.GetBlogPostsAsync(storeId: (await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
        foreach (var blogPost in blogPosts)
        {
            var seName = await _urlRecordService.GetSeNameAsync(blogPost.Id, nameof(BlogPost));
            if (!string.IsNullOrEmpty(seName))
                seName = seName.Replace("inspiration/", "news/");

            var url = "/" + seName;
            WriteUrlLocation(scheme + url, UpdateFrequency.Weekly, DateTime.UtcNow);
        }
    }

    protected async Task WritePhysicalStoreDetailsAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;
        var physicalStores = await _locationService.GetLocationsAsync(((int)WebsiteEnum.FMUSA));
        foreach (var physicalStore in physicalStores)
            WriteUrlLocation(scheme+ "/locations/"+ physicalStore.LocationId +"/", UpdateFrequency.Weekly, DateTime.UtcNow);
    }

    protected async Task WriteBioAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;

        var customerListModel = new CustomerListModel
        {
            CustomerList = await (await _customerService.GetCustomersByRoleAsync(NopCustomerDefaults.OurTeamRoleName)).SelectAwait(async x => new CustomerModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DisplayOrder = (await _customerService.GetFMCustomersByCustomerIdAsync(x.Id)).DisplayOrder

            }).OrderBy(x => x.DisplayOrder == 0).ThenBy(x => x.DisplayOrder).ThenBy(x => x.LastName).ThenBy(x => x.FirstName).ToListAsync()
        };

        foreach (var author in customerListModel.CustomerList)
        {
            var seName = await _urlRecordService.GetSeNameAsync(author.Id, nameof(Customer));
            if (string.IsNullOrEmpty(seName))
                continue;

            var url = "/ourteam/" + seName.TrimEnd('/') + "/";
            WriteUrlLocation(scheme + url, UpdateFrequency.Weekly, DateTime.UtcNow);
        }
    }

    protected async Task WriteContestsAsync(IUrlHelper urlHelper)
    {
        string scheme = urlHelper.ActionContext.HttpContext.Request.Scheme + "://" + urlHelper.ActionContext.HttpContext.Request.Host;

        var urlRecords = await _urlRecordService.GetSlugsAsync(nameof(LandingPage), (await _storeService.GetCurrentStoreAsync())?.Id ?? 0);
        foreach (var urlRecord in urlRecords)
        {
            try
            {
                var contest = await _landingPageService.GetlandingPageByIdAsync(urlRecord.EntityId);
                var currentDate = _commonService.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
                if (contest == null)
                    continue;
                if (!contest.Published)
                    continue;
                if (contest.StartDateUtc != null && contest.StartDateUtc >= currentDate)
                    continue;
                if (contest.EndDateUtc != null && contest.EndDateUtc <= currentDate)
                    continue;

                var url = "/" + urlRecord.Slug;
                WriteUrlLocation(scheme + url, UpdateFrequency.Weekly, DateTime.UtcNow);
            }
            catch (Exception)
            {
                continue;
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// This will build an xml sitemap for better index with search engines.
    /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
    /// </summary>
    /// <param name="urlHelper">URL helper</param>
    /// <returns>Sitemap.xml as string</returns>
    public async Task<string> GenerateAsync(IUrlHelper urlHelper)
    {
        return await _staticCacheManager.GetAsync(CacheKeys.SitemapCacheKey, async () =>
        {
            using var stream = new MemoryStream();
            await GenerateAsync(urlHelper, stream);
            return Encoding.UTF8.GetString(stream.ToArray());
        });
    }

    /// <summary>
    /// This will build an xml sitemap for better index with search engines.
    /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
    /// </summary>
    /// <param name="urlHelper">URL helper</param>
    /// <param name="stream">Stream of sitemap.</param>
    public async Task GenerateAsync(IUrlHelper urlHelper, Stream stream)
    {
        using (_writer = new XmlTextWriter(stream, Encoding.UTF8))
        {
            _writer.Formatting = Formatting.Indented;
            _writer.WriteStartDocument();
            _writer.WriteStartElement("urlset");
            _writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            _writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            _writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            await GenerateUrlNodesAsync(urlHelper);

            _writer.WriteEndElement();
        }
    }

    #endregion
}
